using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models;
using Microsoft.Xna.Framework;
using Monocle;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public static class StatManager {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;
    public static GoldberriesIntegrationModule Module => GoldberriesIntegrationModule.Instance;

    public static bool IsFetching { get; set; } = false;

    public static bool Initialized { get; set; } = false;

    public static bool StatsFetched { get; set; } = false;

    public static bool PlayerHasSubmissions { get; set; } = false;

    public static PlayerInfo PlayerInfo { get; set; }

    public static List<Submission> Submissions { get; set; }

    public static readonly string RootFolder = @".\GoldberriesIntegration\";
 
    public static readonly string PlayerInfoFile = Path.Combine(RootFolder, "player_info.json");
    public static readonly string SubmissionsFile = Path.Combine(RootFolder, "submissions.json");

    public static event Action OnStatInitialized;

    public static void CheckRootFolder() {
        if (!Directory.Exists(RootFolder)) {
            Directory.CreateDirectory(RootFolder);
        }
    }

    public static List<GBStat> Stats { get; set; } = new List<GBStat>() {
        MiscStat.Instance,
        GoldenTierStat.Instance,
        AnnualRecapStat.Instance
    };

    public static void Initialize() {
        if (!StatsFetched) {
            Utils.Log("Stats not fetched", LogLevel.Error);
            return;
        }

        if (Submissions.Count == 0) {
            Utils.Log("This player has no submissions", LogLevel.Warn);
            PlayerHasSubmissions = false;
            return;
        }

        PlayerHasSubmissions = true;
        PlayerName = PlayerInfo.Name;
        PlayerAccount account = PlayerInfo.Account;
        PlayerNameColor = account.NameColorStart == null ? Color.Black : Calc.HexToColor(account.NameColorStart);

        Utils.Log("Initializing Stats...");

        foreach (GBStat stat in Stats) {
            stat.InitializeStat(Submissions);
        }

        Initialized = true;
        OnStatInitialized?.Invoke();
    }

    public static void Reset() {
        StatsFetched = false;
        Initialized = false;
        Stats.ForEach(stat => stat.Reset());
        ResetStatsFile();
    }

    public static async Task Fetch(int playerId) {
        Utils.Log("Fetching Stats From goldberries.net");
        CheckRootFolder();

        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync($"{PlayerInfo.URL}?id={playerId}");
        if (response.StatusCode != HttpStatusCode.OK) {
            Utils.Log("Failed to fetch stats", LogLevel.Error);
            IsFetching = false;
            throw new HttpRequestException("Failed to fetch stats. Status Code: " + (int) response.StatusCode);
        }

        string json = await response.Content.ReadAsStringAsync();
        PlayerInfo = JsonConvert.DeserializeObject<PlayerInfo>(json);

        json = await client.GetStringAsync($"{Submission.URL}?player_id={playerId}&arbitrary=true&archived=true");
        Submissions = JsonConvert.DeserializeObject<List<Submission>>(json).Where(s => !s.Challenge.Difficulty.IsUntieredOrUndetermined && !s.IsObsolete).ToList();
        
        StatsFetched = true;
        SaveStatsFile();
        Initialize();
    }

    public static bool CheckStatsFile() {
        return File.Exists(SubmissionsFile) && File.Exists(PlayerInfoFile);
    }

    public static void SaveStatsFile(string playerInfo, string submissions) {
        File.WriteAllText(PlayerInfoFile, playerInfo);
        File.WriteAllText(SubmissionsFile, submissions);
    }

    public static void SaveStatsFile(PlayerInfo playerInfo, List<Submission> submissions) {
        SaveStatsFile(JsonConvert.SerializeObject(playerInfo, Formatting.Indented), JsonConvert.SerializeObject(submissions, Formatting.Indented));
    }

    public static void SaveStatsFile() {
        SaveStatsFile(PlayerInfo, Submissions);
    }

    public static void ResetStatsFile() {
        foreach (string file in Directory.GetFiles(RootFolder, "*.json")) {
            File.Delete(file);
        }
    }

    public static void LoadStatsFile() {
        PlayerInfo = JsonConvert.DeserializeObject<PlayerInfo>(File.ReadAllText(PlayerInfoFile));
        Submissions = JsonConvert.DeserializeObject<List<Submission>>(File.ReadAllText(SubmissionsFile));
        StatsFetched = true;
        Initialize();
    }

    public static string PlayerName { get; set; }
    public static Color PlayerNameColor { get; set; }

    public const int TierAmount = 22;

}