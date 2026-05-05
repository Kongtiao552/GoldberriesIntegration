using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
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

    public static string GetCachedStatsFilePath(string statName) => Path.Combine(RootFolder, $"cached_v{Module.Metadata.VersionString}_{statName}.json");

    public static List<GBStat> Stats { get; set; } = new List<GBStat>() {
        MiscStat.Instance,
        GoldenTierStat.Instance,
        AnnualRecapStat.Instance
    };

    public static void RefreshCache() {
        // Delete cache files that don't match the current version
        foreach (string file in Directory.GetFiles(RootFolder, "cached_v*_*.json")) {
            if (!Path.GetFileName(file).StartsWith($"cached_v{Module.Metadata.VersionString}_")) {
                File.Delete(file);
            }
        }
    }

    public static void Initialize(bool useCache) {
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

        if (useCache) {
            Utils.Log("Loading Stats");

            foreach (GBStat stat in Stats) {
                string filePath = GetCachedStatsFilePath(stat.GetType().Name);

                if (File.Exists(filePath)) {
                    stat.Load(File.ReadAllText(filePath));
                } else {
                    Utils.Log($"Cache file for {stat.GetType().Name} not found, calculating stat normally");
                    stat.Calculate(Submissions);
                    File.WriteAllText(filePath, stat.Save());
                }
            }

            RefreshCache();
        } else {
            Utils.Log("Calculating Stats");

            foreach (GBStat stat in Stats) {
                stat.Calculate(Submissions);
                File.WriteAllText(GetCachedStatsFilePath(stat.GetType().Name), stat.Save());
            }
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
        IsFetching = true;
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
        Initialize(useCache: false);
        IsFetching = false;
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
        Initialize(useCache: false);
    }

    public static string PlayerName { get; set; }
    public static Color PlayerNameColor { get; set; }

    public static Dictionary<string, Color> TierColors { get; } = new Dictionary<string, Color>() {
        {"Untiered", Calc.HexToColor("#ffffff")},
        {"Undetermined", Calc.HexToColor("#aaaaaa")},
        {"Tier 1", Calc.HexToColor("#9696ff")},
        {"Tier 2", Calc.HexToColor("#93aeff")},
        {"Tier 3", Calc.HexToColor("#91c8ff")},
        {"Tier 4", Calc.HexToColor("#8eecff")},
        {"Tier 5", Calc.HexToColor("#8cffe2")},
        {"Tier 6", Calc.HexToColor("#89ffb0")},
        {"Tier 7", Calc.HexToColor("#9bff87")},
        {"Tier 8", Calc.HexToColor("#b7ff84")},
        {"Tier 9", Calc.HexToColor("#d5ff82")},
        {"Tier 10", Calc.HexToColor("#f4ff7f")},
        {"Tier 11", Calc.HexToColor("#fff47c")},
        {"Tier 12", Calc.HexToColor("#ffdd7a")},
        {"Tier 13", Calc.HexToColor("#ffc677")},
        {"Tier 14", Calc.HexToColor("#ffae75")},
        {"Tier 15", Calc.HexToColor("#ff9572")},
        {"Tier 16", Calc.HexToColor("#ff7c70")},
        {"Tier 17", Calc.HexToColor("#ff6d79")},
        {"Tier 18", Calc.HexToColor("#ff6daa")},
        {"Tier 19", Calc.HexToColor("#ff68d9")},
        {"Tier 20", Calc.HexToColor("#f266ff")},
        {"Tier 21", Calc.HexToColor("#d863ff")},
        {"Tier 22", Calc.HexToColor("#bd60ff")}
    };  

    public static int TierCount = 22;

}