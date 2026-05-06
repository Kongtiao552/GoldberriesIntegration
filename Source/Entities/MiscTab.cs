using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

public class MiscTab : Tab {

    public MiscTab(string title) : base(title) {
    }

    private readonly static int TableRowAmount = 20;

    // Used for calculating page amount
    private readonly static decimal TableRowAmountDecimal = 20m;

    private static MiscStat MiscStatInstance => MiscStat.Instance;

    private TableHelper SubmissionTable { get; set; }
    private TableHelper VerifierTable { get; set; }

    private readonly string SubmissionTableHeader = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_SUBMISSIONS_BY_TIME_SPENT");
    private readonly string VerifierTableHeader = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_VERIFIERS_BY_SUBMISSIONS_VERIFIED");

    public override string GetTitle() {
        return base.GetTitle() + $" ({SelectedPage}/{PageAmount})";
    }

    public override void Update() {
        List<Submission> submissionsByTime = MiscStatInstance.SubmissionsByTime;
        List<Tuple<string, string, int>> verifiers = MiscStatInstance.Verifiers;

        int submissionTablePageAmount = (int) Math.Ceiling(submissionsByTime.Count / TableRowAmountDecimal);
        int verifierTablePageAmount = (int) Math.Ceiling(verifiers.Count / TableRowAmountDecimal);

        int submissionTableCurrentPage = Math.Max(Math.Min(SelectedPage, submissionTablePageAmount), 1);
        int verifierTableCurrentPage = Math.Max(Math.Min(SelectedPage, verifierTablePageAmount), 1);

        PageAmount = Math.Max(submissionTablePageAmount, verifierTablePageAmount);

        base.Update();

        float cellHeight = 30f;
        float cellTextSize = 0.3f;

        SubmissionTable = new TableHelper();

        SubmissionTable.Insert(0, 0, 60f, cellHeight, "#", cellTextSize, color: GraphHud.HighlightTabColor);
        SubmissionTable.Insert(1, 0, 300f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_SUBMISSION"), cellTextSize, color: GraphHud.HighlightTabColor);
        SubmissionTable.Insert(2, 0, 120f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TIME_SPENT"), cellTextSize, color: GraphHud.HighlightTabColor);
        SubmissionTable.Insert(3, 0, 120f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_DIFFICULTY"), cellTextSize, color: GraphHud.HighlightTabColor);
        SubmissionTable.Insert(4, 0, 120f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_DATE"), cellTextSize, color: GraphHud.HighlightTabColor);

        int offset = (submissionTableCurrentPage - 1) * 20;

        for (int i = 1; i <= TableRowAmount; i++) {
            int index = offset + i - 1;

            if (index < submissionsByTime.Count) {
                Submission submission = submissionsByTime[index];

                SubmissionTable.Insert(0, i, 60f, cellHeight, $"#{offset + i}", cellTextSize);
                SubmissionTable.Insert(1, i, 300f, cellHeight, submission.FormattedName, cellTextSize, textColor: submission.Challenge.Difficulty.Color, outline: true);
                SubmissionTable.Insert(2, i, 120f, cellHeight, Utils.Format(submission.TimeTaken.Value), cellTextSize);
                SubmissionTable.Insert(3, i, 120f, cellHeight, submission.Challenge.Difficulty.Name, cellTextSize, textColor: submission.Challenge.Difficulty.Color, outline: true);
                SubmissionTable.Insert(4, i, 120f, cellHeight, submission.DateAchieved.ToShortDateString(), cellTextSize);
            } else {
                break;
            }      
        }

        VerifierTable = new TableHelper();

        VerifierTable.Insert(0, 0, 60f, cellHeight, "#", cellTextSize, color: GraphHud.HighlightTabColor);
        VerifierTable.Insert(1, 0, 200f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_VERIFIER"), cellTextSize, color: GraphHud.HighlightTabColor);
        VerifierTable.Insert(2, 0, 200f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_SUBMISSIONS_VERIFIED"), cellTextSize, color: GraphHud.HighlightTabColor);

        offset = (verifierTableCurrentPage - 1) * 20;

        for (int i = 1; i <= TableRowAmount; i++) {
            int index = offset + i - 1;

            if (index < verifiers.Count) {
                Tuple<string, string, int> verifier = verifiers[index];

                VerifierTable.Insert(0, i, 60f, cellHeight, $"#{offset + i}", cellTextSize);

                Color color = verifier.Item2 == null ? Color.Black : Calc.HexToColor(verifier.Item2);
                bool textOutline = color != Color.Black;

                VerifierTable.Insert(1, i, 200f, cellHeight, verifier.Item1, cellTextSize, textColor: color, outline: textOutline);
                VerifierTable.Insert(2, i, 200f, cellHeight, $"{verifier.Item3} ({Utils.GetPercentString((float) verifier.Item3, StatManager.Submissions.Count)})", cellTextSize);
            } else {
                break;
            }
        }
    }

    public override void Render() {
        Draw.Rect(Engine.Width / 2f, GraphHud.TabPosition.Y, 2f, GraphHud.TabHeight, Color.Black);

        Vector2 pointer = GraphHud.TabPosition;
        pointer.Move(20f, 50f);
        ActiveFont.Draw(
            SubmissionTableHeader,
            pointer,
            Vector2.UnitY,
            Vector2.One * 0.4f,
            Color.Black
        );
        SubmissionTable.DrawTable(pointer);

        pointer.Move(770f, 0f);
        ActiveFont.Draw(
            VerifierTableHeader,
            pointer,
            Vector2.UnitY,
            Vector2.One * 0.4f,
            Color.Black
        );
        VerifierTable.DrawTable(pointer);
    }

}