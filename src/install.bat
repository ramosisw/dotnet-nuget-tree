dotnet pack -c Local
dotnet tool uninstall --global dotnet-nuget-tree
dotnet tool install --global --add-source "c:\src\nugets" dotnet-nuget-tree