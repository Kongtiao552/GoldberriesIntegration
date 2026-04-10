using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesIntegration.Menu;

public class ColoredButton : TextMenu.Button {

    public Color HighlightedColor { get; set; }

    public ColoredButton(string label, Color highlightedColor) : base(label) {
        HighlightedColor = highlightedColor;
    }

    public override void Render(Vector2 position, bool highlighted) {
        float alpha = Container.Alpha;
        Color color = Disabled ? Color.DarkSlateGray : ((highlighted ? HighlightedColor : Color.White) * alpha);
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        bool flag = Container.InnerContent == TextMenu.InnerContentMode.TwoColumn && !AlwaysCenter;
        Vector2 position2 = position + (flag ? Vector2.Zero : new Vector2(Container.Width * 0.5f, 0f));
        Vector2 justify = (flag && !AlwaysCenter) ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
        ActiveFont.DrawOutline(Label, position2, justify, Vector2.One, color, 2f, strokeColor);
    }
}