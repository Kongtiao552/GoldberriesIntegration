using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Celeste.Mod.GoldberriesStat.Entities;
using Celeste.Mod.GoldberriesStat.Misc;
using Celeste.Mod.GoldberriesStat.Stats;

namespace Celeste.Mod.GoldberriesStat;

public class GoldberriesStatModule : EverestModule {

    public static GoldberriesStatModule Instance { get; private set; }

    public override Type SettingsType => typeof(GoldberriesStatModuleSettings);
    
    public static GoldberriesStatModuleSettings ModSettings => (GoldberriesStatModuleSettings) Instance._Settings;

    public GoldberriesStatModule() {
        Instance = this;

        Logger.SetLogLevel(nameof(GoldberriesStatModule), LogLevel.Info);
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