using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Extensions;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using static CounterStrikeSharp.API.Core.Listeners;
using Microsoft.Extensions.Logging;


namespace SharpTimerTrails
{
    public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "SharpTimer Trails";
        public override string ModuleVersion => "1.0.0";
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

        public void ReloadConfigCommand(CCSPlayerController? player, CommandInfo? command)
        {
            if (player != null && !AdminManager.PlayerHasPermissions(player, Config.Permission))
            {
                command?.ReplyToCommand($" {ChatColors.Red}You do not have the correct permission to execute this command.");
                return;
            }
            
            try
            {
                Config.Reload();
                command?.ReplyToCommand($" {ChatColors.White}[Trails] {ChatColors.Lime}Configuration reloaded successfully!");
            }
            catch (Exception)
            {
                command?.ReplyToCommand($" {ChatColors.White}[Trails] {ChatColors.Red}Failed to reload configuration.");
            }
        }

        public void UpdateConfigCommand(CCSPlayerController? player, CommandInfo? command)
        {
            if (player != null && !AdminManager.PlayerHasPermissions(player, Config.Permission))
            {
                command?.ReplyToCommand($" {ChatColors.White}[Trails] {ChatColors.Red}You do not have the correct permission to execute this command.");
                return;
            }

            try
            {
                Config.Update();
                command?.ReplyToCommand($" {ChatColors.White}[Trails] {ChatColors.Lime}Configuration updated successfully!");
            }
            catch (Exception)
            {
                command?.ReplyToCommand($" {ChatColors.White}[Trails] {ChatColors.Red}Failed to update configuration.");
            }
        }
    }
}