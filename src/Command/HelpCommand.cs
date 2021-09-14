using System;
using System.Linq;

namespace dotnet.nuget.tree.Command
{
    public static class HelpCommand
    {
        public static bool HasHelpOption(string[] args)
        {
            if (args.Contains("-h") || args.Contains("--help"))
            {
                WriteHelp();
                return true;
            }
            return false;
        }

        public static void WriteHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet-nuget-tree [options] <PATH|PROJECT>");
            Console.WriteLine("  ");
            Console.WriteLine("Arguments:");
            Console.WriteLine("  <PATH|PROJECT>  Where find *.csproj files to load nugets info. If a file is not specified, the command will search the current directory for one.");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  -v, --verbosity       Displays query packages.");
            Console.WriteLine("  -h, --help            Show command line help.");
            Console.WriteLine("  -d, --deep            Deep search tree, default 2.");
            Console.WriteLine("  -t, --tree            Output like a tree packages, default true.");
            // Console.WriteLine("  -c, --configuration   Load custom nuget.config, e.g. for private nugets.");
        }
    }
}