using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Models;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class MiscStat : GBStat {

    public static MiscStat Instance { get; } = new MiscStat();

    [JsonConverter(typeof(TimeSpanConverter))]
    [JsonProperty("total_time_spent")]
    public TimeSpan TotalTimeSpent { get; set; } = TimeSpan.Zero;

    [JsonProperty("total_goldberries_points")]
    public double TotalGoldberriesPoints { get; set; } = 0d;

    [JsonProperty("submissions_by_time")]
    public List<Submission> SubmissionsByTime { get; set; }

    [JsonProperty("verifiers")]
    // Tuple items:
    // 1. Verifier name
    // 2. Verifier account name color (hex)
    // 3. Number of submissions verified by this verifier
    public List<Tuple<string, string, int>> Verifiers { get; set; }

    public override void Reset() {
        TotalTimeSpent = TimeSpan.Zero;
        TotalGoldberriesPoints = 0d;

        SubmissionsByTime?.Clear();
        Verifiers?.Clear();
    }

    public override void InitializeStat(List<Submission> submissions) {
        List<Submission> timedSubmissions = submissions.Where(s => s.TimeTaken.HasValue).ToList();

        SubmissionsByTime = timedSubmissions.OrderByDescending(s => s.TimeTaken.Value).ToList();
        TotalTimeSpent = TimeSpan.FromSeconds(timedSubmissions.Sum(s => s.TimeTaken.Value.TotalSeconds));
        TotalGoldberriesPoints = submissions.Sum(s => s.Challenge.Difficulty.GP);

        int submissionsVerifiedByMoldenTeam = submissions.Count(s => s.Verifier == null);

        Verifiers = submissions
            .Where(s => s.Verifier != null)
            .GroupBy(s => s.Verifier)
            .Select(g => new Tuple<string, string, int>(g.Key.Name, g.Key.Account.NameColorStart, g.Count()))
            .ToList();

        if (submissionsVerifiedByMoldenTeam > 0) {
            Verifiers.Add(new Tuple<string, string, int>("Modded Golden Team", null, submissionsVerifiedByMoldenTeam));
        }

        Verifiers = Verifiers.OrderByDescending(v => v.Item3).ToList();
    }
    
}