using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models;

public class Verifier {

    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("account")]
    public VerifierAccount Account { get; set; }

    public override bool Equals(object obj) {
        return obj != null && obj is Verifier other && other.Id == Id;
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }

}