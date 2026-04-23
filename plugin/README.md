# Console Commands
```py
# Clear obsolete installs
dotnet nuget locals all --clear
dotnet tool uninstall -g Evaisa.NetcodePatcher.Cli

# Install netcode patcher
dotnet tool install -g Evaisa.NetcodePatcher.Cli
dotnet tool restore

# Build project
dotnet build
```