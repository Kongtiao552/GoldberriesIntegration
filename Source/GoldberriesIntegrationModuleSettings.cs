using Celeste;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Celeste.Mod.GoldberriesIntegration.Menu;
using Microsoft.Xna.Framework;
using Celeste.Mod.GoldberriesIntegration.Misc;
using System;
using Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

namespace Celeste.Mod.GoldberriesIntegration;

[SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_MODULE_TITLE")]
public class GoldberriesIntegrationModuleSettings : EverestModuleSettings {

    [SettingIgnore]
    public static GoldberriesIntegrationModule Module => GoldberriesIntegrationModule.Instance;

    [SettingNumberInput(allowNegatives: false)]
    public int PlayerId { get; set; } = 0;
    
    public bool StatsOptions { get; set; }

    public void CreateStatsOptionsEntry(TextMenu menu, bool inGame) {
        TextMenuExt.SubMenu subMenu = new TextMenuExt.SubMenu(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS"), false);

        DoubleComfirmationButton resetStatsButton = new DoubleComfirmationButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_RESET_BUTTON"), Color.Red) {
            OnDoubleComfirmation = GoldberriesStatsManager.Reset,
            Disabled = !GoldberriesStatsManager.StatsFetched
        };

        DoubleComfirmationButton fetchStatsButton = new DoubleComfirmationButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_FETCH_BUTTON"), Color.Yellow);

        fetchStatsButton.OnDoubleComfirmation = async () => {
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
            } catch (Exception) {
                fetchStatsButton.Label = fetchStatsButton.Label.Replace(fetchingString, failedString);
            }
        };

        fetchStatsButton.Disabled = PlayerId < 1;

        TextMenu.Button viewChartsButton = new TextMenu.Button(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_CHARTS")) {
            OnPressed = () => {
                menu.RemoveSelf();
                GoldberriesGUI.Instance.Show();
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