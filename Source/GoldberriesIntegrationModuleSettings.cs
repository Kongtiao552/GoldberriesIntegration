using Celeste;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Celeste.Mod.GoldberriesIntegration.Menu;
using Microsoft.Xna.Framework;
using Celeste.Mod.GoldberriesIntegration.Misc;
using YamlDotNet.Serialization;
using System;
using Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

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

        DoubleConfirmationButton resetStatsButton = new DoubleConfirmationButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_RESET_BUTTON"), Color.Red) {
            OnDoubleConfirmation = GoldberriesStatsManager.Reset,
            Disabled = !GoldberriesStatsManager.StatsFetched
        };

        DoubleConfirmationButton fetchStatsButton = new DoubleConfirmationButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_FETCH_BUTTON"), Color.Yellow);

        fetchStatsButton.OnDoubleConfirmation = async () => {
            string fetchingString = " (Fetching...)";
            string failedString = " (Failed to fetch stats)";

            fetchStatsButton.Disabled = true;
            fetchStatsButton.Label += fetchingString;
            resetStatsButton.Disabled = true;

            try {
                await GoldberriesStatsManager.Fetch(PlayerId);

                fetchStatsButton.Disabled = false;
                fetchStatsButton.Label = fetchStatsButton.Label.Replace(fetchingString, "");
                resetStatsButton.Disabled = false;
            } catch (Exception e) {
                Utils.Log($"Error fetching stats: {e.Message}", LogLevel.Error);
                fetchStatsButton.Label = fetchStatsButton.Label.Replace(fetchingString, failedString);
            }
        };

        fetchStatsButton.Disabled = PlayerId < 1;

        TextMenu.Button viewChartsButton = new TextMenu.Button(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_CHARTS")) {
            OnPressed = () => {
                menu.RemoveSelf();
                GBStatsHUD.Instance.Show();
            }
        };

        subMenu.Add(fetchStatsButton);
        subMenu.Add(resetStatsButton);
        subMenu.Add(viewChartsButton);

        menu.Add(subMenu);
    }

    [SettingSubHeader("MODOPTION_GOLDBERRIES_INTEGRATION_STATS")]
    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_CHARTS")]
    public ButtonBinding ButtonViewCharts { get; set; }

    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_TOGGLE_PAGE_MODIFIER")]
    public ButtonBinding ButtonTogglePageModifier { get; set; }

}