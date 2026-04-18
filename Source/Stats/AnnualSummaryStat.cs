using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class AnnualSummaryStat : GBStat {

    public class AnnualSummary {

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("submissions_count")]
        public int SubmissionsCount { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        [JsonProperty("time_spent")]
        public TimeSpan TimeSpent { get; set; }

        [JsonProperty("goldberries_points")]
        public double GoldberriesPoints { get; set; }

        [JsonProperty("hardest_completed")]
        public Submission HardestCompleted { get; set; }
    }

    public static AnnualSummaryStat Instance { get; } = new AnnualSummaryStat();

    public override void InitializeStat(List<Submission> submissions) {
        throw new NotImplementedException();
    }

    public override void Reset() {
        throw new NotImplementedException();
    }
}