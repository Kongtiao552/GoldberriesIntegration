using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesStat.Models;

public class Map {
    
    [JsonProperty("name")]
    public string Name { get; set; }

}