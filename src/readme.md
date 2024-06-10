## Usage
```
Usage:
  dotnet-nuget-tree [options] <PATH|PROJECT>

Arguments:
  <PATH|PROJECT>  Where find *.csproj files to load nuget info. If a file is not specified, the command will search the current directory for one.

Options:
  -v, --verbosity       Displays query packages.
  -h, --help            Show command line help.
  -d, --deep            Deep search tree, default 1.
  -t, --tree            Output like a tree packages, default true.
  --disable-cache       Disable cache of packages, default false.
```

## Example
```
$ dotnet-nuget-tree -d 2
root
├── dotnet-nuget-tree/ 3 packages
│   ├── Microsoft.AspNet.WebApi.Client [5.2.7]
│   │   ├── Newtonsoft.Json [10.0.1]
│   │   └── Newtonsoft.Json.Bson [1.0.1]
│   ├── Microsoft.Build [15.9.20]
│   │   ├── Microsoft.Build.Framework [15.9.20]
│   │   ├── Microsoft.Win32.Registry [4.3.0]
│   │   ├── System.Collections.Immutable [1.5.0]
│   │   ├── System.Diagnostics.TraceSource [4.0.0]
│   │   ├── System.IO.Compression [4.3.0]
│   │   ├── System.Reflection.Metadata [1.6.0]
│   │   ├── System.Reflection.TypeExtensions [4.1.0]
│   │   ├── System.Runtime.InteropServices.RuntimeInformation [4.3.0]
│   │   ├── System.Runtime.Loader [4.0.0]
│   │   ├── System.Text.Encoding.CodePages [4.0.1]
│   │   └── System.Threading.Tasks.Dataflow [4.6.0]
│   └── Microsoft.CodeAnalysis [3.0.0]
│       ├── Microsoft.CodeAnalysis.CSharp.Workspaces [3.0.03.0.0]
│       └── Microsoft.CodeAnalysis.VisualBasic.Workspaces [3.0.03.0.0]
└── tests/ 3 packages
    ├── Microsoft.NET.Test.Sdk [15.9.0]
    ├── xunit [2.4.0]
    └── xunit.runner.visualstudio [2.4.0]
```
