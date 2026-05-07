using System;
using System.Threading.Tasks;
using Celeste.Mod.GoldberriesStat.Stats;
using Monocle;

namespace Celeste.Mod.GoldberriesStat;

public static class ConsoleCommands {

    private static GoldberriesStatModuleSettings ModSettings => GoldberriesStatModule.ModSettings;
    private static GoldberriesStatModule Module => GoldberriesStatModule.Instance;

    private static void Log(string log) {
        Engine.Commands.Log(log);
    }
    
    [Command("gb_player_id", "get or set the player id")]
    public static void GbPlayerId(string stringPlayerId = null) {
        if (!ModSettings.StatsEnabled) {
            Log("This feature is currently disabled!");
            return;
        }

        if (string.IsNullOrEmpty(stringPlayerId)) {
            Log($"Player ID: {ModSettings.PlayerId}");
            return;
        }

        if (!int.TryParse(stringPlayerId, out int playerId) || playerId < 1) {
            Log("Please input a valid player id!");
            return;
        }

        ModSettings.PlayerId = playerId;
        Module.SaveSettings();
        Log($"Player ID set to {playerId}");
    }

    [Command("gb_fetch_stats", "fetch stats from goldberries.net")]
    public static async Task GbFetchStats() {
        if (!ModSettings.StatsEnabled) {
            Log("This feature is currently disabled!");
            return;
        }

        if (ModSettings.PlayerId < 1) {
            Log("Please set a valid player id before fetching stats!");
            return;
        }

        if (StatManager.IsFetching) {
            Log("Already fetching stats!");
            return;
        }

        Log("Fetching stats...");
        StatManager.IsFetching = true;

        try {
            await StatManager.Fetch(ModSettings.PlayerId);
            Log("Stats fetched successfully.");
        } catch (Exception e) {
            Log($"Error fetching stats: {e}");
        } finally {
            StatManager.IsFetching = false;
        }
    }
}