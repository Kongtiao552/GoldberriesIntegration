using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class MiscStat : GBStat {

    public static MiscStat Instance { get; } = new MiscStat();

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("total_time_spent")]
    public TimeSpan TotalTimeSpent { get; set; } = TimeSpan.Zero;

    [JsonProperty("total_goldberries_points")]
    public double TotalGoldberriesPoints { get; set; } = 0d;

    public override void Reset() {
        TotalTimeSpent = TimeSpan.Zero;
        TotalGoldberriesPoints = 0d;
    }

    public override void CalculateStat(List<Submission> submissions) {
        List<Submission> unobsoleted = submissions.Where(s => !s.IsObsolete).ToList();

        TotalTimeSpent = TimeSpan.FromSeconds(unobsoleted.Where(s => s.TimeTaken.HasValue).Sum(s => s.TimeTaken.Value.TotalSeconds));
        TotalGoldberriesPoints = unobsoleted.Sum(s => s.Challenge.Difficulty.GP);
    }

    
}