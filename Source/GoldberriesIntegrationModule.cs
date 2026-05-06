using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Celeste.Mod.GoldberriesIntegration.Entities;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Stats;

namespace Celeste.Mod.GoldberriesIntegration;

public class GoldberriesIntegrationModule : EverestModule {

    public static GoldberriesIntegrationModule Instance { get; private set; }

    public override Type SettingsType => typeof(GoldberriesIntegrationModuleSettings);
    
    public static GoldberriesIntegrationModuleSettings ModSettings => (GoldberriesIntegrationModuleSettings) Instance._Settings;

    public GoldberriesIntegrationModule() {
        Instance = this;

        Logger.SetLogLevel(nameof(GoldberriesIntegrationModule), LogLevel.Info);
    }

    public override void Load() {
        Everest.Events.Level.OnLoadLevel += On_Load_Level;
    }

    public override void Unload() {
        Everest.Events.Level.OnLoadLevel -= On_Load_Level;
    }

    public override void Initialize() {
        InitializeHotkeys();

        if (ModSettings.StatsEnabled) {
            try {
                StatManager.CheckRootFolder();

                if (StatManager.CheckStatsFile()) {
                    StatManager.LoadStatsFile();
                }
            } catch (Exception e) {
                Utils.Log($"Error loading stats: {e}", LogLevel.Error);
            }
        }
    }

    private void InitializeHotkeys() {
        // Set default binds
        if (!ModSettings.ButtonTogglePageModifier.Binding.HasInput) {
            ModSettings.ButtonTogglePageModifier.Binding.Add(Input.Grab.Binding.Keyboard[0]);
            SaveSettings();
        }
    }

    private static void On_Load_Level(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
        if (isFromLoader && ModSettings.StatsEnabled) {
            level.Add(new GraphHud());
            Utils.Log("GraphHud Added");
        }
    }

}