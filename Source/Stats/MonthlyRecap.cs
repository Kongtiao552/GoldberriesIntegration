using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Models;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class MonthlyRecap {

    [JsonProperty("month")]
    public int Month { get; set; }

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("time_spent")]
    public TimeSpan TimeSpent { get; set; } = TimeSpan.Zero;

    [JsonProperty("goldberries_points")]
    public double GoldberriesPoints { get; set; } = 0d;

}