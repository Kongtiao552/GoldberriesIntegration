using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Models;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class AnnualRecapStat : GBStat {

    public static AnnualRecapStat Instance { get; } = new AnnualRecapStat();

    [JsonProperty("annual_recaps")]
    public List<AnnualRecap> AnnualRecaps { get; set; }

    public override void Reset() {
        AnnualRecaps?.Clear();
    }

    public override void CalculateStat(List<Submission> submissions) {
        AnnualRecaps = submissions
            .GroupBy(s => s.DateAchieved.Year)
            .Select(g => {
                List<Submission> submissionList = g.ToList();
                List<Submission> timedSubmissions = submissionList.Where(s => s.TimeTaken.HasValue).ToList();

                List<MonthlyRecap> monthlyRecaps = submissionList
                    .GroupBy(s => s.DateAchieved.Month)
                    .Select(g1 => {
                        return new MonthlyRecap() {
                            Month = g1.Key,

                            TimeSpent = TimeSpan.FromSeconds(
                                g1.Where(
                                    s => s.TimeTaken.HasValue
                                ).Sum(
                                    s => s.TimeTaken.Value.TotalSeconds
                                )
                            ),

                            GoldberriesPoints = g1.Sum(s => s.Challenge.Difficulty.GP)
                        };
                    }).OrderBy(mr => mr.Month).ToList();

                return new AnnualRecap() {
                    Year = g.Key,
                    SubmissionCount = submissionList.Count,

                    GoldberriesPoints = submissionList.Sum(s => s.Challenge.Difficulty.GP),

                    TimeSpent = TimeSpan.FromSeconds(timedSubmissions.Sum(s => s.TimeTaken.Value.TotalSeconds)),

                    HardestSubmissions = submissionList.OrderByDescending(
                        s => s.Challenge.Difficulty.Sort
                    ).Take(5).ToList(),

                    LongestSubmissions = timedSubmissions.OrderByDescending(
                        s => s.TimeTaken.Value.TotalMinutes
                    ).Take(5).ToList(),

                    MonthlyRecaps = monthlyRecaps,
                };
            }).OrderBy(ar => ar.Year).ToList();
    }

}