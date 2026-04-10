using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class PlayerInfo {
    
    public const string URL = "https://goldberries.net/api/player";

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("account")]
    public PlayerAccount Account { get; set; }

}