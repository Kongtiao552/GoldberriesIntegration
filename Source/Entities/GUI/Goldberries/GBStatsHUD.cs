using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

public class GBStatsHUD : InGameWindow {

    public override Color WindowColor { get; set; } = new Color(255, 242, 204);

    public static readonly Color HighlightTabColor = new Color(255, 217, 102);

    public List<Tab> TabList { get; set; } = new List<Tab>();

    public int SelectedTab { get; set; } = 0;

    public int TabCount => TabList.Count;

    public static GBStatsHUD Instance { get; } = new GBStatsHUD();

    public GBStatsHUD() : base() {
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

            Vector2 pointer = new Vector2(tabXPosition - tabSize / 2, TabYPosition + TabHeight / 2);
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
        Vector2 pointer = new Vector2(Engine.Width / 2, WindowPositionY + TabYOffset / 2);
        ActiveFont.Draw(name, pointer, Vector2.One / 2, Vector2.One * NameSize, color);

        RenderTabs();
    } 

    public override void Render() {
        base.Render();

        if (!GoldberriesStatsManager.StatsFetched) {
            ActiveFont.Draw(
                Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_FETCHED"),
                new Vector2(Engine.Width / 2, Engine.Height / 2),
                Vector2.One / 2,
                Vector2.One * 0.8f,
                Color.Black
            );
        } else if (!GoldberriesStatsManager.Initialized) {
                ActiveFont.Draw(
                Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_INITIALIZED"),
                new Vector2(Engine.Width / 2, Engine.Height / 2),
                Vector2.One / 2,
                Vector2.One * 0.8f,
                Color.Black
            );
        } else {
            InnerRender();
        }
    }

    public override void Update() {
        base.Update();

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