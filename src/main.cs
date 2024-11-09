using CounterStrikeSharp.API.Core;
using static CounterStrikeSharp.API.Core.Listeners;
using Microsoft.Extensions.Logging;


namespace SharpTimerTrails
{
    public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "SharpTimer Trails";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "exkludera + Marchand";

        public required PluginConfig Config { get; set; } = new PluginConfig();
        public int TrailCookie = 0;
        public Dictionary<CCSPlayerController, string> playerCookies = new();

        public void OnConfigParsed(PluginConfig config)
        {
            if (config.Version < Config.Version)
                Logger.LogWarning("Configuration version mismatch (Expected: {0} | Current: {1})", Config.Version, config.Version);

            Config = config;
        }

        public override void Load(bool hotReload)
        {
            InitializeDatabasePathAndConnectionString();

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