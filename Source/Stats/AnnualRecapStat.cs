using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class AnnualRecapStat : GBStat {

    public static AnnualRecapStat Instance { get; } = new AnnualRecapStat();

    [JsonProperty("annual_recaps")]
    public List<AnnualRecap> AnnualRecaps { get; set; } = new List<AnnualRecap>();

    public override void Reset() {
        AnnualRecaps.Clear();
    }

    public override void InitializeStat(List<Submission> submissions) {
        Dictionary<int, List<Submission>> submissionsByYear = submissions
            .Where(s => !GoldberriesStatsManager.IsUntieredOrUndetermined(s))
            .GroupBy(s => s.DateAchieved.Year)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        foreach (KeyValuePair<int, List<Submission>> keyValuePair in submissionsByYear) {
            AnnualRecaps.Add(new AnnualRecap() {
                Year = keyValuePair.Key,
                SubmissionCount = keyValuePair.Value.Count,
                GoldberriesPoints = keyValuePair.Value.Sum(s => GoldberriesStatsManager.GetGP(s)),

                TimeSpent = TimeSpan.FromSeconds(
                    keyValuePair.Value.Where(
                        s => s.TimeTaken.HasValue
                    ).Sum(
                        s => s.TimeTaken.Value.TotalSeconds
                    )
                ),

                HardestSubmissions = keyValuePair.Value.OrderByDescending(
                    s => GoldberriesStatsManager.TryGetIntTier(s, out int tier) ? tier : -1
                ).Take(5).ToList(),

                LongestSubmissions = keyValuePair.Value.Where(
                    s => s.TimeTaken.HasValue
                ).OrderByDescending(
                    s => s.TimeTaken.Value.TotalMinutes
                ).Take(5).ToList(),
            });
        }
    }

}