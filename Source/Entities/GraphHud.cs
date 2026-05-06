using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

[Tracked]
public class GraphHud : Entity {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    public static readonly float HudPositionX = Engine.Width * 0.1f;
    public static readonly float HudPositionY = Engine.Height * 0.1f;
    public static readonly float HudWidth = Engine.Width * 0.8f;
    public static readonly float HudHeight = Engine.Height * 0.8f;

    public static readonly Vector2 HudPosition = new Vector2(HudPositionX, HudPositionY);
    public static readonly Vector2 HudSize = new Vector2(HudWidth, HudHeight);

    public Color HudColor { get; set; } = new Color(255, 242, 204);

    public static readonly Color HighlightTabColor = new Color(255, 217, 102);

    public List<Tab> TabList { get; set; }

    public int SelectedTab { get; set; } = 0;

    public int TabCount => TabList.Count;

    public GraphHud() {
        Depth = -10000;
        Tag = Tags.Global | Tags.HUD | Tags.PauseUpdate | Tags.TransitionUpdate;
        Position = HudPosition;
        Visible = false;
        Active = true;

        TabList = new List<Tab>() {
            new TimeSpentTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB1"),
            new GoldberriesPointTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB2"),
            new MilestonesTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB3"),
            new AnnualRecapTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB4"),
            new MiscTab("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_TAB5")
        };
    }

    private static readonly float NameSize = 1f;

    private static readonly float TabLabelHeight = 50f;
    private static readonly float TabYOffset = 100f;
    private static readonly float TabYPosition = HudPositionY + TabYOffset;

    public static readonly Vector2 TabPosition = new Vector2(HudPositionX, TabYPosition + TabLabelHeight);
    public static readonly float TabHeight = HudHeight - TabYOffset - TabLabelHeight;

    public static readonly Vector2 TabLeftTop = TabPosition;
    public static readonly Vector2 TabRightTop = TabPosition.MoveCopy(HudWidth, 0f);
    public static readonly Vector2 TabLeftBottom = HudPosition.MoveCopy(0f, HudHeight);
    public static readonly Vector2 TabRightBottom = HudPosition.MoveCopy(HudWidth, HudHeight);

    private void RenderTabs() {
        float textSize = 0.5f;
        float labelSize = 2f;
        float tabSize = HudWidth / TabCount;

        Draw.Rect(HudPositionX + tabSize * SelectedTab, TabYPosition, tabSize, TabLabelHeight, HighlightTabColor);

        Draw.Rect(HudPositionX, TabYPosition, HudWidth, labelSize, Color.Black);
        Draw.Rect(HudPositionX, TabYPosition + TabLabelHeight, HudWidth, labelSize, Color.Black);

        for (int i = 1; i <= TabCount; i++) {
            float tabXPosition = HudPositionX + tabSize * i;

            if (i < TabCount) {
                Draw.Rect(tabXPosition, TabYPosition, labelSize, TabLabelHeight, Color.Black);
            }

            Vector2 pointer = new(tabXPosition - tabSize / 2f, TabYPosition + TabLabelHeight / 2f);
            ActiveFont.Draw(
                TabList[i - 1].GetTitle(), 
                pointer,
                Vector2.One / 2f,
                Vector2.One * textSize,
                Color.Black
            );
        }

        TabList[SelectedTab].Render();
    }

    private void InnerRender() {
        // Render player name
        string name = StatManager.PlayerName;
        Color color = StatManager.PlayerNameColor;
        Vector2 pointer = new Vector2(Engine.Width / 2, HudPositionY + TabYOffset / 2);
        ActiveFont.Draw(name, pointer, Vector2.One / 2, Vector2.One * NameSize, color);

        RenderTabs();
    } 

    private static void RenderWarning(string message) {
        ActiveFont.Draw(
            Dialog.Clean(message),
            new Vector2(Engine.Width / 2f, Engine.Height / 2f),
            Vector2.One / 2,
            Vector2.One * 0.8f,
            Color.Black
        );
    }

    public override void Render() {
        base.Render();

        Draw.Rect(Position, HudSize.X, HudSize.Y, HudColor);

        if (!StatManager.StatsFetched) {
            RenderWarning("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_FETCHED");
        } else if (!StatManager.PlayerHasSubmissions) {
            RenderWarning("MODOPTION_GOLDBERRIES_INTEGRATION_PLAYER_HAS_NO_SUBMISSIONS");
        } else if (!StatManager.Initialized) {
            RenderWarning("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_NOT_INITIALIZED");
        } else {
            InnerRender();
        }
    }

    public void Open() {
        Level level = SceneAs<Level>();

        if (level != null) {
            Visible = true;
            level.Paused = true;
            UpdateTabs();
        }
    }

    public void Close() {
        Level level = SceneAs<Level>();

        if (level != null) {
            Visible = false;
            level.Paused = false;
        }
    }

    private void MoveTab(int step) {
        SelectedTab += step;
    
        if (SelectedTab >= TabCount) {
            SelectedTab = 0;
        } else if (SelectedTab < 0) {
            SelectedTab = TabCount - 1;
        }

        TabList[SelectedTab].Update();
    }

    public override void Update() {
        base.Update();

        if (!Visible) {
            if (ModSettings.ButtonViewGraphs.Pressed && Scene.Entities.FindFirst<TextMenu>() == null) {
                Open();
            }

            return;
        }

        if (ModSettings.ButtonViewGraphs.Pressed || Input.MenuCancel.Pressed || Input.ESC.Pressed) {
            Close();
            Input.ESC.ConsumeBuffer();
        }

        if (!StatManager.Initialized) return;

        if (Input.MenuRight.Pressed) {
            if (ModSettings.ButtonTogglePageModifier.Check) {
                TabList[SelectedTab].MovePage(1);
            } else {
                MoveTab(1);
            }
        }
        
        if (Input.MenuLeft.Pressed) {
            if (ModSettings.ButtonTogglePageModifier.Check) {
                TabList[SelectedTab].MovePage(-1);
            } else {
                MoveTab(-1);
            };
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        StatManager.OnStatInitialized += On_Stat_Initialized;
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);

        StatManager.OnStatInitialized -= On_Stat_Initialized;
    }

    private void UpdateTabs() {
        foreach (Tab tab in TabList) {
            tab.Update();
        }
    }

    private void On_Stat_Initialized() {
        foreach (Tab tab in TabList) {
            tab.SelectedPage = 1;
        }

        UpdateTabs();
    }

}