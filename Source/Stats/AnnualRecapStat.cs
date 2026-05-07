using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Celeste.Mod.GoldberriesStat.Models;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesStat.Stats;

public class AnnualRecapStat : GBStat {

    public static AnnualRecapStat Instance { get; } = new AnnualRecapStat();

    [JsonProperty("annual_recaps")]
    public List<AnnualRecap> AnnualRecaps { get; set; }

    public override void Reset() {
        AnnualRecaps?.Clear();
    }

    public override void InitializeStat(List<Submission> submissions) {
        AnnualRecaps = submissions
            .GroupBy(s => s.DateAchieved.Year)
            .Select(g => {
                List<Submission> submissionList = g.ToList();
                List<Submission> timedSubmissions = submissionList.Where(s => s.TimeTaken.HasValue).ToList();

                Dictionary<int, List<Submission>> rawRecaps = submissionList
                    .GroupBy(s => s.DateAchieved.Month)
                    .ToDictionary(g => g.Key, g => g.ToList());
                
                List<MonthlyRecap> monthlyRecaps = Enumerable.Range(1, 12)
                    .Select(m => {
                        if (rawRecaps.TryGetValue(m, out List<Submission> monthlySubmissions)) {
                            return new MonthlyRecap() {
                                Month = m,

                                TimeSpent = TimeSpan.FromSeconds(
                                    monthlySubmissions.Where(
                                        s => s.TimeTaken.HasValue
                                    ).Sum(
                                        s => s.TimeTaken.Value.TotalSeconds
                                    )
                                ),

                                GoldberriesPoints = monthlySubmissions.Sum(s => s.Challenge.Difficulty.GP)
                            };
                        }
                        
                        return new MonthlyRecap() {Month = m};
                    }).ToList();

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