using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public class AnnualRecapStat : GBStat {

    public static AnnualRecapStat Instance { get; } = new AnnualRecapStat();

    [JsonProperty("annual_recaps")]
    public List<AnnualRecap> AnnualRecaps { get; set; } = new List<AnnualRecap>();

    public override void Reset() {
        AnnualRecaps.Clear();
    }

    public override void CalculateStat(List<Submission> submissions) {
        AnnualRecaps = submissions
            .GroupBy(s => s.DateAchieved.Year)
            .Select(g => {
                List<Submission> submissionList = g.ToList();
                List<Submission> unobsoleted = submissionList.Where(s => !s.IsObsolete).ToList();
                List<Submission> unobsoletedWithTime = unobsoleted.Where(s => s.TimeTaken.HasValue).ToList();

                List<MonthlyRecap> monthlyRecaps = submissionList
                    .GroupBy(s => s.DateAchieved.Month)
                    .Select(g1 => {
                        List<Submission> monthlyUnobsoleted = g1.Where(s => !s.IsObsolete).ToList();

                        return new MonthlyRecap() {
                            Month = g1.Key,

                            TimeSpent = TimeSpan.FromSeconds(
                                monthlyUnobsoleted.Where(
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

                    GoldberriesPoints = unobsoleted.Sum(s => s.Challenge.Difficulty.GP),

                    TimeSpent = TimeSpan.FromSeconds(unobsoletedWithTime.Sum(s => s.TimeTaken.Value.TotalSeconds)),

                    HardestSubmissions = submissionList.OrderByDescending(
                        s => s.Challenge.Difficulty.Tier
                    ).Take(5).ToList(),

                    LongestSubmissions = unobsoletedWithTime.OrderByDescending(
                        s => s.TimeTaken.Value.TotalMinutes
                    ).Take(5).ToList(),

                    MonthlyRecaps = monthlyRecaps,
                    
                    MonthlyRecapMaxTimeSpent = monthlyRecaps.Max(mr => mr.TimeSpent),
                    MonthlyRecapMaxGP = monthlyRecaps.Max(mr => mr.GoldberriesPoints)
                };
            }).OrderBy(ar => ar.Year).ToList();
    }

}