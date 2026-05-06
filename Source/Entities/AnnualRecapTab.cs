using System;
using System.Globalization;
using System.Linq;
using Celeste.Mod.GoldberriesIntegration.Entities.Graphs;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

public class AnnualRecapTab : Tab {

    public AnnualRecapTab(string title) : base(title) {
    }

    public override string GetTitle() {
        return base.GetTitle() + $" ({SelectedPage}/{PageAmount})";
    }

    private static AnnualRecapStat AnnualRecapStatInstance => AnnualRecapStat.Instance;
    private static MiscStat MiscStatInstance => MiscStat.Instance;

    private AnnualRecap AnnualRecap => AnnualRecapStatInstance.AnnualRecaps[SelectedPage - 1];

    private TableHelper LongestTable { get; set; }
    private TableHelper HardestTable { get; set; }
    private TableHelper MiscTable { get; set; }

    private readonly string LongestTableHeader = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_LONGEST_SUBMISSIONS");
    private readonly string HardestTableHeader = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_HARDEST_SUBMISSIONS");

    private LineChart TimeSpentLineChart { get; set; }
    private LineChart GPLineChart { get; set; }

    public override void Update() {
        PageAmount = AnnualRecapStatInstance.AnnualRecaps.Count;

        base.Update();

        // Update tables
        LongestTable = new TableHelper();
        HardestTable = new TableHelper();
        MiscTable = new TableHelper();

        float cellHeight = 30f;
        float cellTextSize = 0.3f;

        LongestTable.Insert(0, 0, 60f, cellHeight, "#", cellTextSize, color: GraphHud.HighlightTabColor);
        LongestTable.Insert(1, 0, 300f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_SUBMISSION"), cellTextSize, color: GraphHud.HighlightTabColor);
        LongestTable.Insert(2, 0, 120f, cellHeight, "", cellTextSize, color: GraphHud.HighlightTabColor);
        LongestTable.Insert(3, 0, 120f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_DATE"), cellTextSize, color: GraphHud.HighlightTabColor);

        HardestTable.Insert(0, 0, 60f, cellHeight, "#", cellTextSize, color: GraphHud.HighlightTabColor);
        HardestTable.Insert(1, 0, 300f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_SUBMISSION"), cellTextSize, color: GraphHud.HighlightTabColor);
        HardestTable.Insert(2, 0, 120f, cellHeight, "", cellTextSize, color: GraphHud.HighlightTabColor);
        HardestTable.Insert(3, 0, 120f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_DATE"), cellTextSize, color: GraphHud.HighlightTabColor);


        for (int i = 0; i < 5; i++) {
            LongestTable.Insert(0, i + 1, 60f, cellHeight, $"#{i + 1}", cellTextSize);
            HardestTable.Insert(0, i + 1, 60f, cellHeight, $"#{i + 1}", cellTextSize);

            if (i < AnnualRecap.LongestSubmissions.Count) {
                Submission submission = AnnualRecap.LongestSubmissions[i];

                LongestTable.Insert(1, i + 1, 300f, cellHeight, submission.FormattedName, cellTextSize, textColor: submission.Challenge.Difficulty.Color, outline: true);
                LongestTable.Insert(2, i + 1, 120f, cellHeight, Utils.Format(submission.TimeTaken.Value), cellTextSize);
                LongestTable.Insert(3, i + 1, 120f, cellHeight, submission.DateAchieved.ToShortDateString(), cellTextSize);
            } else {
                LongestTable.Insert(1, i + 1, 300f, cellHeight, "-", cellTextSize);
                LongestTable.Insert(2, i + 1, 120f, cellHeight, "-", cellTextSize);
                LongestTable.Insert(3, i + 1, 120f, cellHeight, "-", cellTextSize);
            }

            if (i < AnnualRecap.HardestSubmissions.Count) {
                Submission submission = AnnualRecap.HardestSubmissions[i];

                HardestTable.Insert(1, i + 1, 300f, cellHeight, submission.FormattedName, cellTextSize, textColor: submission.Challenge.Difficulty.Color, outline: true);
                HardestTable.Insert(2, i + 1, 120f, cellHeight, submission.Challenge.Difficulty.Name, cellTextSize, textColor: submission.Challenge.Difficulty.Color, outline: true);
                HardestTable.Insert(3, i + 1, 120f, cellHeight, submission.DateAchieved.ToShortDateString(), cellTextSize);
            } else {
                HardestTable.Insert(1, i + 1, 300f, cellHeight, "-", cellTextSize);
                HardestTable.Insert(2, i + 1, 120f, cellHeight, "-", cellTextSize);
                HardestTable.Insert(3, i + 1, 120f, cellHeight, "-", cellTextSize);
            }
        }

        cellHeight = 40f;
        cellTextSize = 0.4f;
        
        MiscTable.Insert(0, 0, 250f, cellHeight * 2, AnnualRecap.Year.ToString(), 1f, outline: true, color: GraphHud.HighlightTabColor, textColor: AnnualRecap.HardestSubmissions[0].Challenge.Difficulty.Color);

        MiscTable.Insert(1, 0, 250f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TOTAL_GP"), cellTextSize, color: GraphHud.HighlightTabColor);
        MiscTable.Insert(1, 1, 250f, cellHeight, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TIME_SPENT"), cellTextSize, color: GraphHud.HighlightTabColor);

        MiscTable.Insert(2, 0, 250f, cellHeight, $"{AnnualRecap.GoldberriesPoints:F2} gp ({Utils.GetPercentString(AnnualRecap.GoldberriesPoints, MiscStatInstance.TotalGoldberriesPoints)})", cellTextSize);
        MiscTable.Insert(2, 1, 250f, cellHeight, AnnualRecap.TimeSpent == TimeSpan.Zero ? "-" : $"{AnnualRecap.TimeSpent.TotalHours:F2}h ({Utils.GetPercentString(AnnualRecap.TimeSpent, MiscStatInstance.TotalTimeSpent)})", cellTextSize);

        // Update charts
        TimeSpentLineChart = new LineChart() {
            Width = 700f,
            Height = 180f,
            Header = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TIME_SPENT_GRAPH"),
            HeaderSize = 0.4f,
            LabelSize = 0.3f,
            LegendLabelSize = 0.3f,
            DataPointLabelSize = 0.3f
        };

        GPLineChart = new LineChart() {
            Width = 700f,
            Height = 180f,
            Header = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_GP_GRAPH"),
            HeaderSize = 0.4f,
            LabelSize = 0.3f,
            LegendLabelSize = 0.3f,
            DataPointLabelSize = 0.3f
        };

        for (int i = 1; i <= 12; i++) {
            string monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(i);
            TimeSpentLineChart.AddLabel(monthName);
            GPLineChart.AddLabel(monthName);
        }

        LineChart.LineSeries totalTimeSpentLine = new LineChart.LineSeries() {
            Name = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TIME_SPENT_THIS_YEAR"),
            LineColor = Color.Gold
        };
        LineChart.LineSeries monthlyTimeSpentLine = new LineChart.LineSeries() {
            Name = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MONTHLY_TIME_SPENT"),
            LineColor = Color.LightBlue
        };

        LineChart.LineSeries totalGPLine = new LineChart.LineSeries() {
            Name = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_GP_GAINED_THIS_YEAR"),
            LineColor = Color.Gold
        };
        LineChart.LineSeries monthlyGPLine = new LineChart.LineSeries() {
            Name = Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_MONTHLY_GP"),
            LineColor = Color.LightBlue
        };

        TimeSpan totalTimeSpent = TimeSpan.Zero;
        double totalGP = 0d;

        for (int i = 0; i < 12; i++) {
            MonthlyRecap mr = AnnualRecap.MonthlyRecaps[i];

            TimeSpan timeSpent = mr.TimeSpent;
            totalTimeSpent += timeSpent;

            double gp = mr.GoldberriesPoints;
            totalGP += gp;
            
            totalTimeSpentLine.Add(i, totalTimeSpent.TotalSeconds);
            totalGPLine.Add(i, totalGP);

            monthlyTimeSpentLine.Add(i, timeSpent.TotalSeconds);
            monthlyGPLine.Add(i, gp);
        }

        double timeSpentChartYAxisSpacing = totalTimeSpent.TotalHours / 4;
        double gpChartYAxisSpacing = totalGP / 4;

        for (int i = 0; i <= 4; i++) {
            TimeSpentLineChart.AddYAxisLabel($"{timeSpentChartYAxisSpacing * i:F2}h");
            GPLineChart.AddYAxisLabel($"{gpChartYAxisSpacing * i:F2} gp");
        }

        monthlyTimeSpentLine.MaxValue = totalTimeSpentLine.MaxValue;
        monthlyGPLine.MaxValue = totalGPLine.MaxValue;

        TimeSpentLineChart.Add(monthlyTimeSpentLine);
        TimeSpentLineChart.Add(totalTimeSpentLine);

        GPLineChart.Add(monthlyGPLine);
        GPLineChart.Add(totalGPLine);
    }

    public override void Render() {
        Vector2 pointer = GraphHud.TabPosition;
        pointer.Move(30f, 30f);
        MiscTable.DrawTable(pointer);

        // Render tables
        pointer.Move(0f, 150f);
        ActiveFont.Draw(
            LongestTableHeader,
            pointer.MoveCopy(300f, -10f),
            new Vector2(0.5f, 1f),
            Vector2.One * 0.4f,
            Color.Black
        );

        LongestTable.DrawTable(pointer);
        pointer.Move(0f, 180f);
        TimeSpentLineChart.Render(pointer.MoveCopy(750f, 0f));
        pointer.Move(0f, 100f);

        ActiveFont.Draw(
            HardestTableHeader,
            pointer.MoveCopy(300f, -10f),
            new Vector2(0.5f, 1f),
            Vector2.One * 0.4f,
            Color.Black
        );

        HardestTable.DrawTable(pointer);

        GPLineChart.Render(pointer.MoveCopy(750f, 180f));
    }

}