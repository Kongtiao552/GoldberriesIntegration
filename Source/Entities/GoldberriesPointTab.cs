using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Entities.Graphs;
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
    private static MiscStat MiscStatInstance => MiscStat.Instance;

    private string TotalLabel { get; set; } = "";

    private BarChart BarChart { get; set; }

    public override void Render() {
        BarChart.RenderVerticallyCentered(GraphHud.TabPosition.MoveCopy(0f, GraphHud.TabHeight / 2));

        ActiveFont.Draw(
            TotalLabel,
            GraphHud.TabRightBottom.MoveCopy(-20f, -10f),
            Vector2.One, 
            Vector2.One * 0.6f, 
            Color.Black
        );
    }

    public override void Update() {
        BarChart = new BarChart() {
            LabelWidth = 120f,
            BarHeight = 30f,
            MaxBarWidth = 1000f,
            TextSize = 0.4f
        };
        
        for (int i = StatManager.TierCount - 1; i >= 0; i--) {
            GoldenTier goldenTier = GoldenTierStatInstance.GoldenTiers[i];
            BarChart.Add(
                goldenTier.GoldberriesPoints, 
                goldenTier.Color, 
                goldenTier.TierString,
                $"{goldenTier.GoldberriesPoints:F2} gp ({Utils.GetPercentString(goldenTier.GoldberriesPoints, MiscStatInstance.TotalGoldberriesPoints)})"
            );
        }

        TotalLabel = $"{TotalString}: {MiscStatInstance.TotalGoldberriesPoints:F2} gp";
    }

}