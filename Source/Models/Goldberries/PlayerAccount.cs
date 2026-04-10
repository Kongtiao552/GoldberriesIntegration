using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class PlayerAccount {
    
    [JsonProperty("name_color_start")]
    public string NameColorStart { get; set; }

    [JsonProperty("name_color_end")]
    public string NameColorEnd { get; set; }

    [JsonProperty("input_method")]
    public string InputMethod { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }
}