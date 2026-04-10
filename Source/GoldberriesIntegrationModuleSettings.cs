using Celeste;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Celeste.Mod.GoldberriesIntegration.Menu;
using Microsoft.Xna.Framework;

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

        fetchStatsButton.OnDoubleComfirmation = () => GoldberriesStatsManager.Fetch(PlayerId, fetchStatsButton, resetStatsButton);
        fetchStatsButton.Disabled = PlayerId < 1;

        subMenu.Add(fetchStatsButton);
        subMenu.Add(resetStatsButton);

        menu.Add(subMenu);
    }

    [SettingSubHeader("MODOPTION_GOLDBERRIES_INTEGRATION_STATS")]
    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_CHARTS")]
    public ButtonBinding ButtonViewCharts { get; set; }

    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_TOGGLE_PAGE_MODIFIER")]
    public ButtonBinding ButtonTogglePageModifier { get; set; }

}