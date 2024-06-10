﻿using System.Text.RegularExpressions;
using System.Collections.Generic;
using dotnet.nuget.tree.Command;
using System.Linq;
using System;
using Microsoft.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Dynamic;

namespace dotnet.nuget.tree
{
    public class ProjectPackage
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<ProjectPackage> Dependencies { get; set; } = new List<ProjectPackage>();
    }

    public class Project
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> TargetFrameworks { get; set; }
        public List<ProjectPackage> Packages { get; set; }
    }

    class Program
    {
        private static readonly List<Regex> _excludedPatterns = new List<Regex>() {
            new Regex("[Oo]bj/"),
            new Regex("[Bb]in/"),
            new Regex("Test.cs$")
        };

        static PackageLoader _packageLoader;

        public async static Task Main(string[] args)
        {
            if (HelpCommand.HasHelpOption(args)) return;
            try
            {
                var options = OptionsCommand.Parse(args);
                if (options.Verbosity) Console.WriteLine("Working...");
                _packageLoader = new PackageLoader(options.DisableCache);
                await Run(options);
                if (options.Tree)
                    PrintTree(options);
                if (options.Verbosity && !options.DisableCache) Console.WriteLine($"Cache hits: {_packageLoader._cacheHits}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message + "\n");
                HelpCommand.WriteHelp();
            }
        }

        private static void PrintTree(OptionsCommand options)
        {
            Console.WriteLine("root");
            for (var i = 0; i < options.ProjectList.Count; i++)
            {
                var project = options.ProjectList[i];
                var isLast = i == options.ProjectList.Count - 1;
                var level = $"{(isLast ? "└──" : "├──")}";
                Console.WriteLine($"{level} {project.Name}/ {project.Packages.Count} packages");
                PrintPackages(project.Packages, $"{(isLast ? " " : "│")}   ");
            }

        }

        private static void PrintPackages(List<ProjectPackage> packages, string level = "")
        {
            if (packages.Count == 0) return;
            for (var i = 0; i < packages.Count; i++)
            {
                var package = packages[i];
                var isLast = i == packages.Count - 1;
                Console.WriteLine($"{level}{(isLast ? "└" : "├")}── {package.Name} [{package.Version}]");
                PrintPackages(package.Dependencies, level + $"{(isLast ? " " : "│")}   ");
            }
        }

        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }

        private static bool IsExcluded(string searchPath, string target)
        {
            var startPath = target.Replace(searchPath, string.Empty).Replace(@"\", "/");
            return _excludedPatterns.Any(d => d.Match(startPath).Success);
        }

        private async static Task Run(OptionsCommand options)
        {
            var deep = 1;
            for (var i = 0; i < options.ProjectList.Count; i++)
            {
                var project = options.ProjectList[i];
                if (options.Verbosity) Console.WriteLine($"{project.Name}/ {project.Packages.Count} packages");
                for (var j = 0; j < project.Packages.Count; j++)
                {
                    var package = project.Packages[j];
                    await GetDependencies(project, package, options, "   ", deep);
                }
            }
        }

        private async static Task GetDependencies(Project project, ProjectPackage package, OptionsCommand options, string level = "", int deep = 0)
        {
            if (deep >= options.Deep) return;
            if (options.Verbosity) Console.WriteLine($"{level}{package.Name} [{package.Version}]");
            level += "   ";

            await foreach (var packageDep in _packageLoader.GetDependenciesForProject(project, package.Name, package.Version))
            {
                await GetDependencies(project, packageDep, options, level + "   ", deep + 1);
                package.Dependencies.Add(packageDep);
            }
        }
    }
}
