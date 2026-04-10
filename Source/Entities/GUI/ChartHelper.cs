using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI;

public static class ChartHelper {

    public static string TotalString = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TOTAL_STRING");

    public delegate string LabelFormatter(GoldenTier tier);
    public delegate double ValueGetter(GoldenTier tier);
    
    public static void GBGoldenTierBarChart(LabelFormatter labelFormatter, ValueGetter valueGetter, double max, double total, string totalString) {
        Vector2 pointer = GoldberriesGUI.TabPosition + new Vector2(120f, 20f);
        GoldenTierStat stat = GoldenTierStat.Instance;

        float textSize = 0.4f;
        int barHeight = 30;
        int height = barHeight * GoldberriesStatsManager.TierCount;
        float barMaxWidth = InGameWindow.WindowWidth - 350f;
        
        totalString = TotalString + ": " + totalString;
        ActiveFont.Draw(totalString, GoldberriesGUI.TabPosition + new Vector2(InGameWindow.WindowWidth - 20f, InGameWindow.WindowHeight - 200f), Vector2.UnitX, Vector2.One * 0.6f, Color.Black);

        pointer.Y += height;
        Draw.Rect(pointer, 1f, -height, Color.Black);

        pointer += Vector2.UnitX;

        foreach (GoldenTier goldenTier in stat.GoldenTiers) {
            double value = valueGetter(goldenTier);
            string statString = labelFormatter(goldenTier);
            string tierString = goldenTier.GetTierString();
            Color color = goldenTier.GetColor();

            InGameWindow.DrawTextInCenter(tierString, textSize, pointer - new Vector2(60f, barHeight / 2), color, true);

            if (value != 0d) {
                double proportion = value / max;
                float barWidth = (float) (proportion * barMaxWidth);

                Draw.Rect(pointer, barWidth, -barHeight, color); 
                Draw.Rect(pointer + new Vector2(barWidth, 0f), 1f, -barHeight, Color.Black);
                Draw.Rect(pointer, barWidth + 1f, 1f, Color.Black);
                Draw.Rect(pointer - new Vector2(0f, barHeight), barWidth + 1f, 1f, Color.Black);

                statString += $" ({(value / total).ToString("P2").Replace(" ", "")})";
                Vector2 measure = ActiveFont.Measure(statString) * textSize;
                ActiveFont.DrawOutline(statString, pointer + new Vector2(barWidth + 10f, -barHeight / 2) - new Vector2(0f, measure.Y / 2), Vector2.Zero, Vector2.One * textSize, color, 1f, Color.Black);
            }
            
            pointer.Y -= barHeight;
        }
    }

}