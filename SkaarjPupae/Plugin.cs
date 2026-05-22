using System.Reflection;
using UnityEngine;
using BepInEx;
using LethalLib.Modules;
using BepInEx.Logging;
using System.IO;
using SkaarjPupae.Configuration;

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

            InitializeNetworkBehaviours();

            LoadPupae();

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

        private void LoadPupae()
        {
            var bundleName = "skaarjpupae";
            pupaeAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), bundleName));
            if (pupaeAssets == null)
            {
                Logger.LogError($"Failed to load custom assets. Missing `{bundleName}` file.");
                return;
            }

            BoundConfig = new PluginConfig(base.Config);
            var SkaarjPupae = pupaeAssets.LoadAsset<EnemyType>("SkaarjPupaeEnemy");
            var SkaarjPupaeTN = pupaeAssets.LoadAsset<TerminalNode>("SkaarjPupaeTN");
            var SkaarjPupaeTK = pupaeAssets.LoadAsset<TerminalKeyword>("SkaarjPupaeTK");
            NetworkPrefabs.RegisterNetworkPrefab(SkaarjPupae.enemyPrefab);

            Enemies.RegisterEnemy(
                SkaarjPupae,
                BoundConfig.vanillaRarities,
                BoundConfig.customRarities,
                SkaarjPupaeTN,
                SkaarjPupaeTK
            );
        }
    }
}