using Celeste;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Celeste.Mod.GoldberriesIntegration.Menu;
using Microsoft.Xna.Framework;
using Celeste.Mod.GoldberriesIntegration.Misc;
using YamlDotNet.Serialization;
using System;
using Celeste.Mod.GoldberriesIntegration.Entities;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GoldberriesIntegration;

[SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_MODULE_TITLE")]
public class GoldberriesIntegrationModuleSettings : EverestModuleSettings {

    [SettingIgnore]
    public static GoldberriesIntegrationModule Module => GoldberriesIntegrationModule.Instance;

    [SettingNumberInput(allowNegatives: false)]
    public int PlayerId { get; set; } = 0;
    
    [YamlIgnore]
    public bool StatsOptions { get; set; }

    [SettingIgnore]
    public bool StatsEnabled { get; set; } = true;

    private bool IsFetchButtonDisabled => PlayerId < 1 || StatManager.IsFetching;
    private bool IsResetButtonDisabled => !StatManager.StatsFetched || StatManager.IsFetching;

    public void CreateStatsOptionsEntry(TextMenu menu, bool inGame) {
        TextMenuExt.SubMenu subMenu = new TextMenuExt.SubMenu(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS"), false);

        DoubleConfirmButton resetStatsButton = new DoubleConfirmButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_RESET_BUTTON"), Color.Red) {
            OnDoubleConfirm = StatManager.Reset,
            Disabled = !StatsEnabled || IsResetButtonDisabled
        };

        DoubleConfirmButton fetchStatsButton = new DoubleConfirmButton(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_STATS_FETCH_BUTTON"), Color.Yellow);

        fetchStatsButton.OnDoubleConfirm = async () => {
            string fetchingString = " (Fetching...)";
            string failedString = " (Failed to fetch stats)";

            fetchStatsButton.Disabled = true;
            fetchStatsButton.Label += fetchingString;
            resetStatsButton.Disabled = true;

            StatManager.IsFetching = true;

            try {
                await StatManager.Fetch(PlayerId);

                fetchStatsButton.Disabled = false;
                fetchStatsButton.Label = fetchStatsButton.Label.Replace(fetchingString, "");
                resetStatsButton.Disabled = false;
            } catch (Exception e) {
                Utils.Log($"Error fetching stats: {e}", LogLevel.Error);
                fetchStatsButton.Label = fetchStatsButton.Label.Replace(fetchingString, failedString);
            } finally {
                StatManager.IsFetching = false;
            }
        };

        fetchStatsButton.Disabled = !StatsEnabled || IsFetchButtonDisabled;

        TextMenu.Button viewGraphsButton = new TextMenu.Button(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_GRAPHS")) {
            Disabled = !inGame || !StatsEnabled,
            OnPressed = () => {
                if (Engine.Scene is Level level) {
                    GraphHud hud = level.Tracker.GetEntity<GraphHud>();
                    
                    if (hud == null) return;

                    menu.RemoveSelf();   
                    hud.Open();
                }           
            }
        };

       TextMenu.OnOff enableStatsOnOff = new TextMenu.OnOff(Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_ENABLED"), StatsEnabled) {
            OnValueChange = v => {
                StatsEnabled = v;
                resetStatsButton.Disabled = !StatsEnabled || IsResetButtonDisabled;
                fetchStatsButton.Disabled = !StatsEnabled || IsFetchButtonDisabled;
                viewGraphsButton.Disabled = !StatsEnabled || !inGame;
            }
        };

        subMenu.Add(enableStatsOnOff);
        subMenu.Add(fetchStatsButton);
        subMenu.Add(resetStatsButton);
        subMenu.Add(viewGraphsButton);

        NeedsRelaunch(enableStatsOnOff, subMenu, menu);

        enableStatsOnOff.AddDescription(subMenu, menu, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_STATS_ENABLED_DESCRIPTION"));
        fetchStatsButton.AddDescription(subMenu, menu, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_FETCH_BUTTON_DESCRIPTION"));
        viewGraphsButton.AddDescription(subMenu, menu, Dialog.Clean("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_GRAPHS_DESCRIPTION"));

        menu.Add(subMenu);
    }

    [SettingSubHeader("MODOPTION_GOLDBERRIES_INTEGRATION_GRAPHS")]
    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_VIEW_GRAPHS")]
    public ButtonBinding ButtonViewGraphs { get; set; }

    [SettingName("MODOPTION_GOLDBERRIES_INTEGRATION_GOLDBERRIES_TOGGLE_PAGE_MODIFIER")]
    public ButtonBinding ButtonTogglePageModifier { get; set; }

    // Stolen from NeedsRelaunch(this TextMenu.Item option, TextMenu containingMenu, bool needsRelaunch = true)
    // Made specifically for submenus
    private static void NeedsRelaunch(TextMenu.Item option, TextMenuExt.SubMenu containingSubMenu, TextMenu parentContainer) {
        TextMenuExt.EaseInSubHeaderExt needsRelaunchText = new TextMenuExt.EaseInSubHeaderExt(Dialog.Clean("MODOPTIONS_NEEDSRELAUNCH"), initiallyVisible: false, parentContainer) {
            TextColor = Color.OrangeRed,
            HeightExtra = 0f
        };

        List<TextMenu.Item> items = containingSubMenu.Items;

        if (items.Contains(option)) {
            containingSubMenu.Insert(items.IndexOf(option) + 1, needsRelaunchText);
        } else if (containingSubMenu.ContainsDelayedAddItem(option)) {
            containingSubMenu.InsertDelayedAddItem(needsRelaunchText, option);
        }

        option.OnEnter += () => needsRelaunchText.FadeVisible = true;
        option.OnLeave += () => needsRelaunchText.FadeVisible = false;
    }

}