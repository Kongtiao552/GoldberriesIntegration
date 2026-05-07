using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesStat.Misc;
using Celeste.Mod.GoldberriesStat.Models;
using Celeste.Mod.GoldberriesStat.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesStat.Entities;

public class MilestonesTab : Tab {

    public MilestonesTab(string title) : base(title) {
        PageAmount = (int) Math.Ceiling(StatManager.TierAmount / 8f);
    }

    public override string GetTitle() {
        return base.GetTitle() + $" ({SelectedPage}/{PageAmount})";
    }

    public class Milestone {

        private static readonly string FirstCompletedLabel = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MILESTONE_FTRST_COMPLETED");
        private static readonly string FastestLabel = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MILESTONE_FASTEST");
        private static readonly string LongestLabel = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MILESTONE_LONGEST");

        private static readonly float LabelSize = 0.4f;

        private static readonly float LineHeight = 30f;

        private static readonly int SubmissionStringMaxLength = 30;

        private static string GetStatString(Submission submission) {        
            return submission.TimeTaken.HasValue ? $"({submission.DateAchieved:d}, {submission.TimeTaken.Value.TotalHours:F2}h)" : $"({submission.DateAchieved:d})";
        }

        private static string GetTruncatedName(Submission submission) {
            string name = submission.FormattedName;

            if (name.Length > SubmissionStringMaxLength) {
                name = name.Substring(0, SubmissionStringMaxLength) + " ...";
            }

            return name;
        }

        public Milestone(GoldenTier goldenTier) {
            GoldenTier = goldenTier;

            if (GoldenTier.FirstCompleted != null) {
                TruncatedFirstCompletedName = GetTruncatedName(GoldenTier.FirstCompleted);
                FirstCompletedStat = GetStatString(GoldenTier.FirstCompleted);
            }

            if (GoldenTier.Fastest != null) {
                TruncatedFastestName = GetTruncatedName(GoldenTier.Fastest);
                FastestStat = GetStatString(GoldenTier.Fastest);
            }

            if (GoldenTier.Longest != null) {
                TruncatedLongestName = GetTruncatedName(GoldenTier.Longest);
                LongestStat = GetStatString(GoldenTier.Longest);
            }
        }

        public void Render(Vector2 pointer) {
            Color color = GoldenTier.Color;

            ActiveFont.DrawOutline(
                GoldenTier.TierString,
                pointer.MoveCopy(MilestoneWidth / 2, 40f), 
                Vector2.One / 2,
                Vector2.One * 0.8f,
                color,
                1f,
                Color.Black
            );

            pointer.Move(10f, 90f);

            ActiveFont.Draw(FastestLabel, pointer, Vector2.Zero, Vector2.One * LabelSize, Color.Black);
            pointer.MoveY(LineHeight);

            if (GoldenTier.Fastest != null) {
                ActiveFont.DrawOutline(TruncatedFastestName, pointer, Vector2.Zero, Vector2.One * LabelSize, GoldenTier.Color, 1f, Color.Black);
                pointer.MoveY(LineHeight);
                ActiveFont.DrawOutline(FastestStat, pointer, Vector2.Zero, Vector2.One * LabelSize, GoldenTier.Color, 1f, Color.Black);
                pointer.MoveY(LineHeight);
            } else {
                ActiveFont.Draw("-", pointer, Vector2.Zero, Vector2.One * LabelSize, Color.Black);
                pointer.MoveY(LineHeight);
            }

            ActiveFont.Draw(LongestLabel, pointer, Vector2.Zero, Vector2.One * LabelSize, Color.Black);
            pointer.MoveY(ActiveFont.Measure(LongestLabel).Y * LabelSize);

            if (GoldenTier.Longest != null) {
                ActiveFont.DrawOutline(TruncatedLongestName, pointer, Vector2.Zero, Vector2.One * LabelSize, GoldenTier.Color, 1f, Color.Black);
                pointer.MoveY(LineHeight);
                ActiveFont.DrawOutline(LongestStat, pointer, Vector2.Zero, Vector2.One * LabelSize, GoldenTier.Color, 1f, Color.Black);
                pointer.MoveY(LineHeight);
            } else {
                ActiveFont.Draw("-", pointer, Vector2.Zero, Vector2.One * LabelSize, Color.Black);
                pointer.MoveY(LineHeight);
            }

            ActiveFont.Draw(FirstCompletedLabel, pointer, Vector2.Zero, Vector2.One * LabelSize, Color.Black);
            pointer.MoveY(ActiveFont.Measure(FirstCompletedLabel).Y * LabelSize);

            if (GoldenTier.FirstCompleted != null) {
                ActiveFont.DrawOutline(TruncatedFirstCompletedName, pointer, Vector2.Zero, Vector2.One * LabelSize, GoldenTier.Color, 1f, Color.Black);
                pointer.MoveY(LineHeight);
                ActiveFont.DrawOutline(FirstCompletedStat, pointer, Vector2.Zero, Vector2.One * LabelSize, GoldenTier.Color, 1f, Color.Black);
            } else {
                ActiveFont.Draw("-", pointer, Vector2.Zero, Vector2.One * LabelSize, Color.Black);
            }
        }

        private GoldenTier GoldenTier { get; set; }

        private string TruncatedFastestName { get; set; }
        private string TruncatedLongestName { get; set; }
        private string TruncatedFirstCompletedName { get; set; }

        private string FastestStat { get; set; }
        private string LongestStat { get; set; }
        private string FirstCompletedStat { get; set; }
    }

    public List<Milestone> Milestones { get; set; }

    private static GoldenTierStat GoldenTierStatInstance => GoldenTierStat.Instance;

    private static readonly float MilestoneWidth = GraphHud.HudWidth / 4;

    public override void Update() {
        Milestones = new List<Milestone>();
        for (int i = (SelectedPage - 1) * 8; i < SelectedPage * 8 && i < StatManager.TierAmount; i++) {
            Milestones.Add(new Milestone(GoldenTierStatInstance.GoldenTiers[i]));
        }
    }

    public override void Render() {
        Vector2 pointer = GraphHud.TabPosition;

        Draw.Rect(new Vector2(GraphHud.HudPositionY, pointer.Y + GraphHud.TabHeight / 2), GraphHud.HudWidth, 2f, Color.Black);

        for (int i = 1; i < 4; i++) {
            pointer.X += MilestoneWidth;
            Draw.Rect(pointer, 2f, GraphHud.TabHeight, Color.Black);
        }

        pointer = GraphHud.TabPosition;

        for (int i = 0; i < 4; i++) {
            if (i >= Milestones.Count) return;

            Milestones[i].Render(pointer);
            pointer.MoveX(MilestoneWidth);
        }

        pointer = GraphHud.TabPosition;
        pointer.Y += GraphHud.TabHeight / 2;

        for (int i = 4; i < 8; i++) {
            if (i >= Milestones.Count) return;

            Milestones[i].Render(pointer);
            pointer.X += MilestoneWidth;
        }
    }

}