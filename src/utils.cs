using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Drawing;

namespace SharpTimerTrails
{
    public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
    {
        public void OnServerPrecacheResources(ResourceManifest manifest)
        {
            foreach (KeyValuePair<string, Trail> trail in Config.Trails)
                manifest.AddResource(trail.Value.File);
        }

        public bool HasTrailPermission(CCSPlayerController player)
        {
            return string.IsNullOrEmpty(Config.TrailPermission) || AdminManager.PlayerHasPermissions(player, Config.TrailPermission);
        }

        public bool HasCommandPermission(CCSPlayerController player)
        {
            return string.IsNullOrEmpty(Config.CommandPermission) || AdminManager.PlayerHasPermissions(player, Config.CommandPermission);
        }

        public void ReloadConfigCommand(CCSPlayerController? player, CommandInfo? command)
        {
            if (player != null && !HasCommandPermission(player))
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
            if (player != null && !HasCommandPermission(player))
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

        public static float VecCalculateDistance(Vector vector1, Vector vector2)
        {
            float dx = vector2.X - vector1.X;
            float dy = vector2.Y - vector1.Y;
            float dz = vector2.Z - vector1.Z;

            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static void VecCopy(Vector vector1, Vector vector2)
        {
            vector2.X = vector1.X;
            vector2.Y = vector1.Y;
            vector2.Z = vector1.Z;
        }

        public static bool VecIsZero(Vector vector)
        {
            return vector.LengthSqr() == 0;
        }

        Color[] rainbowColors = {
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 255, 25, 0),
            Color.FromArgb(255, 255, 50, 0),
            Color.FromArgb(255, 255, 75, 0),
            Color.FromArgb(255, 255, 100, 0),
            Color.FromArgb(255, 255, 125, 0),
            Color.FromArgb(255, 255, 150, 0),
            Color.FromArgb(255, 255, 175, 0),
            Color.FromArgb(255, 255, 200, 0),
            Color.FromArgb(255, 255, 225, 0),
            Color.FromArgb(255, 255, 250, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 230, 255, 0),
            Color.FromArgb(255, 205, 255, 0),
            Color.FromArgb(255, 180, 255, 0),
            Color.FromArgb(255, 155, 255, 0),
            Color.FromArgb(255, 130, 255, 0),
            Color.FromArgb(255, 105, 255, 0),
            Color.FromArgb(255, 80, 255, 0),
            Color.FromArgb(255, 55, 255, 0),
            Color.FromArgb(255, 30, 255, 0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 255, 25),
            Color.FromArgb(255, 0, 255, 50),
            Color.FromArgb(255, 0, 255, 75),
            Color.FromArgb(255, 0, 255, 100),
            Color.FromArgb(255, 0, 255, 125),
            Color.FromArgb(255, 0, 255, 150),
            Color.FromArgb(255, 0, 255, 175),
            Color.FromArgb(255, 0, 255, 200),
            Color.FromArgb(255, 0, 255, 225),
            Color.FromArgb(255, 0, 255, 250),
            Color.FromArgb(255, 0, 255, 255),
            Color.FromArgb(255, 0, 230, 255),
            Color.FromArgb(255, 0, 205, 255),
            Color.FromArgb(255, 0, 180, 255),
            Color.FromArgb(255, 0, 155, 255),
            Color.FromArgb(255, 0, 130, 255),
            Color.FromArgb(255, 0, 105, 255),
            Color.FromArgb(255, 0, 80, 255),
            Color.FromArgb(255, 0, 55, 255),
            Color.FromArgb(255, 0, 30, 255),
            Color.FromArgb(255, 0, 0, 255),
            Color.FromArgb(255, 25, 0, 255),
            Color.FromArgb(255, 50, 0, 255),
            Color.FromArgb(255, 75, 0, 255),
            Color.FromArgb(255, 100, 0, 255),
            Color.FromArgb(255, 125, 0, 255),
            Color.FromArgb(255, 150, 0, 255),
            Color.FromArgb(255, 175, 0, 255),
            Color.FromArgb(255, 200, 0, 255),
            Color.FromArgb(255, 225, 0, 255),
            Color.FromArgb(255, 250, 0, 255),
            Color.FromArgb(255, 255, 0, 255),
            Color.FromArgb(255, 255, 0, 230),
            Color.FromArgb(255, 255, 0, 205),
            Color.FromArgb(255, 255, 0, 180),
            Color.FromArgb(255, 255, 0, 155),
            Color.FromArgb(255, 255, 0, 130),
            Color.FromArgb(255, 255, 0, 105),
            Color.FromArgb(255, 255, 0, 80),
            Color.FromArgb(255, 255, 0, 55),
            Color.FromArgb(255, 255, 0, 30)
        };
    }
}