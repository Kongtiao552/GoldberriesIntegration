using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

public class MilestonesTab : PageTab {

    public MilestonesTab(string title) : base(title) {
        PageAmount = 3;
    }

    private static GoldenTierStat GoldenTierStatInstance => GoldenTierStat.Instance;

    private readonly float MilestoneWidth = InGameWindow.WindowWidth / 4;

    private readonly string FirstAchievedLabel = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MILESTONE_FTRST_ACHIEVED");
    private readonly string FastestLabel = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MILESTONE_FASTEST");
    private readonly string LongestLabel = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MILESTONE_LONGEST");

    private float TableLabelSize = 0.4f;
    private float TableRowHeight = 30f;

    private int SubmissionStringMaxLength = 30;

    private Vector2 DrawSubmission(Vector2 pointer, Submission submission) {
        if (submission == null) {
            ActiveFont.Draw("-", pointer, Vector2.Zero, Vector2.One * TableLabelSize, Color.Black);
            pointer.Y += TableRowHeight;
            return pointer;
        }

        Color color = GoldberriesStatsManager.TierColors[submission.Challenge.Difficulty.Name];

        string submissionString = submission.ToString();

        if (submissionString.Length > SubmissionStringMaxLength) {
            submissionString = submissionString.Substring(0, SubmissionStringMaxLength) + " ...";
        }

        string statString;
        
        if (!submission.TimeTaken.HasValue) {
            statString = $"({submission.DateAchieved:d})";
        } else {
            statString = $"({submission.DateAchieved:d}, {submission.TimeTaken.Value.TotalHours:F2}h)";
        }

        ActiveFont.DrawOutline(
            submissionString, 
            pointer,
            Vector2.Zero,
            Vector2.One * TableLabelSize,
            color,
            1f,
            Color.Black
        );
        pointer.Y += TableRowHeight;
        ActiveFont.DrawOutline(
            statString,
            pointer,
            Vector2.Zero,
            Vector2.One * TableLabelSize,
            color,
            1f,
            Color.Black
        );

        pointer.Y += TableRowHeight;
        return pointer;
    }

    private void DrawMilestone(Vector2 pointer, GoldenTier goldenTier) {   
        Vector2 offset = new Vector2(MilestoneWidth / 2, 40f);
        Color color = goldenTier.GetColor();
        InGameWindow.DrawTextInCenter(goldenTier.GetTierString(), 0.8f, pointer + offset, color, true);
        
        pointer += new Vector2(20f, GBStatsHUD.TabContentHeight / 2 - 40f);
        pointer.Y -= TableRowHeight * 8;

        ActiveFont.Draw(FastestLabel, pointer, Vector2.Zero, Vector2.One * TableLabelSize, Color.Black);
        pointer.Y += TableRowHeight;
        pointer = DrawSubmission(pointer, goldenTier.Fastest);

        ActiveFont.Draw(LongestLabel, pointer, Vector2.Zero, Vector2.One * TableLabelSize, Color.Black);
        pointer.Y += TableRowHeight;
        pointer = DrawSubmission(pointer, goldenTier.Longest);
        
        ActiveFont.Draw(FirstAchievedLabel, pointer, Vector2.Zero, Vector2.One * TableLabelSize, Color.Black);
        pointer.Y += TableRowHeight;
        DrawSubmission(pointer, goldenTier.FirstAchieved);
    }

    public override void Render() {
        Vector2 pointer = GBStatsHUD.TabPosition;

        Draw.Rect(new Vector2(InGameWindow.WindowPositionX, pointer.Y + GBStatsHUD.TabContentHeight / 2), InGameWindow.WindowWidth, 2f, Color.Black);

        for (int i = 1; i < 4; i++) {
            pointer.X += MilestoneWidth;
            Draw.Rect(pointer, 2f, GBStatsHUD.TabContentHeight, Color.Black);
        }

        pointer = GBStatsHUD.TabPosition;

        int LowestTier = (CurrentPage - 1) * 8 + 1;

        for (int i = LowestTier; i < LowestTier + 4; i++) {
            if (i > GoldberriesStatsManager.TierCount) return;
            DrawMilestone(pointer, GoldenTierStatInstance.GoldenTiers[i - 1]);
            pointer.X += MilestoneWidth;
        }

        pointer = GBStatsHUD.TabPosition;
        pointer.Y += GBStatsHUD.TabContentHeight / 2;

        for (int i = LowestTier + 4; i < LowestTier + 8; i++) {
            if (i > GoldberriesStatsManager.TierCount) return;
            DrawMilestone(pointer, GoldenTierStatInstance.GoldenTiers[i - 1]);
            pointer.X += MilestoneWidth;
        }
    }

}