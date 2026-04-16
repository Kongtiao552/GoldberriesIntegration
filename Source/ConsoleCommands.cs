using System;
using System.Threading.Tasks;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration;

public static class ConsoleCommands {

    private static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;
    private static GoldberriesIntegrationModule Module => GoldberriesIntegrationModule.Instance;
    
    [Command("gb_player_id", "get or set the player id")]
    public static void GbPlayerId(string stringpPlayerId = null) {
        if (string.IsNullOrEmpty(stringpPlayerId)) {
            Engine.Commands.Log($"Player ID: {ModSettings.PlayerId}");
            return;
        }

        if (!int.TryParse(stringpPlayerId, out int playerId) || playerId < 1) {
            Engine.Commands.Log("Please input a valid player id!");
            return;
        }

        ModSettings.PlayerId = playerId;
        Module.SaveSettings();
        Engine.Commands.Log($"Player ID set to {playerId}");
    }

    private static bool IsFetching { get; set; } = false;

    [Command("gb_fetch_stats", "fetch stats from goldberries.net")]
    public static async Task GbFetchStats() {
        if (ModSettings.PlayerId < 1) {
            Engine.Commands.Log("Please set a valid player id before fetching stats!");
            return;
        }

        if (IsFetching) {
            Engine.Commands.Log("Already fetching stats!");
            return;
        }

        IsFetching = true;

        Engine.Commands.Log("Fetching stats...");

        try {
            await GoldberriesStatsManager.Fetch(ModSettings.PlayerId);
            Engine.Commands.Log("Stats fetched successfully.");
        } catch (Exception e) {
            Engine.Commands.Log($"Error fetching stats: {e.Message}");
        } finally {
            IsFetching = false;
        }
    }
}