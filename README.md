# dotnet-nuget-tree
[![.NET](https://github.com/ramosisw/dotnet-nuget-tree/actions/workflows/push-build.yml/badge.svg)](https://github.com/ramosisw/dotnet-nuget-tree/actions/workflows/push-build.yml)
[![.NET](https://github.com/ramosisw/dotnet-nuget-tree/actions/workflows/build-deploy.yml/badge.svg)](https://github.com/ramosisw/dotnet-nuget-tree/actions/workflows/build-deploy.yml)
[![License](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square&logo=read-the-docs)](https://github.com/ramosisw/dotnet-nuget-tree/blob/master/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/dotnet-nuget-tree.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/dotnet-nuget-tree/)
[![NuGet Download](https://img.shields.io/nuget/dt/dotnet-nuget-tree.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/dotnet-nuget-tree/)

A global .NET Core tool to list dependencies and sub-dependencies of projects in a directory or project file.

This tool was created to help you find libraries that are not compatible between nugets during a framework upgrade, for example, nuget C used in nugets A and B is not compatible and the build log only tells you that an inferior version of library C was detected and does not indicate who uses said library.

The .NET team has already developed something similar that I think is better by giving options such as which libraries you can update, which ones are obsolete or incompatible.

https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-list-package

## Table of Contents

- [Install](#installation)
- [Usage](#usage)
- [Example](#example)


# Installation
```sh
dotnet tool install --global dotnet-nuget-tree
```

# Usage

```
Usage:
  dotnet-nuget-tree [options] <PATH|PROJECT>

Arguments:
  <PATH|PROJECT>  Where find *.csproj files to load nugets info. If a file is not specified, the command will search the current directory for one.

Options:
  -v, --verbosity       Displays query packages.
  -h, --help            Show command line help.
  -d, --deep            Deep search tree, default 2.
  -t, --tree            Output like a tree packages, default true.
```

# Example

```powershell
$ dotnet-nuget-tree -d=2
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

