using System;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class Submission {

    public const string URL = "https://goldberries.net/api/player/submissions";

    [JsonProperty("date_achieved")]
    public DateTime DateAchieved { get; set; }

    [JsonProperty("is_verified")]
    public bool IsVerified { get; set; }

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("time_taken")]
    public TimeSpan? TimeTaken { get; set; }

    [JsonProperty("proof_url")]
    public string ProofUrl { get; set; }

    [JsonProperty("challenge")]
    public Challenge Challenge { get; set; }

    [JsonProperty("verifier")]
    public Verifier Verifier { get; set; }

    [JsonProperty("is_fc")]
    public bool IsFc { get; set; }

    [JsonProperty("is_obsolete")]
    public bool IsObsolete { get; set; }

    public override string ToString() {
        string result = Challenge.Map?.Name;

        if (result == null) {
            // FGR
            result = $"{Challenge.Campaign.Name} ({Challenge.Label})";
        } else if (Challenge.Label != null) {
            // Arbitrary Challenge
            result += $" ({Challenge.Label})";
        }

        if (IsFc) {
            result += " FC";
        }

        return result;
    }

    public bool IsHarderThan(Submission other) {
        if (other == null) {
            return true;
        }

        // If this submission is untiered or undetermined, it's not harder than any other submission
        if (GoldberriesStatsManager.IsUntieredOrUndetermined(this)) {
            return false;
        }

        // If the other submission is untiered or undetermined and this submission is not, this submission is harder than it
        if (GoldberriesStatsManager.IsUntieredOrUndetermined(other)) {
            return true;
        }

        if (!GoldberriesStatsManager.TryGetIntTier(this, out int thisTier) || !GoldberriesStatsManager.TryGetIntTier(other, out int otherTier)) {
            Utils.Log("Cannot compare these two submissions because at least one of them has an invalid tier.", LogLevel.Warn);
            return false;
        }

        return thisTier > otherTier;
    }
    
}