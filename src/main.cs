using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using static CounterStrikeSharp.API.Core.Listeners;
using Microsoft.Extensions.Logging;


namespace SharpTimerTrails
{
    public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "SharpTimer Trails";
        public override string ModuleVersion => "1.0.1";
        public override string ModuleAuthor => "exkludera,  Marchand";

        public required PluginConfig Config { get; set; } = new PluginConfig();

        public void OnConfigParsed(PluginConfig config)
        {
            if (config.Version < Config.Version)
                Logger.LogWarning("Configuration version mismatch (Expected: {0} | Current: {1})", Config.Version, config.Version);

            Config = config;
        }

        public override void Load(bool hotReload)
        {
            try
            {
                Config.Reload();
            }
            catch (Exception)
            {
                Logger.LogWarning($"Failed to reload config file.");
            }
            
            if (Config.AutoUpdateConfig == true)
            {
                try
                {
                    Config.Update();
                }
                catch (Exception)
                {
                    Logger.LogWarning($"Failed to update config file.");
                }
            }
            
            InitializeDatabasePathAndConnectionString();
            colorIndex = 0;

            AddCommand($"css_reloadtrailscfg", "Reloads the trails config", ReloadConfigCommand);
            AddCommand($"css_updatetrailscfg", "Updates the trails config", UpdateConfigCommand);

            RegisterListener<OnTick>(OnTick);
            RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);

            for (int i = 0; i < 64; i++)
            {
                TrailEndOrigin[i] = new();
                TrailLastOrigin[i] = new();
            }
        }

        public override void Unload(bool hotReload)
        {
            RemoveListener<OnTick>(OnTick);
            RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        }
    }
}