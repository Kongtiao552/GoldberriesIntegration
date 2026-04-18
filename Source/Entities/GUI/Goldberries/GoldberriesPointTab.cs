using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Celeste.Mod.GoldberriesIntegration.Stats;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

public class GoldberriesPointTab : Tab {

    public GoldberriesPointTab(string title) : base(title) {
        
    }

    private static GoldenTierStat GoldenTierStatInstance => GoldenTierStat.Instance;

    public override void Render() {
        ChartHelper.GBGoldenTierBarChart(
            tier => $"{tier.GoldberriesPoints:F2} gp",
            tier => tier.GoldberriesPoints,
            GoldenTierStatInstance.MaxGoldberriesPoints,
            GoldenTierStatInstance.TotalGoldberriesPoints,
            $"{GoldenTierStatInstance.TotalGoldberriesPoints:F2} gp"
        );
    }
}