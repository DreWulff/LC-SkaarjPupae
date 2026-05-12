using System.Reflection;
using UnityEngine;
using BepInEx;
using LethalLib.Modules;
using BepInEx.Logging;
using System.IO;
using SkaarjPupae.Configuration;
using System.Collections.Generic;
using static LethalLib.Modules.Levels;

namespace SkaarjPupae
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = null!;
        internal static PluginConfig BoundConfig { get; private set; } = null!;
        public static AssetBundle? pupaeAssets;
        public static AssetBundle? plushieAssets;

        private void Awake()
        {
            Logger = base.Logger;

            // If you don't want your mod to use a configuration file, you can remove this line, Configuration.cs, and other references.
            BoundConfig = new PluginConfig(base.Config);

            // This should be ran before Network Prefabs are registered.
            InitializeNetworkBehaviours();

            // We load the asset bundle that should be next to our DLL file, with the specified name.
            // You may want to rename your asset bundle from the AssetBundle Browser in order to avoid an issue with
            // asset bundle identifiers being the same between multiple bundles, allowing the loading of only one bundle from one mod.
            // In that case also remember to change the asset bundle copying code in the csproj.user file.
            var bundleName = "skaarjpupae";
            pupaeAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), bundleName));
            if (pupaeAssets == null)
            {
                Logger.LogError($"Failed to load custom assets.");
                return;
            }

            var plushieBundle = "pupaeplushie";
            plushieAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), plushieBundle));
            if (pupaeAssets == null)
            {
                Logger.LogError($"Failed to load custom assets.");
                return;
            }

            // We load our assets from our asset bundle. Remember to rename them both here and in our Unity project.
            var SkaarjPupae = pupaeAssets.LoadAsset<EnemyType>("SkaarjPupaeEnemy");
            var SkaarjPupaeTN = pupaeAssets.LoadAsset<TerminalNode>("SkaarjPupaeTN");
            var SkaarjPupaeTK = pupaeAssets.LoadAsset<TerminalKeyword>("SkaarjPupaeTK");

            // Item PupaePlushie = plushieAssets.LoadAsset<Item>("PupaePlushie");
            // if (PupaePlushie != null)
            // {
            //     var properties = PupaePlushie.GetType().GetProperties();
            //     foreach (var property in properties)
            //     {
            //         var value = property.GetValue(PupaePlushie, null);
            //         Logger.LogInfo($"PupaePlushie Property: {property.Name} = {value}");
            //         NetworkPrefabs.RegisterNetworkPrefab(PupaePlushie.spawnPrefab);
            //         Utilities.FixMixerGroups(PupaePlushie.spawnPrefab);
            //         Items.RegisterScrap(PupaePlushie, 100, (LevelTypes)(-1));
            //     }
            // }
            // else
            // {
            //     Logger.LogError("PupaePlushie asset could not be loaded.");
            // }
            // Network Prefabs need to be registered. See https://docs-multiplayer.unity3d.com/netcode/current/basics/object-spawning/
            // LethalLib registers prefabs on GameNetworkManager.Start.
            NetworkPrefabs.RegisterNetworkPrefab(SkaarjPupae.enemyPrefab);

            // For different ways of registering your enemy, see https://github.com/EvaisaDev/LethalLib/blob/main/LethalLib/Modules/Enemies.cs
            // Enemies.RegisterEnemy(SkaarjPupae, BoundConfig.SpawnWeight.Value, Levels.LevelTypes.All, SkaarjPupaeTN, SkaarjPupaeTK);
            // For using our rarity tables, we can use the following:
            var SkaarjPupaeLevelRarities = new Dictionary<Levels.LevelTypes, int> {
                {Levels.LevelTypes.ExperimentationLevel, 50},
                {Levels.LevelTypes.AssuranceLevel, 200},
                {Levels.LevelTypes.VowLevel, 100},
                {Levels.LevelTypes.OffenseLevel, 30},
                {Levels.LevelTypes.AdamanceLevel, 150},
                //{Levels.LevelTypes.All, 30},     // Affects unset values, with lowest priority (gets overridden by Levels.LevelTypes.Modded)
                //{Levels.LevelTypes.Modded, 50},     // Affects values for modded moons that weren't specified
            };
            var SkaarjPupaeCustomLevelRarities = new Dictionary<string, int> {
                {"46 Infernis", 100},    // Either LLL or LE(C) name can be used, LethalLib will handle both
                {"76 Acidir", 300},
                {"84 Junic", 300},
                {"6 Mazon", 200},
                {"Halation", 100},
            };
            Enemies.RegisterEnemy(SkaarjPupae, SkaarjPupaeLevelRarities, SkaarjPupaeCustomLevelRarities, SkaarjPupaeTN, SkaarjPupaeTK);

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private static void InitializeNetworkBehaviours()
        {
            // See https://github.com/EvaisaDev/UnityNetcodePatcher?tab=readme-ov-file#preparing-mods-for-patching
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}