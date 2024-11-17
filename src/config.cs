using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace SharpTimerTrails
{
    public class Trail
    {
        public string Name { get; set; } = "Trail";
        public string File { get; set; } = "";
        public string Color { get; set; } = "rainbow";
        public float Width { get; set; } = 1.0f;
        public float Lifetime { get; set; } = 1.0f;
    }

    public sealed class DatabaseSettings
    {
        public string Host { get; set; } = "localhost";
        public string Database { get; set; } = "database";
        public string Username { get; set; } = "user";
        public string Password { get; set; } = "password";
        public int Port { get; set; } = 3306;
        public string Sslmode { get; set; } = "none";
        public string TablePrefix { get; set; } = "";
    }

    public class PluginConfig : BasePluginConfig
    {
        public int TopCount { get; set; } = 5;
        public string Permission { get; set; } = "@css/root";
        public int TicksForUpdate { get; set; } = 1;
        public float TeleportThreshold { get; set; } = 100.0f;
        public int DatabaseRefreshInterval { get; set; } = 120;
        public int DatabaseType { get; set; } = 1; // 1 = MySQL, 2 = SQLite, 3 = PostgreSQL
        public DatabaseSettings DatabaseSettings { get; set; } = new DatabaseSettings();
        public Dictionary<string, Trail> Trails { get; set; } = new()
        {
            { "0", new Trail { Name = "Fire Trail", File = "particles/explosions_fx/molotov_child_flame02a.vpcf", Lifetime = 5.0f } },
            { "1", new Trail { Name = "Glowing Sparks Trail", File = "particles/ambient_fx/ambient_sparks_glow.vpcf", Lifetime = 5.0f } },
            { "2", new Trail { Name = "Rainbow Trail", Color = "rainbow", Width = 1.0f, Lifetime = 3.0f } },
            { "3", new Trail { Name = "Red Trail", Color = "255 0 0", Width = 1.0f, Lifetime = 3.0f } },
            { "4", new Trail { Name = "Green Trail", Color = "0 255 0", Width = 1.0f, Lifetime = 3.0f } },
            { "5", new Trail { Name = "Blue Trail", Color = "0 0 255", Width = 1.0f, Lifetime = 3.0f } }
        };
        public bool EnableDebug { get; set; } = false;

        [JsonPropertyName("ConfigVersion")] 
        public override int Version { get; set; } = 1;
    }
}