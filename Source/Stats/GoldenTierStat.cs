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

        foreach (Submission submission in submissions) {
            if (!GoldberriesStatsManager.TryGetIntTier(submission, out int tier)) continue;

            GoldenTier submissionTier = GoldenTiers[tier - 1];

            if (submission.TimeTaken.HasValue) {
                if (submissionTier.Fastest == null) {
                    submissionTier.Fastest = submission;
                } else if (submissionTier.Fastest.TimeTaken.Value > submission.TimeTaken.Value) {
                    submissionTier.Fastest = submission;
                }

                if (submissionTier.Longest == null) {
                    submissionTier.Longest = submission;
                } else if (submissionTier.Longest.TimeTaken.Value < submission.TimeTaken.Value) {
                    submissionTier.Longest = submission;
                }

                if (!submission.IsObsolete) {
                    submissionTier.TimeSpent += submission.TimeTaken.Value;
                    TotalTimeSpent += submission.TimeTaken.Value;
                }
            }

            if (!submission.IsObsolete) {
                double gp = GoldberriesStatsManager.GetGP(tier);
                submissionTier.GoldberriesPoints += gp;
                TotalGoldberriesPoints += gp;
            }

            if (!submissionTier.HasBeenDone) {
                submissionTier.FirstAchieved = submission;
                submissionTier.HasBeenDone = true;
            } else if (submissionTier.FirstAchieved.DateAchieved > submission.DateAchieved) {
                submissionTier.FirstAchieved = submission;
            }
        }

        MaxTimeSpent = GoldenTiers.Max(tier => tier.TimeSpent);
        MaxGoldberriesPoints = GoldenTiers.Max(tier => tier.GoldberriesPoints);
    }

    
}