using Celeste;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Celeste.Mod.GoldberriesIntegration.Menu;
using Microsoft.Xna.Framework;
using Celeste.Mod.GoldberriesIntegration.Misc;
using YamlDotNet.Serialization;
using System;
using Celeste.Mod.GoldberriesIntegration.Entities;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration;

[SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_MODULE_TITLE")]
public class GoldberriesIntegrationModuleSettings : EverestModuleSettings {

    [SettingIgnore]
    public static GoldberriesIntegrationModule Module => GoldberriesIntegrationModule.Instance;

    [SettingNumberInput(allowNegatives: false)]
    public int PlayerId { get; set; } = 0;
    
    [YamlIgnore]
    public bool StatsOptions { get; set; }

    public void CreateStatsOptionsEntry(TextMenu menu, bool inGame) {
        TextMenuExt.SubMenu subMenu = new TextMenuExt.SubMenu(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS"), false);

        DoubleConfirmButton resetStatsButton = new DoubleConfirmButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_RESET_BUTTON"), Color.Red) {
            OnDoubleConfirm = StatManager.Reset,
            Disabled = !StatManager.StatsFetched || StatManager.IsFetching
        };

        DoubleConfirmButton fetchStatsButton = new DoubleConfirmButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_FETCH_BUTTON"), Color.Yellow);

        fetchStatsButton.OnDoubleConfirm = async () => {
            string fetchingString = " (Fetching...)";
            string failedString = " (Failed to fetch stats)";

            fetchStatsButton.Disabled = true;
            fetchStatsButton.Label += fetchingString;
            resetStatsButton.Disabled = true;

            try {
                await StatManager.Fetch(PlayerId);

                fetchStatsButton.Disabled = false;
                fetchStatsButton.Label = fetchStatsButton.Label.Replace(fetchingString, "");
                resetStatsButton.Disabled = false;
            } catch (Exception e) {
                Utils.Log($"Error fetching stats: {e}", LogLevel.Error);
                fetchStatsButton.Label = fetchStatsButton.Label.Replace(fetchingString, failedString);
            }
        };

        fetchStatsButton.Disabled = PlayerId < 1 || StatManager.IsFetching;

        TextMenu.Button viewGraphsButton = new TextMenu.Button(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_GRAPHS")) {
            Disabled = !inGame,
            OnPressed = () => {
                if (Engine.Scene is Level level) {
                    GraphHud hud = level.Tracker.GetEntity<GraphHud>();
                    
                    if (hud == null) return;

                    menu.RemoveSelf();   
                    hud.Open();
                }           
            }
        };

        subMenu.Add(fetchStatsButton);
        subMenu.Add(resetStatsButton);
        subMenu.Add(viewGraphsButton);

        viewGraphsButton.AddDescription(subMenu, menu, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_GRAPHS_DESCRIPTION"));

        menu.Add(subMenu);
    }

    [SettingSubHeader("MODOPTION_GOLDBERRIES_INTEGRATION_GRAPHS")]
    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_GRAPHS")]
    public ButtonBinding ButtonViewGraphs { get; set; }

    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_TOGGLE_PAGE_MODIFIER")]
    public ButtonBinding ButtonTogglePageModifier { get; set; }

}