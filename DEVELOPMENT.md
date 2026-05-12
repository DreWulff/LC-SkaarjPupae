# Introduction
As I am not an "authority" on the matter and whatever I say is a reflection of my personal experience, first person will be used in order to emphasize to the reader that the decisions made here and the workflow presented are personal preference and not a development standard or a strict guideline.

# Initial setup

# Building

# Working in Unity

# Testing

# Console Commands
```py
# Clear obsolete installs
dotnet nuget locals all --clear
dotnet tool uninstall -g Evaisa.NetcodePatcher.Cli

# Install netcode patcher
dotnet tool install -g Evaisa.NetcodePatcher.Cli

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