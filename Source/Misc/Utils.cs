using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesStat;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesStat.Misc;

public static class Utils {

    public static void Log(string message, LogLevel level = LogLevel.Info) => Logger.Log(level, "GoldberriesIntegration", message);

    public static string ToPercentString(this float num) => num.ToString("P2").Replace(" ", "");
    public static string ToPercentString(this double num) => num.ToString("P2").Replace(" ", "");
    public static string ToPercentString(this decimal num) => num.ToString("P2").Replace(" ", "");

    public static string GetPercentString(float portion, float total) => (portion / total).ToPercentString();
    public static string GetPercentString(double portion, double total) => (portion / total).ToPercentString();
    public static string GetPercentString(decimal portion, decimal total) => (portion / total).ToPercentString();
    public static string GetPercentString(TimeSpan portion, TimeSpan total) => (portion / total).ToPercentString();

    public static string Format(TimeSpan timeSpan) {
        return $"{timeSpan.Days * 24 + timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    public static void Move(ref this Vector2 pointer, float x, float y) {
        pointer += new Vector2(x, y);
    }

    public static void MoveX(ref this Vector2 pointer, float x) {
        pointer.X += x;
    }

    public static void MoveY(ref this Vector2 pointer, float y) {
        pointer.Y += y;
    }

    public static Vector2 MoveCopy(this Vector2 pointer, float x, float y) {
        return pointer + new Vector2(x, y);
    }

}