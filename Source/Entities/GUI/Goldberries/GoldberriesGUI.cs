using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

public class GoldberriesGUI : InGameWindow {

    public override Color WindowColor { get; set; } = new Color(255, 242, 204);

    public static readonly Color HighlightTabColor = new Color(255, 217, 102);

    public List<StatTab> TabList { get; set; } = new List<StatTab>();

    public int SelectedTab { get; set; } = 0;

    public int TabCount => TabList.Count;

    public static GoldberriesGUI Instance { get; } = new GoldberriesGUI();

    public GoldberriesGUI() : base() {
        TabList.Add(new TimeSpentTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB1"));
        TabList.Add(new GoldberriesPointTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB2"));
        TabList.Add(new MilestonesTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB3"));
    }

    private static readonly float NameSize = 1f;

    private static readonly float TabHeight = 50f;
    private static readonly float TabYOffset = 100f;
    private static readonly float TabYPosition = WindowPositionY + TabYOffset;

    public static Vector2 TabPosition = new Vector2(WindowPositionX, TabYPosition + TabHeight);
    public static readonly float TabContentHeight = WindowHeight - TabYOffset - TabHeight;

    public void RenderTabs() {
        // Draw tabs

        float textSize = 0.5f;
        float labelSize = 2f;
        float tabSize = WindowWidth / TabCount;

        // Highlight selected tab
        Draw.Rect(WindowPositionX + tabSize * SelectedTab, TabYPosition, tabSize, TabHeight, HighlightTabColor);

        // Draw labels
        Draw.Rect(WindowPositionX, TabYPosition, WindowWidth, labelSize, Color.Black);
        Draw.Rect(WindowPositionX, TabYPosition + TabHeight, WindowWidth, labelSize, Color.Black);

        for (int i = 1; i <= TabCount; i++) {
            float tabXPosition = WindowPositionX + tabSize * i;

            if (i < TabCount) {
                Draw.Rect(tabXPosition, TabYPosition, labelSize, TabHeight, Color.Black);
            }

            DrawTextInCenter(TabList[i - 1].GetTitle(), textSize, new Vector2(tabXPosition - tabSize / 2, TabYPosition + TabHeight / 2), Color.Black, false);
        }

        TabList[SelectedTab].Render();
    }

    public void InnerRender() {
        // Render player name
        string name = GoldberriesStatsManager.PlayerName;
        Vector2 measure = ActiveFont.Measure(name) * NameSize;
        Vector2 pointer = new Vector2(Engine.Width / 2, WindowPositionY + TabYOffset / 2);
        pointer -= measure / 2;
        DrawText(GoldberriesStatsManager.PlayerName, NameSize, pointer, GoldberriesStatsManager.PlayerNameColor, false);

        RenderTabs();
    } 

    public override void Render() {
        base.Render();

        if (!GoldberriesStatsManager.StatsFetched) {
            DrawTextInScreenCenter("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_FETCHED", 0.8f, Color.Black, false);
        } else if (!GoldberriesStatsManager.Initialized) {
            DrawTextInScreenCenter("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_INITIALIZED", 0.8f, Color.Black, false);
        } else {
            InnerRender();
        }
    }

    public override void Update() {
        base.Update();

        if (!GoldberriesStatsManager.Initialized) return;

        TabList[SelectedTab].Update();

        if (Input.MenuRight.Pressed && !ModSettings.ButtonTogglePageModifier.Check && SelectedTab + 1 < TabCount) {
            SelectedTab++;
        }
        
        if (Input.MenuLeft.Pressed && !ModSettings.ButtonTogglePageModifier.Check && SelectedTab - 1 >= 0) {
            SelectedTab--;
        }
    }

}