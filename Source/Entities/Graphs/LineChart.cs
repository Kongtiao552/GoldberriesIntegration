using System.Collections.Generic;
using Celeste.Mod.GoldberriesStat.Misc;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesStat.Entities.Graphs;

public class LineChart {

    public float Width { get; set; }
    public float Height { get; set; }

    public float LabelSize { get; set; }
    public float LegendLabelSize { get; set; }
    public float DataPointLabelSize { get; set; }

    public string Header { get; set; }

    public float HeaderSize { get; set; }

    private List<string> Labels { get; set; } = new List<string>();

    public void AddLabel(string label) {
        Labels.Add(label);
    } 

    private List<string> YAxisLabels { get; set; } = new List<string>();

    public void AddYAxisLabel(string label) {
        YAxisLabels.Add(label);
    }
    
    public class LineSeries {
        
        public string Name { get; set; }
        public Color LineColor { get; set; }

        public List<DataPoint> DataPoints { get; private set; } = new List<DataPoint>();

        public void Add(int index, double value) {
            DataPoints.Add(new DataPoint() {
                Index = index,
                Value = value
            });

            if (value > MaxValue) {
                MaxValue = value;
            }
        }

        public double MaxValue { get; set; }

    }

    public class DataPoint {
        
        public int Index { get; set; }
        public double Value { get; set; }

        public Vector2 ChartOffset { get; set; }

    }

    private List<LineSeries> Lines { get; set; } = new List<LineSeries>();

    public void Add(LineSeries line) {
        float labelSpacing = Width / (Labels.Count - 1);

        for (int i = 0; i < line.DataPoints.Count; i++) {
            DataPoint dataPoint = line.DataPoints[i];
            dataPoint.ChartOffset = new Vector2(labelSpacing * dataPoint.Index, (float) (dataPoint.Value / line.MaxValue) * -Height);
        }

        Lines.Add(line);
    }

    public void Render(Vector2 pointer) {
        Draw.Rect(pointer, Width, -Height, Color.Gray * 0.1f);

        RenderGridLines(pointer);
        RenderXAxis(pointer);
        RenderYAxis(pointer);
        RenderLines(pointer);
        RenderLegend(pointer);

        ActiveFont.Draw(Header, pointer.MoveCopy(0f, -Height), Vector2.UnitY, Vector2.One * HeaderSize, Color.Black);
    }

    private void RenderXAxis(Vector2 pointer) {
        Draw.Rect(pointer, Width, 3f, Color.Gray * 0.2f);

        float labelSpacing = Width / (Labels.Count - 1);

        foreach (string label in Labels) {
            Draw.Rect(pointer, 3f, 10f, Color.Gray * 0.2f);
            ActiveFont.Draw(label, pointer.MoveCopy(0f, 10f), Vector2.UnitX / 2, Vector2.One * LabelSize, Color.Gray);
            pointer.X += labelSpacing;
        }
    }

    private void RenderYAxis(Vector2 pointer) {
        Draw.Rect(pointer, 3f, -Height, Color.Gray * 0.2f);

        float labelSpacing = Height / (YAxisLabels.Count - 1);

        foreach (string label in YAxisLabels) {
            Draw.Rect(pointer, -10f, 3f, Color.Gray * 0.2f);
            ActiveFont.Draw(label, pointer.MoveCopy(-15f, 0f), new Vector2(1f, 0.5f), Vector2.One * LabelSize, Color.Gray);
            pointer.Y -= labelSpacing;
        }
    }

    private void RenderGridLines(Vector2 pointer) {
        float labelSpacing = Height / (YAxisLabels.Count - 1);

        for (int i = 1; i < YAxisLabels.Count; i++) {
            pointer.Y -= labelSpacing;
            Draw.Rect(pointer, Width, 3f, Color.Gray * 0.2f);      
        }
    }


    private void RenderLines(Vector2 pointer) {
        foreach (LineSeries line in Lines) {
            List<DataPoint> dataPoints = line.DataPoints;

            for (int i = 1; i < dataPoints.Count; i++) {
                Draw.Line(pointer + dataPoints[i - 1].ChartOffset, pointer + dataPoints[i].ChartOffset, line.LineColor, 3f);
            }
        }
    }

    private void RenderLegend(Vector2 pointer) {
        Vector2 legendPos = pointer.MoveCopy(Width, -Height - 10f);

        foreach (LineSeries line in Lines) {
            Draw.Rect(legendPos, -20f, -20f, line.LineColor);
            ActiveFont.Draw(line.Name, legendPos.MoveCopy(-30f, -10f), new Vector2(1f, 0.5f), Vector2.One * LegendLabelSize, Color.Black);
            legendPos.Move(0f, -25f);
        }
    }
    
}