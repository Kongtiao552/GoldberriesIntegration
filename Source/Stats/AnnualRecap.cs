using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class AnnualRecap {

    [JsonProperty("year")]
    public int Year { get; set; }

    [JsonProperty("submission_count")]
    public int SubmissionCount { get; set; } = 0;

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("time_spent")]
    public TimeSpan TimeSpent { get; set; } = TimeSpan.Zero;

    [JsonProperty("goldberries_points")]
    public double GoldberriesPoints { get; set; } = 0d;
    
    [JsonProperty("hardest_submissions")]
    public List<Submission> HardestSubmissions { get; set; } = new List<Submission>();

    [JsonProperty("longest_submissions")]
    public List<Submission> LongestSubmissions { get; set; } = new List<Submission>();

}