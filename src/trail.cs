using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using Microsoft.Extensions.Logging;

namespace SharpTimerTrails;

public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
{
    public int Tick { get; set; } = 1;
    static readonly Vector[] TrailLastOrigin = new Vector[64];
    static readonly Vector[] TrailEndOrigin = new Vector[64];
    private List<string> cachedTopPlayers = new List<string>();
    private DateTime lastFetchTime = DateTime.MinValue;
    private TimeSpan DatabaseRefreshInterval => TimeSpan.FromSeconds(Config.DatabaseRefreshInterval);
    private int colorIndex = 0;
    private readonly object cachedPlayersLock = new object();
    private bool isFetchingPlayers = false;

    public void OnTick()
    {
        Tick++;

        if (Tick < Config.TicksForUpdate)
            return;

        Tick = 0;

        if (!isFetchingPlayers && (DateTime.UtcNow - lastFetchTime) >= DatabaseRefreshInterval)
        {
            lastFetchTime = DateTime.UtcNow;
            isFetchingPlayers = true;
            Task.Run(async () =>
            {
                try
                {
                    var topPlayers = await GetTopPlayersAsync();
                    lock (cachedPlayersLock)
                    {
                        cachedTopPlayers = topPlayers;
                    }
                }
                finally
                {
                    isFetchingPlayers = false;
                }
            });
        }
        
        try
        {
            var allPlayers = Utilities.GetPlayers().Where(p => !p.IsBot).ToList();

            List<string> localCachedPlayers;
            lock (cachedPlayersLock)
            {
                localCachedPlayers = cachedTopPlayers.ToList();
            }

            for (int i = 0; i < localCachedPlayers.Count && i < Config.Trails.Count; i++)
            {
                var player = allPlayers.FirstOrDefault(p => p.SteamID.ToString() == localCachedPlayers[i]);

                if (player == null || !player.PawnIsAlive) continue;

                var absOrigin = player.PlayerPawn?.Value?.AbsOrigin;
                if (absOrigin == null) continue;

                if (VecCalculateDistance(TrailLastOrigin[player.Slot], absOrigin) > 5.0f)
                {
                    VecCopy(absOrigin, TrailLastOrigin[player.Slot]);
                    CreateTrail(player, absOrigin, i + 1);
                }
                
            }

            foreach (var player in allPlayers.Where(p => !localCachedPlayers.Contains(p.SteamID.ToString())))
            {
                if (!player.PawnIsAlive || !HasPermission(player))
                    continue;

                    var absOrigin = player.PlayerPawn?.Value?.AbsOrigin;
                    if (absOrigin == null) continue;

                if (VecCalculateDistance(TrailLastOrigin[player.Slot], absOrigin) > 5.0f)
                {
                    VecCopy(absOrigin, TrailLastOrigin[player.Slot]);
                    CreateTrail(player, absOrigin, 0);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error during OnTick. Current Tick: {Tick}, Cached Players Count: {cachedTopPlayers.Count}");
        }
    }

    public void CreateTrail(CCSPlayerController player, Vector absOrigin, int rank)
    {
        if (rank > Config.Trails.Count)
            return;

        string trailKey = rank > 0 ? rank.ToString() : "0";

        if (!Config.Trails.TryGetValue(trailKey, out var trailData))
        {
            Logger.LogWarning($"The trail key {trailKey} was not found in the cfg. Skipping trail creation for rank {rank}.");
            return;
        }

        if (trailData.File.EndsWith(".vpcf"))
            CreateParticle(player, absOrigin, trailData);
        else
            CreateBeam(player, absOrigin, trailData);
    }

    public void CreateParticle(CCSPlayerController player, Vector absOrigin, Trail trailData)
    {
        float lifetimeValue = trailData.Lifetime > 0 ? trailData.Lifetime : 1.0f;
     
        var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = trailData.File;
        particle.DispatchSpawn();
        particle.AcceptInput("Start");
        particle.AcceptInput("FollowEntity", player.PlayerPawn?.Value!, player.PlayerPawn?.Value!, "!activator");

        particle.Teleport(absOrigin, new QAngle(), new Vector());

        AddTimer(lifetimeValue, () =>
        {
            if (particle != null && particle.IsValid)
                particle.Remove();
        });
    }

    public void CreateBeam(CCSPlayerController player, Vector absOrigin, Trail trailData)
    {
        try
        {
            float teleportThreshold = Config.TeleportThreshold;
            string colorValue = !string.IsNullOrEmpty(trailData.Color) ? trailData.Color : "255 255 255";
            float widthValue = trailData.Width > 0 ? trailData.Width : 1.0f;
            float lifetimeValue = trailData.Lifetime > 0 ? trailData.Lifetime : 1.0f;

            Color color;
            if (string.IsNullOrEmpty(colorValue) || colorValue == "rainbow")
            {
                color = rainbowColors[colorIndex];
                colorIndex = (colorIndex + 1) % rainbowColors.Length;
            }
            else
            {
                var colorParts = colorValue.Split(' ');
                if (colorParts.Length == 3 &&
                    int.TryParse(colorParts[0], out var r) &&
                    int.TryParse(colorParts[1], out var g) &&
                    int.TryParse(colorParts[2], out var b))
                {
                    color = Color.FromArgb(255, r, g, b);
                }
                else
                {
                    Logger.LogWarning($"Invalid color format: {colorValue}, defaulting to white.");
                    color = Color.White;
                }
            }

            if (VecIsZero(TrailEndOrigin[player.Slot]))
            {
                VecCopy(absOrigin, TrailEndOrigin[player.Slot]);
                return;
            }

            float distance = VecCalculateDistance(TrailEndOrigin[player.Slot], absOrigin);

            if (distance > teleportThreshold)
            {
                if (Config.EnableDebug)
                {
                    Logger.LogInformation($"Skipping beam creation for player {player.SteamID} due to teleport detected. Distance: {distance}");
                }
                VecCopy(absOrigin, TrailEndOrigin[player.Slot]);
                return;
            }

            var beam = Utilities.CreateEntityByName<CEnvBeam>("env_beam")!;

            beam.Width = widthValue;
            beam.Render = color;
            beam.SpriteName = trailData.File; // WIP
            beam.SetModel(trailData.File);    // WIP

            beam.Teleport(absOrigin, new QAngle(), new Vector());

            VecCopy(TrailEndOrigin[player.Slot], beam.EndPos);
            VecCopy(absOrigin, TrailEndOrigin[player.Slot]);

            Utilities.SetStateChanged(beam, "CBeam", "m_vecEndPos");

            AddTimer(lifetimeValue, () =>
            {
                if (beam != null && beam.DesignerName == "env_beam")
                    beam.Remove();
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred while creating a beam for SteamID: {player.SteamID}, trail data: {trailData.Name}");
        }
    }
}