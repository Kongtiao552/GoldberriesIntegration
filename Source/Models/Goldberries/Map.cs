using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class Map {
    
    [JsonProperty("name")]
    public string Name { get; set; }

}