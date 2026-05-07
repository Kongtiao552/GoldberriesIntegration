using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesStat.Models;

public class Campaign {

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    public override bool Equals(object obj) {
        return obj != null && obj is Campaign other && other.Id == Id;
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }
    
}