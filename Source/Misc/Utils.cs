using Celeste.Mod.GoldberriesIntegration;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Misc;

public static class Utils {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    public static void Log(string message, LogLevel level = LogLevel.Info) => Logger.Log(level, "GoldberriesIntegration", message);

    public static Vector2 ScreenCenter => new Vector2(Engine.Width, Engine.Height) / 2;

}