using System;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class GoldenTier {
    public int Tier { get; set; }
    public TimeSpan TimeSpent { get; set; } = TimeSpan.Zero;
    public double GoldberriesPoints { get; set; } = 0d;

    public GoldenTier(int tier) => Tier = tier;

    public bool HasBeenDone { get; set; } = false;

    public Submission FirstAchieved  { get; set; }
    public Submission Fastest  { get; set; }
    public Submission Longest  { get; set; }

    public string GetTierString() => "Tier " + Tier;

    public Color GetColor() => GoldberriesStatsManager.TierColors[GetTierString()];
}