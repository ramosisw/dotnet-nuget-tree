using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace dotnet.nuget.tree.Command
{
    public class OptionsCommand
    {
        public int Deep { get; set; } = 1;
        public string Configuration { get; set; }
        public string PathProject { get; set; }
        public List<Project> ProjectList { get; set; } = new List<Project>();
        public string OutputDir { get; set; }
        public bool Tree { get; set; } = true;
        public bool Verbosity { get; set; } = false;

        public OptionsCommand() => PathProject = Path.GetFullPath(".");

        public override string ToString() => $"Options [-d {Deep} -t {Tree} -v {Verbosity} {PathProject}]";

        public static OptionsCommand Parse(string[] args)
        {
            var validOptions = new[] {
                // "-c", "--configuration",
                // "-o", "--output-dir",
                "-d", "--deep",
                "-v", "--verbosity",
                "-t", "--tree",
            };
            var optionsCommand = new OptionsCommand();
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i].Trim();
                var argSplit = arg.Split("=");
                var argName = argSplit[0];
                var argValue = args[i];
                if (arg.StartsWith("-") && !validOptions.Contains(argName)) throw new ArgumentException($"Unknown option {argName}");
                if (arg.StartsWith("-") && !ArgWithoutValue(arg))
                {
                    if (arg.Contains("=") && !string.IsNullOrEmpty(argSplit[1]))
                        argValue = argSplit[1];
                    else if (i + 1 < args.Length)
                        argValue = args[i++ + 1];
                    else
                        throw new ArgumentException($"No value was specified for the option {argName}");

                }
                switch (argName.ToLower())
                {
                    case "-v":
                    case "--verbosity":
                        optionsCommand.Verbosity = true;
                        break;
                    case "-c":
                    case "--configuration":
                        optionsCommand.Configuration = argValue;
                        break;
                    case "-o":
                    case "--output-dir":
                        optionsCommand.OutputDir = argValue;
                        break;
                    case "-d":
                    case "--deep":
                        if (!int.TryParse(argValue, out var deep))
                            throw new ArgumentException($"Unknown value {argName}={argValue}; expects a integer number.");
                        optionsCommand.Deep = deep;
                        break;
                    case "-t":
                    case "--tree":
                        var tree = true;
                        if (!string.IsNullOrWhiteSpace(argValue) && !bool.TryParse(argValue, out tree))
                            throw new ArgumentException($"Unknown value {argName}={argValue}; expects true or false.");
                        optionsCommand.Tree = tree;
                        break;
                    default:
                        optionsCommand.PathProject = Path.GetFullPath(argValue);
                        break;
                }
            }

            var projectPathList = ResolveProjectFile(optionsCommand.PathProject);
            foreach (var projectPath in projectPathList)
            {
                var projectName = Path.GetFileNameWithoutExtension(projectPath);
                // Console.WriteLine($"Searching packages for {projectName}");
                var packages = GetPackages(projectPath);
                optionsCommand.ProjectList.Add(new Project
                {
                    Name = projectName,
                    Packages = packages,
                    Path = projectPath,
                    TargetFrameworks = GetTargetFrameworks(projectPath)
                });
            }
            if (optionsCommand.Verbosity) Console.WriteLine(optionsCommand);
            return optionsCommand;
        }

        private static bool ArgWithoutValue(string arg) => new[] { "-v", "--verbosity" }.Contains(arg);

        public static List<string> ResolveProjectFile(string path)
        {
            var projectFiles = new List<string>();
            var isFile = File.Exists(path);
            var isDirectory = Directory.Exists(path);

            if (isFile && !Path.GetExtension(path).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Only accept *.csproj files, actual {path}.");

            if (!isFile && !isDirectory)
                throw new ArgumentException($"The path not found, actual {path}");

            if (isDirectory)
                projectFiles.AddRange(Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories));

            if (isFile)
                projectFiles.Add(path);
            return projectFiles;
        }

        private static List<ProjectPackage> GetPackages(string fullProjectPath)
        {
            var packages = new List<ProjectPackage>();
            try
            {
                var projDefinition = XDocument.Load(fullProjectPath);
                packages = projDefinition
                    .Element("Project")
                    .Elements("ItemGroup")
                    .Elements("PackageReference")
                    .Select(e => new ProjectPackage
                    {
                        Name = e.Attribute("Include").Value,
                        Version = e.Attribute("Version").Value
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return packages;
        }

        private static List<string> GetTargetFrameworks(string fullProjectPath)
        {
            var targetFrameworksList = new List<string>();
            try
            {
                var projDefinition = XDocument.Load(fullProjectPath);
                var targetFramework = projDefinition
                    .Element("Project")
                    .Elements("PropertyGroup")
                    .Elements("TargetFramework")
                    .Select(e => e.Value)
                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(targetFramework)) targetFrameworksList.Add(targetFramework);
                else
                {
                    var targetFrameworks = projDefinition
                        .Element("Project")
                        .Elements("PropertyGroup")
                        .Elements("TargetFrameworks")
                        .Select(e => e.Value)
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(targetFrameworks)) targetFrameworksList.AddRange(targetFrameworks.Split(";"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return targetFrameworksList;
        }
    }
}