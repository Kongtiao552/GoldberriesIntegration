using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public static class GoldberriesStatsManager {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    public static bool Initialized { get; set; } = false;

    public static bool StatsFetched { get; set; } = false;

    public static PlayerInfo PlayerInfo { get; set; }

    public static List<Submission> Submissions { get; set; }

    public const string PLAYER_INFO_FILE = @".\gb_player_info.json";
    public const string SUBMISSIONS_FILE = @".\gb_submissions.json";

    public static List<GBStat> Stats { get; set; } = new List<GBStat>() {
        GoldenTierStat.Instance
    };

    public static void Initialize() {
        if (!StatsFetched) {
            Utils.Log("Stats not fetched", LogLevel.Error);
            return;
        }

        PlayerName = PlayerInfo.Name;
        PlayerAccount account = PlayerInfo.Account;
        PlayerNameColor = account.NameColorStart == null ? Color.Black : account.NameColorStart.ToColor();

        Utils.Log("Initializing Stats", LogLevel.Info);
        Stats.ForEach(stat => stat.Initialize(Submissions));

        foreach (GBStat stat in Stats) {
            if (!stat.Initialized) return;
        }

        Initialized = true;
    }

    public static void Reset() {
        Stats.ForEach(stat => stat.Reset());
        StatsFetched = false;
        Initialized = false;
        ResetStatsFile();
    }

    public static async Task Fetch(int playerId) {
        Utils.Log("Fetching Stats From goldberries.net", LogLevel.Info);

        try {
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{PlayerInfo.URL}?id={playerId}");

            if (response.StatusCode != HttpStatusCode.OK) {
                Utils.Log("Failed to fetch stats", LogLevel.Error);
                throw new HttpRequestException("Failed to fetch stats. Status Code: " + (int) response.StatusCode);
            }

            string json = await response.Content.ReadAsStringAsync();
            PlayerInfo = JsonConvert.DeserializeObject<PlayerInfo>(json);

            json = await client.GetStringAsync($"{Submission.URL}?player_id={playerId}&arbitrary=true&archived=true");
            Submissions = JsonConvert.DeserializeObject<List<Submission>>(json);

            if (Submissions.Count == 0) return;
            
            StatsFetched = true;
            SaveStatsFile();
            Initialize();
        } catch (HttpRequestException e) {
            Utils.Log($"Network error: {e.Message}", LogLevel.Error);
            throw;
        }
    }

    public static bool CheckStatsFile() {
        return File.Exists(SUBMISSIONS_FILE) && File.Exists(PLAYER_INFO_FILE);
    }

    public static void SaveStatsFile(string playerInfo, string submissions) {
        File.WriteAllText(PLAYER_INFO_FILE, playerInfo);
        File.WriteAllText(SUBMISSIONS_FILE, submissions);
    }

    public static void SaveStatsFile(PlayerInfo playerInfo, List<Submission> submissions) {
        SaveStatsFile(JsonConvert.SerializeObject(playerInfo, Formatting.Indented), JsonConvert.SerializeObject(submissions, Formatting.Indented));
    }

    public static void SaveStatsFile() {
        SaveStatsFile(PlayerInfo, Submissions);
    }

    public static void ResetStatsFile() {
        File.Delete(PLAYER_INFO_FILE);
        File.Delete(SUBMISSIONS_FILE);
    }

    public static void LoadStatsFile() {
        if (CheckStatsFile()) {
            PlayerInfo = JsonConvert.DeserializeObject<PlayerInfo>(File.ReadAllText(PLAYER_INFO_FILE));
            Submissions = JsonConvert.DeserializeObject<List<Submission>>(File.ReadAllText(SUBMISSIONS_FILE));
            StatsFetched = true;
            Initialize();
        }
    }

    public static string PlayerName { get; set; }
    public static Color PlayerNameColor { get; set; }

    public static Dictionary<string, Color> TierColors { get; } = new Dictionary<string, Color>() {
        {"Untiered", "#ffffff".ToColor()},
        {"Undetermined", "#aaaaaa".ToColor()},
        {"Tier 1", "#9696ff".ToColor()},
        {"Tier 2", "#93aeff".ToColor()},
        {"Tier 3", "#91c8ff".ToColor()},
        {"Tier 4", "#8eecff".ToColor()},
        {"Tier 5", "#8cffe2".ToColor()},
        {"Tier 6", "#89ffb0".ToColor()},
        {"Tier 7", "#9bff87".ToColor()},
        {"Tier 8", "#b7ff84".ToColor()},
        {"Tier 9", "#d5ff82".ToColor()},
        {"Tier 10", "#f4ff7f".ToColor()},
        {"Tier 11", "#fff47c".ToColor()},
        {"Tier 12", "#ffdd7a".ToColor()},
        {"Tier 13", "#ffc677".ToColor()},
        {"Tier 14", "#ffae75".ToColor()},
        {"Tier 15", "#ff9572".ToColor()},
        {"Tier 16", "#ff7c70".ToColor()},
        {"Tier 17", "#ff6d79".ToColor()},
        {"Tier 18", "#ff6daa".ToColor()},
        {"Tier 19", "#ff68d9".ToColor()},
        {"Tier 20", "#f266ff".ToColor()},
        {"Tier 21", "#d863ff".ToColor()},
        {"Tier 22", "#bd60ff".ToColor()}
    };  

    public static int TierCount = 22;

    public static bool IsUntieredOrUndetermined(string tier) => tier == "Untiered" || tier == "Undetermined";

    public static bool IsUntieredOrUndetermined(Submission submission) => IsUntieredOrUndetermined(submission.Challenge.Difficulty.Name);

    public static bool TryGetIntTier(string tier, out int intTier) {
        intTier = 0;

        if (tier == null) return false;
        if (IsUntieredOrUndetermined(tier)) return false;

        if (int.TryParse(tier.Substring(5), out int result)) {
            intTier = result;
            return true;
        }

        return false;
    }

    public static bool TryGetIntTier(Submission submission, out int intTier) {
        bool isTier = TryGetIntTier(submission.Challenge?.Difficulty?.Name, out int result);
        intTier = result;
        return isTier;
    }

}