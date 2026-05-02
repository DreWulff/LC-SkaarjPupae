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

# Unity Portion
- Save animations in Animations folder
- Save sounds in Sounds folder
- Save bestiary assets (video) in Bestiary folder
- Modify Bestiary scriptable objects TK and TN
- Save Materials in Materials folder
  - Keep MapDotRed
- Save model
- Modify enemy prefab