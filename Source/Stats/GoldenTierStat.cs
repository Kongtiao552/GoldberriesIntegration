using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesStat.Misc;
using Celeste.Mod.GoldberriesStat.Models;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesStat.Stats;

public class GoldenTierStat : GBStat {

    public static GoldenTierStat Instance { get; } = new GoldenTierStat();

    [JsonProperty("golden_tiers")]
    public List<GoldenTier> GoldenTiers { get; set; } = new List<GoldenTier>(capacity: StatManager.TierAmount);

    public override void Reset() {
        GoldenTiers?.Clear();
    }

    public override void InitializeStat(List<Submission> submissions) {
        for (int i = 1; i <= StatManager.TierAmount; i++) {
            GoldenTiers.Add(new GoldenTier(i));
        }

        Dictionary<GoldenTier, List<Submission>> tierSubmissions = submissions
            .GroupBy(s => GoldenTiers[s.Challenge.Difficulty.Sort - 1])
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (GoldenTier goldenTier in GoldenTiers) {
            List<Submission> submissionsForTier = tierSubmissions.GetValueOrDefault(goldenTier);

            if (submissionsForTier == null) continue;

            goldenTier.HasBeenDone = true;
            goldenTier.FirstCompleted = submissionsForTier.OrderBy(s => s.DateAchieved).First();

            List<Submission> submissionsByTime = submissionsForTier.Where(s => s.TimeTaken.HasValue).OrderBy(s => s.TimeTaken).ToList();
            goldenTier.Fastest = submissionsByTime.FirstOrDefault();
            goldenTier.Longest = submissionsByTime.LastOrDefault();

            goldenTier.TimeSpent = TimeSpan.FromSeconds(submissionsByTime.Sum(s => s.TimeTaken.Value.TotalSeconds));
            goldenTier.GoldberriesPoints = submissionsForTier.Sum(s => s.Challenge.Difficulty.GP);
        }
    }

}