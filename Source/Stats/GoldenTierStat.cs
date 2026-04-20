using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class GoldenTierStat : GBStat {

    public static GoldenTierStat Instance { get; } = new GoldenTierStat();

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("max_time_spent")]
    public TimeSpan MaxTimeSpent { get; set; } = TimeSpan.Zero;

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("total_time_spent")]
    public TimeSpan TotalTimeSpent { get; set; } = TimeSpan.Zero;

    [JsonProperty("max_goldberries_points")]
    public double MaxGoldberriesPoints { get; set; } = 0d;

    [JsonProperty("total_goldberries_points")]
    public double TotalGoldberriesPoints { get; set; } = 0d;

    [JsonProperty("golden_tiers")]
    public List<GoldenTier> GoldenTiers { get; set; } = new List<GoldenTier>(capacity: GoldberriesStatsManager.TierCount);

    public override void Reset() {
        GoldenTiers.Clear();

        MaxTimeSpent = TimeSpan.Zero;
        TotalTimeSpent = TimeSpan.Zero;

        MaxGoldberriesPoints = 0d;
        TotalGoldberriesPoints = 0d;

        Initialized = false;
    }

    public override void InitializeStat(List<Submission> submissions) {
        for (int i = 1; i <= GoldberriesStatsManager.TierCount; i++) {
            GoldenTiers.Add(new GoldenTier(i));
        }

        Dictionary<GoldenTier, List<Submission>> tierSubmissions = submissions
            .Where(s => !GoldberriesStatsManager.IsUntieredOrUndetermined(s))
            .GroupBy(s => GoldberriesStatsManager.TryGetIntTier(s, out int tier) ? GoldenTiers[tier - 1] : null)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (GoldenTier goldenTier in GoldenTiers) {
            List<Submission> submissionsForTier = tierSubmissions.GetValueOrDefault(goldenTier);

            if (submissionsForTier == null) continue;

            goldenTier.HasBeenDone = true;
            goldenTier.FirstAchieved = submissionsForTier.OrderBy(s => s.DateAchieved).First();

            List<Submission> submissionsByTime = submissionsForTier.Where(s => s.TimeTaken.HasValue).OrderBy(s => s.TimeTaken).ToList();
            goldenTier.Fastest = submissionsByTime.FirstOrDefault();
            goldenTier.Longest = submissionsByTime.LastOrDefault();

            goldenTier.TimeSpent = TimeSpan.FromSeconds(submissionsByTime.Where(s => !s.IsObsolete).Sum(s => s.TimeTaken.Value.TotalSeconds));
            goldenTier.GoldberriesPoints = submissionsForTier.Where(s => !s.IsObsolete).Sum(s => GoldberriesStatsManager.GetGP(s));

            TotalTimeSpent += goldenTier.TimeSpent;
            TotalGoldberriesPoints += goldenTier.GoldberriesPoints;
        }

        MaxTimeSpent = GoldenTiers.Max(tier => tier.TimeSpent);
        MaxGoldberriesPoints = GoldenTiers.Max(tier => tier.GoldberriesPoints);
    }

    
}