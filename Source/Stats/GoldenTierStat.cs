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

    [JsonProperty("max_goldberries_points")]
    public double MaxGoldberriesPoints { get; set; } = 0d;

    [JsonProperty("golden_tiers")]
    public List<GoldenTier> GoldenTiers { get; set; } = new List<GoldenTier>(capacity: StatManager.TierCount);

    public override void Reset() {
        GoldenTiers.Clear();
        MaxTimeSpent = TimeSpan.Zero;
        MaxGoldberriesPoints = 0d;
    }

    public override void CalculateStat(List<Submission> submissions) {
        for (int i = 1; i <= StatManager.TierCount; i++) {
            GoldenTiers.Add(new GoldenTier(i));
        }

        Dictionary<GoldenTier, List<Submission>> tierSubmissions = submissions
            .GroupBy(s => GoldenTiers[s.Challenge.Difficulty.Tier - 1])
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (GoldenTier goldenTier in GoldenTiers) {
            List<Submission> submissionsForTier = tierSubmissions.GetValueOrDefault(goldenTier);

            if (submissionsForTier == null) continue;

            goldenTier.HasBeenDone = true;
            goldenTier.FirstCompleted = submissionsForTier.OrderBy(s => s.DateAchieved).First();

            List<Submission> submissionsByTime = submissionsForTier.Where(s => s.TimeTaken.HasValue).OrderBy(s => s.TimeTaken).ToList();
            goldenTier.Fastest = submissionsByTime.FirstOrDefault();
            goldenTier.Longest = submissionsByTime.LastOrDefault();

            goldenTier.TimeSpent = TimeSpan.FromSeconds(submissionsByTime.Where(s => !s.IsObsolete).Sum(s => s.TimeTaken.Value.TotalSeconds));
            goldenTier.GoldberriesPoints = submissionsForTier.Where(s => !s.IsObsolete).Sum(s => s.Challenge.Difficulty.GP);
        }

        MaxTimeSpent = GoldenTiers.Max(tier => tier.TimeSpent);
        MaxGoldberriesPoints = GoldenTiers.Max(tier => tier.GoldberriesPoints);
    }

    
}