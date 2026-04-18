using System;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class GoldenTier {

    [JsonProperty("tier")]
    public int Tier { get; set; }

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("time_spent")]
    public TimeSpan TimeSpent { get; set; } = TimeSpan.Zero;

    [JsonProperty("goldberries_points")]
    public double GoldberriesPoints { get; set; } = 0d;

    public GoldenTier(int tier) => Tier = tier;

    [JsonProperty("has_been_done")]
    public bool HasBeenDone { get; set; } = false;

    [JsonProperty("first_achieved")]
    public Submission FirstAchieved  { get; set; }

    [JsonProperty("fastest")]
    public Submission Fastest  { get; set; }

    [JsonProperty("longest")]
    public Submission Longest  { get; set; }

    public string GetTierString() => "Tier " + Tier;

    public Color GetColor() => GoldberriesStatsManager.TierColors[GetTierString()];
}