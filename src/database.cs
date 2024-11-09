using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Dapper;
using MySqlConnector;
using Npgsql;
using System.Data.SQLite;
using System.Data.Common;

namespace SharpTimerTrails
{
    public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
    {
        private string? _databasePath;
        private string? _connectionString;

        private void InitializeDatabasePathAndConnectionString()
        {
            var dbSettings = Config.DatabaseSettings;
            if (Config.DatabaseType == 1)
            {
                var mySqlSslMode = dbSettings.Sslmode.ToLower() switch
                {
                    "none" => MySqlSslMode.None,
                    "preferred" => MySqlSslMode.Preferred,
                    "required" => MySqlSslMode.Required,
                    "verifyca" => MySqlSslMode.VerifyCA,
                    "verifyfull" => MySqlSslMode.VerifyFull,
                    _ => MySqlSslMode.None
                };
                _connectionString = $@"Server={dbSettings.Host};Port={dbSettings.Port};Database={dbSettings.Database};Uid={dbSettings.Username};Pwd={dbSettings.Password};SslMode={mySqlSslMode};AllowPublicKeyRetrieval=True;";
            }
            else if (Config.DatabaseType == 2)
            {
                _databasePath = Path.Combine(Server.GameDirectory, "csgo", "cfg", "SharpTimer", "database.db");
                _connectionString = $"Data Source={_databasePath};Version=3;";
            }
            else if (Config.DatabaseType == 3)
            {
                var npgSqlSslMode = dbSettings.Sslmode.ToLower() switch
                {
                    "disable" => SslMode.Disable,
                    "require" => SslMode.Require,
                    "prefer" => SslMode.Prefer,
                    "allow" => SslMode.Allow,
                    "verify-full" => SslMode.VerifyFull,
                    _ => SslMode.Disable
                };
                _connectionString = $"Host={dbSettings.Host};Port={dbSettings.Port};Database={dbSettings.Database};Username={dbSettings.Username};Password={dbSettings.Password};SslMode={npgSqlSslMode};";
            }
        }

        public async Task<List<string>> GetTopPlayersAsync()
        {
            var topPlayers = new List<string>();
            var dbSettings = Config.DatabaseSettings;

            string query = "";
            DbConnection? connection = null;

            switch (Config.DatabaseType)
            {
                case 1: // MySQL
                    query = $@"
                        WITH RankedPlayers AS (
                            SELECT
                                SteamID,
                                PlayerName,
                                GlobalPoints,
                                DENSE_RANK() OVER (ORDER BY GlobalPoints DESC) AS playerPlace
                            FROM {dbSettings.TablePrefix}PlayerStats
                        )
                        SELECT SteamID
                        FROM RankedPlayers
                        WHERE playerPlace <= {Config.TopCount}
                        ORDER BY GlobalPoints DESC";
                    
                    connection = new MySqlConnection(_connectionString);
                    break;

                case 2: // SQLite
                    query = $@"
                        WITH RankedPlayers AS (
                            SELECT
                                SteamID,
                                PlayerName,
                                GlobalPoints,
                                DENSE_RANK() OVER (ORDER BY GlobalPoints DESC) AS playerPlace
                            FROM {dbSettings.TablePrefix}PlayerStats
                        )
                        SELECT SteamID
                        FROM RankedPlayers
                        WHERE playerPlace <= {Config.TopCount}
                        ORDER BY GlobalPoints DESC";
                    
                    connection = new SQLiteConnection(_connectionString);
                    break;

                case 3: // PostgreSQL
                    query = $@"
                        WITH RankedPlayers AS (
                            SELECT
                                ""SteamID"",
                                ""PlayerName"",
                                ""GlobalPoints"",
                                DENSE_RANK() OVER (ORDER BY ""GlobalPoints"" DESC) AS playerPlace
                            FROM ""{dbSettings.TablePrefix}PlayerStats""
                        )
                        SELECT ""SteamID""
                        FROM RankedPlayers
                        WHERE playerPlace <= {Config.TopCount}
                        ORDER BY ""GlobalPoints"" DESC";
                    
                    connection = new NpgsqlConnection(_connectionString);
                    break;
            }

            if (connection == null)
            {
                return topPlayers;
            }

            try
            {
                await connection.OpenAsync();
                topPlayers = (await connection.QueryAsync<string>(query)).ToList();
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching top players: {ex.Message}");
            }

            return topPlayers;
        }
    }
}