using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesStat.Models;

public class PlayerInfo {
    
    public const string URL = "https://goldberries.net/api/player";

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("account")]
    public PlayerAccount Account { get; set; }

    public override bool Equals(object obj) {
        return obj != null && obj is PlayerInfo other && other.Id == Id;
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }

}