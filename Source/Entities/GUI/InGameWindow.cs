using System;
using System.Collections.Generic;
using System.Linq;
using Celeste;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI;

public class InGameWindow : Entity {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    public static readonly float WindowPositionX = Engine.Width * 0.1f;
    public static readonly float WindowPositionY = Engine.Height * 0.1f;
    public static readonly float WindowWidth = Engine.Width * 0.8f;
    public static readonly float WindowHeight = Engine.Height * 0.8f;
    public static readonly float OutlineThickness = 3f;

    public static readonly Vector2 WindowPosition = new Vector2(WindowPositionX, WindowPositionY);
    public static readonly Vector2 WindowSize = new Vector2(WindowWidth, WindowHeight);

    public Color WindowColor { get; set; } = Color.Gray;

    public static readonly int WindowDepth = -105;

    public InGameWindow() {
        Depth = WindowDepth;
        Tag = Tags.Global | Tags.HUD | Tags.FrozenUpdate | Tags.PauseUpdate | Tags.TransitionUpdate;
        Position = WindowPosition;
        Visible = false;
        Active = false;

        WindowList.Add(this);
    }

    public override void Update() {
        base.Update();

        if (Input.MenuCancel.Pressed || Input.ESC.Pressed || ModSettings.ButtonViewCharts.Pressed) {
            Hide();

            Input.ESC.ConsumePress();
            ModSettings.ButtonViewCharts.ConsumePress();
        }
    }

    public override void Render() {
        base.Render();

        Draw.Rect(Position, WindowSize.X, WindowSize.Y, WindowColor);
    }

    public void Show() {
        HideAllWindows();

        Visible = true;
        Active = true;

        if (Engine.Scene is Level level) {
            level.Paused = true;
        }
    }
    public void Hide() {
        Visible = false;
        Active = false;

        if (Engine.Scene is Level level) {
            level.Paused = false;
        }
    }

    public void Toggle() {
        if (Visible) Hide();
        else Show();
    }

    public static List<InGameWindow> WindowList= new List<InGameWindow>();

    public static void HideAllWindows() => WindowList.ForEach(window => window.Hide());

    public static void DrawText(string text, float size, Vector2 position, Color color, bool outline) {
        text = text.DialogCleanOrNull() ?? text;

        if (outline) {
            ActiveFont.DrawOutline(text, position, Vector2.Zero, Vector2.One * size, color, 1f, Color.Black);
        } else {
            ActiveFont.Draw(text, position, Vector2.Zero, Vector2.One * size, color);
        }
    }

    public static void DrawTextInCenter(string text, float size, Vector2 position, Color color, bool outline) {
        text = text.DialogCleanOrNull() ?? text;

        position -= ActiveFont.Measure(text) / 2 * size;
        DrawText(text, size, position, color, outline);
    }

    public static void DrawTextInScreenCenter(string text, float size, Color color, bool outline) {
        DrawTextInCenter(text, size, Utils.ScreenCenter, color, outline);
    }
}