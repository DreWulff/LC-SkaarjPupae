using System.Collections.Generic;
using BepInEx.Configuration;
using LevelTypes = LethalLib.Modules.Levels.LevelTypes;

namespace SkaarjPupae.Configuration {
    public class PluginConfig {
        // For more info on custom configs, see https://lethal.wiki/dev/intermediate/custom-configs
        public Dictionary<LevelTypes, int> vanillaRarities;
        public Dictionary<string, int> customRarities;
        public PluginConfig(ConfigFile cfg) {
            // Spawn weights configuration
            ConfigHelper.Entities.Spawning.GetRarities(
                cfg: cfg,
                defaultWeights: new Dictionary<string, int>
                {
                    {"Experimentation", 20},
                    {"Vow", 30},
                    {"Mazon", 40},
                    {"Halation", 5},
                    {"Infernis", 20},
                    {"Junic", 30},
                },
                out vanillaRarities,
                out customRarities);

            ConfigHelper.General.ClearUnusedEntries(cfg);
        }
    }
}