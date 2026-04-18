using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Celeste.Mod.GoldberriesIntegration.Stats;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

public class TimeSpentTab : Tab {

    public TimeSpentTab(string title) : base(title) {
        
    }

    private static GoldenTierStat GoldenTierStatInstance => GoldenTierStat.Instance;

    public override void Render() {
        ChartHelper.GBGoldenTierBarChart(
            tier => $"{tier.TimeSpent.TotalHours:F2}h",
            tier => tier.TimeSpent.TotalMinutes,
            GoldenTierStatInstance.MaxTimeSpent.TotalMinutes,
            GoldenTierStatInstance.TotalTimeSpent.TotalMinutes,
            $"{GoldenTierStatInstance.TotalTimeSpent.TotalHours:F2}h"
        );
    }
}