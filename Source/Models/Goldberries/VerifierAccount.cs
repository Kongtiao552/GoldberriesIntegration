using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class VerifierAccount {
    
    [JsonProperty("name_color_start")]
    public string NameColorStart { get; set; }

    [JsonProperty("name_color_end")]
    public string NameColorEnd { get; set; }
}