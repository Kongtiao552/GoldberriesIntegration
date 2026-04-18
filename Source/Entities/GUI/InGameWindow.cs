using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI;

public abstract class InGameWindow : Entity {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    public static readonly float WindowPositionX = Engine.Width * 0.1f;
    public static readonly float WindowPositionY = Engine.Height * 0.1f;
    public static readonly float WindowWidth = Engine.Width * 0.8f;
    public static readonly float WindowHeight = Engine.Height * 0.8f;
    public static readonly float OutlineThickness = 3f;

    public static readonly Vector2 WindowPosition = new Vector2(WindowPositionX, WindowPositionY);
    public static readonly Vector2 WindowSize = new Vector2(WindowWidth, WindowHeight);

    public virtual Color WindowColor { get; set; } = Color.Gray;

    public static readonly int WindowDepth = -105;

    public InGameWindow() {
        Depth = WindowDepth;
        Tag = Tags.Global | Tags.HUD | Tags.FrozenUpdate | Tags.PauseUpdate | Tags.TransitionUpdate;
        Position = WindowPosition;
        Visible = false;
        Active = false;
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
    
}