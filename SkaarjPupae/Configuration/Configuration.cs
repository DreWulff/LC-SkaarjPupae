using BepInEx.Configuration;

namespace SkaarjPupae.Configuration {
    public class PluginConfig
    {
        // For more info on custom configs, see https://lethal.wiki/dev/intermediate/custom-configs
        public ConfigEntry<int> SpawnWeight;
        public PluginConfig(ConfigFile cfg)
        {
            SpawnWeight = cfg.Bind("General", "Spawn weight", 100,
                "The spawn chance weight for SkaarjPupae, relative to other existing enemies.\n" +
                "Goes up from 0, lower is more rare, 100 and up is very common.");
        }
    }
}