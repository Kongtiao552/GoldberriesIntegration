using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

public class GoldberriesPointTab : Tab {

    private static string TotalString = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TOTAL_STRING");

    public GoldberriesPointTab(string title) : base(title) {
        
    }

    private static GoldenTierStat GoldenTierStatInstance => GoldenTierStat.Instance;

    public override void Render() {
        ChartHelper.GBGoldenTierBarChart(
            tier => $"{tier.GoldberriesPoints:F2} gp",
            tier => tier.GoldberriesPoints,
            GoldenTierStatInstance.MaxGoldberriesPoints,
            GoldenTierStatInstance.TotalGoldberriesPoints
        );

        ActiveFont.Draw(
            $"{TotalString}: {GoldenTierStatInstance.TotalGoldberriesPoints:F2} gp",
            GBStatsHUD.HUDPosition + new Vector2(GBStatsHUD.HUDWidth - 20f, GBStatsHUD.HUDHeight - 10f),
            Vector2.One, 
            Vector2.One * 0.6f, 
            Color.Black
        );
    }
}