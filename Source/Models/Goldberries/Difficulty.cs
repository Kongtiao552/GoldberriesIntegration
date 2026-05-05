using System;
using System.Runtime.Serialization;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class Difficulty {
    
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public int Tier { get; private set; }

    [JsonIgnore]
    public bool IsUntieredOrUndetermined { get; private set; }

    [JsonIgnore]
    public Color Color { get; private set; }

    [JsonIgnore]
    public double GP { get; private set; } = 0d;

    [OnDeserialized]
    internal void Initialize(StreamingContext context) {
        IsUntieredOrUndetermined = Name == "Untiered" || Name == "Undetermined";

        if (!IsUntieredOrUndetermined && int.TryParse(Name.Substring(5), out int tier)) {
            Tier = tier;
            GP = Math.Pow(1.43d, tier - 1);
            Color = StatManager.TierColors[Name];
        }
    }

    public override bool Equals(object obj) {
        return obj != null && obj is Difficulty other && other.Tier == Tier;
    }

    public override int GetHashCode() {
        return Tier.GetHashCode();
    }

}