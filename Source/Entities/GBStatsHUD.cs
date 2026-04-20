using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

[Tracked]
public class GBStatsHUD : Entity {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    public static readonly float HUDPositionX = Engine.Width * 0.1f;
    public static readonly float HUDPositionY = Engine.Height * 0.1f;
    public static readonly float HUDWidth = Engine.Width * 0.8f;
    public static readonly float HUDHeight = Engine.Height * 0.8f;

    public static readonly Vector2 HUDPosition = new Vector2(HUDPositionX, HUDPositionY);
    public static readonly Vector2 HUDSize = new Vector2(HUDWidth, HUDHeight);

    public void Show() {
        Visible = true;

        if (Engine.Scene is Level level) {
            level.Paused = true;
        }
    }

    public void Hide() {
        Visible = false;

        if (Engine.Scene is Level level) {
            level.Paused = false;
        }
    }

    public void Toggle() {
        if (Visible) {
            Hide();
        } else {
            Show();
        }
    }
    public Color HUDColor { get; set; } = new Color(255, 242, 204);

    public static readonly Color HighlightTabColor = new Color(255, 217, 102);

    public List<Tab> TabList { get; set; } = new List<Tab>();

    public int SelectedTab { get; set; } = 0;

    public int TabCount => TabList.Count;

    public GBStatsHUD() {
        Depth = -10000;
        Tag = Tags.Global | Tags.HUD | Tags.PauseUpdate | Tags.TransitionUpdate;
        Position = HUDPosition;
        Visible = false;
        Active = true;

        TabList.Add(new TimeSpentTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB1"));
        TabList.Add(new GoldberriesPointTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB2"));
        TabList.Add(new MilestonesTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB3"));
        TabList.Add(new AnnualRecapTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB4"));
    }

    private static readonly float NameSize = 1f;

    private static readonly float TabLabelHeight = 50f;
    private static readonly float TabYOffset = 100f;
    private static readonly float TabYPosition = HUDPositionY + TabYOffset;

    public static readonly Vector2 TabPosition = new Vector2(HUDPositionX, TabYPosition + TabLabelHeight);
    public static readonly float TabHeight = HUDHeight - TabYOffset - TabLabelHeight;

    public void RenderTabs() {
        // Draw tabs

        float textSize = 0.5f;
        float labelSize = 2f;
        float tabSize = HUDWidth / TabCount;

        // Highlight selected tab
        Draw.Rect(HUDPositionX + tabSize * SelectedTab, TabYPosition, tabSize, TabLabelHeight, HighlightTabColor);

        // Draw labels
        Draw.Rect(HUDPositionX, TabYPosition, HUDWidth, labelSize, Color.Black);
        Draw.Rect(HUDPositionX, TabYPosition + TabLabelHeight, HUDWidth, labelSize, Color.Black);

        for (int i = 1; i <= TabCount; i++) {
            float tabXPosition = HUDPositionX + tabSize * i;

            if (i < TabCount) {
                Draw.Rect(tabXPosition, TabYPosition, labelSize, TabLabelHeight, Color.Black);
            }

            Vector2 pointer = new Vector2(tabXPosition - tabSize / 2, TabYPosition + TabLabelHeight / 2);
            ActiveFont.Draw(
                TabList[i - 1].GetTitle(), 
                pointer,
                Vector2.One / 2,
                Vector2.One * textSize,
                Color.Black
            );
        }

        TabList[SelectedTab].Render();
    }

    public void InnerRender() {
        // Render player name
        string name = GoldberriesStatsManager.PlayerName;
        Color color = GoldberriesStatsManager.PlayerNameColor;
        Vector2 pointer = new Vector2(Engine.Width / 2, HUDPositionY + TabYOffset / 2);
        ActiveFont.Draw(name, pointer, Vector2.One / 2, Vector2.One * NameSize, color);

        RenderTabs();
    } 

    private static void RenderWarning(string message) {
        ActiveFont.Draw(
            Dialog.Clean(message),
            new Vector2(Engine.Width / 2, Engine.Height / 2),
            Vector2.One / 2,
            Vector2.One * 0.8f,
            Color.Black
        );
    }

    public override void Render() {
        base.Render();

        Draw.Rect(Position, HUDSize.X, HUDSize.Y, HUDColor);

        if (!GoldberriesStatsManager.StatsFetched) {
            RenderWarning("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_FETCHED");
        } else if (!GoldberriesStatsManager.PlayerHasSubmissions) {
            RenderWarning("MODOPTION_GOLDBERRIES_INTEGRATION_PLAYER_HAS_NO_SUBMISSIONS");
        } else if (!GoldberriesStatsManager.Initialized) {
            RenderWarning("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_INITIALIZED");
        } else {
            InnerRender();
        }
    }

    public override void Update() {
        base.Update();
        
        if (ModSettings.ButtonViewCharts.Pressed) {
            Toggle();
        }

        if (!Visible) {
            return;
        }

        if (Input.MenuCancel.Pressed || Input.ESC.Pressed) {
            Hide();
            Input.ESC.ConsumePress();
        }

        if (!GoldberriesStatsManager.Initialized) return;

        TabList[SelectedTab].Update();

        if (Input.MenuRight.Pressed && SelectedTab + 1 < TabCount) {
            SelectedTab++;
        }
        
        if (Input.MenuLeft.Pressed && SelectedTab - 1 >= 0) {
            SelectedTab--;
        }
    }

}