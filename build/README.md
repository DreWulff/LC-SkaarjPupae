# Build
The building process defined in this project's [`.csproj`](../plugin/SkaarjPupae.csproj) file consists of the following steps in order:
1. [`Directory.Build.props`](../Directory.Build.props): Checking if the user has defined the required properties for the rest of the process, with these properties being:
   1. `GamePath`: The absolute path to the game's root folder
   2. `ModdedPath`: The absolute path to either a modded version of the game or the Thunderstore/r2modman profile containing the required mods
   3. `UnityProjectDir`: The absolute path of the Unity Project in which the user will work
2. [`BuildSettings.prop`](./BuildSettings.props): Defining basic general properties
3. [`PrepAssemblies.targets`](PrepAssemblies.targets): Copying the required references into a `libs` folder
   1. Base `.dll` files to be copied are defined in [`PrepAssemblies.targets`](PrepAssemblies.targets)
   2. Project-specific dependencies have to be specified in the [`.csproj` file](../plugin/SkaarjPupae.csproj) under the `Reference` `ItemGroup`
4. Executing the building process defined in the `.csproj` file ([`SkaarjPupae.csproj`](../plugin/SkaarjPupae.csproj))
   1. This results in a `.dll` file in the `plugin/bin/Debug/netstandard2.1` folder. \
This file will be automatically copied into the resulting package and Unity project and therefore doesn't need to be manually retrieved.
1. [`Netcode.targets`](./Netcode.targets): Applies a netcode patch in  order to be able to use RPC methods
2. [`Packaging.targets`](./Packaging.targets): Copies multiple files from the project into a temporary `tmp-package` folder before compressing the folder's content into a `.zip` file. The copied files are:
   1. Everything inside the `package` folder
   2. The repository's [`README.md` file](../README.md)
   3. The repository's [`LICENSE` file](../LICENSE)
   4. The project's resulting `.dll` file
3. [`PrepUnityAssemblies.targets`](PrepUnityAssemblies.targets): Copies all required `.dll` files to the Unity Project. Said files are:
   1. The required game's assemblies
   2. The resulting `.dll` file from the building process
   3. Any defined dependencies of the package being built