using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class Challenge {
    
    [JsonProperty("difficulty")]
    public Difficulty Difficulty { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("map")]
    public Map Map { get; set; }

    [JsonProperty("campaign")]

    public Campaign Campaign { get; set; }
}