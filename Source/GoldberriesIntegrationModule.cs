using System;
using System.Collections.Generic;
using System.Globalization;
using Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Stats;

namespace Celeste.Mod.GoldberriesIntegration;

public class GoldberriesIntegrationModule : EverestModule {

    public static GoldberriesIntegrationModule Instance { get; private set; }

    public override Type SettingsType => typeof(GoldberriesIntegrationModuleSettings);
    
    public static GoldberriesIntegrationModuleSettings ModSettings => (GoldberriesIntegrationModuleSettings) Instance._Settings;

    public GoldberriesIntegrationModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(GoldberriesIntegrationModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(GoldberriesIntegrationModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        GoldberriesStatsManager.LoadStatsFile();

        Everest.Events.Level.OnLoadLevel += On_Load_Level;

        On.Celeste.Level.Update += Level_Update;
    }

    public override void Unload() {
        Everest.Events.Level.OnLoadLevel -= On_Load_Level;

        On.Celeste.Level.Update -= Level_Update;
    }

    public static void On_Load_Level(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
        if (isFromLoader) {
            level.Add(GoldberriesGUI.Instance);
            Utils.Log("GoldberriesGUI Added");
        }
    }

    public static void Level_Update(On.Celeste.Level.orig_Update orig, Level self) {
        orig(self);

        if (!self.Paused) {
            UpdateHotkeyPresses();
        }
    }

    public static void UpdateHotkeyPresses() {
        if (ModSettings.ButtonViewCharts.Pressed) {
            GoldberriesGUI.Instance.Show();
        }
    }

}