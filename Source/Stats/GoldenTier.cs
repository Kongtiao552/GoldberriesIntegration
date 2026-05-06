using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Models;
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

    public GoldenTier(int tier) {
        Tier = tier;
        TierString = "Tier " + Tier;

        Color = Difficulty.Colors[Tier];
    }

    [JsonProperty("has_been_done")]
    public bool HasBeenDone { get; set; } = false;

    [JsonProperty("first_completed")]
    public Submission FirstCompleted  { get; set; }

    [JsonProperty("fastest")]
    public Submission Fastest  { get; set; }

    [JsonProperty("longest")]
    public Submission Longest  { get; set; }

    public string TierString { get; private set; }

    public Color Color { get; private set; }

    public override bool Equals(object obj) {
        return obj != null && obj is GoldenTier other && Tier == other.Tier;
    }

    public override int GetHashCode() {
        return Tier.GetHashCode();
    }
}