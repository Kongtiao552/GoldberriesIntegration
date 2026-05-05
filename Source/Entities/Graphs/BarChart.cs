using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities.Graphs;

public class BarChart {
    
    public float LabelWidth { get; set; }
    public float BarHeight { get; set; }
    public float MaxBarWidth { get; set; }
    public float TextSize { get; set; }

    public class BarItem {
        
        public double Value { get; set; }
        public Color Color { get; set; }
        public string Label { get; set; }
        public string ValueLabel { get; set; }

    }

    private List<BarItem> Items { get; set; } = new List<BarItem>();

    private double MaxValue { get; set; } = 0d;

    public void Add(double value, Color color, string label, string valueLabel) {
        Items.Add(new BarItem() {
            Value = value,
            Color = color,
            Label = label,
            ValueLabel = valueLabel
        });

        if (value > MaxValue) {
            MaxValue = value;
        }
    }

    public void Render(Vector2 pointer) {
        foreach (BarItem item in Items) {
            ActiveFont.DrawOutline(item.Label, pointer.MoveCopy(LabelWidth / 2, BarHeight / 2), Vector2.One / 2, Vector2.One * TextSize, item.Color, 1f, Color.Black);

            Vector2 pointer2 = pointer.MoveCopy(LabelWidth, 0f);
            
            if (item.Value != 0d) {
                float width = (float) (item.Value / MaxValue) * MaxBarWidth;

                Draw.Rect(pointer2, width, BarHeight, item.Color);

                // Draw outlines
                Draw.Rect(pointer2, width + 1f, 1f, Color.Black);
                Draw.Rect(pointer2.MoveCopy(0f, BarHeight), width + 1f, 1f, Color.Black);
                Draw.Rect(pointer2.MoveCopy(width, 0f), 1f, BarHeight, Color.Black);

                ActiveFont.DrawOutline(item.ValueLabel, pointer2.MoveCopy(width + 10f, BarHeight / 2), Vector2.UnitY / 2, Vector2.One * TextSize, item.Color, 1f, Color.Black);
            }

            Draw.Rect(pointer2, 1f, BarHeight, Color.Black);

            pointer.Y += BarHeight;
        }
    }

    public void RenderVerticallyCentered(Vector2 pointer) {
        Render(pointer.MoveCopy(0f, -BarHeight * Items.Count / 2));
    }
     
}