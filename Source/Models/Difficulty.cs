using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Celeste.Mod.GoldberriesStat.Stats;
using Microsoft.Xna.Framework;
using Monocle;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesStat.Models;

public class Difficulty {
    
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("sort")]
    public int Sort { get; set; }

    public static Dictionary<int, Color> Colors { get; } = new Dictionary<int, Color>() {
        {-1, Calc.HexToColor("#aaaaaa")},
        {0, Calc.HexToColor("#ffffff")},
        {1, Calc.HexToColor("#9696ff")},
        {2, Calc.HexToColor("#93aeff")},
        {3, Calc.HexToColor("#91c8ff")},
        {4, Calc.HexToColor("#8eecff")},
        {5, Calc.HexToColor("#8cffe2")},
        {6, Calc.HexToColor("#89ffb0")},
        {7, Calc.HexToColor("#9bff87")},
        {8, Calc.HexToColor("#b7ff84")},
        {9, Calc.HexToColor("#d5ff82")},
        {10, Calc.HexToColor("#f4ff7f")},
        {11, Calc.HexToColor("#fff47c")},
        {12, Calc.HexToColor("#ffdd7a")},
        {13, Calc.HexToColor("#ffc677")},
        {14, Calc.HexToColor("#ffae75")},
        {15, Calc.HexToColor("#ff9572")},
        {16, Calc.HexToColor("#ff7c70")},
        {17, Calc.HexToColor("#ff6d79")},
        {18, Calc.HexToColor("#ff6daa")},
        {19, Calc.HexToColor("#ff68d9")},
        {20, Calc.HexToColor("#f266ff")},
        {21, Calc.HexToColor("#d863ff")},
        {22, Calc.HexToColor("#bd60ff")}
    };  

    [JsonIgnore]
    public bool IsUntieredOrUndetermined { get; private set; }

    [JsonIgnore]
    public Color Color { get; private set; }

    [JsonIgnore]
    public double GP { get; private set; } = 0d;

    [OnDeserialized]
    internal void Initialize(StreamingContext context) {
        IsUntieredOrUndetermined = Sort < 1;

        if (!IsUntieredOrUndetermined) {
            GP = Math.Pow(1.43d, Sort - 1);
            Color = Colors[Sort];
        }
    }

    public override bool Equals(object obj) {
        return obj != null && obj is Difficulty other && other.Sort == Sort;
    }

    public override int GetHashCode() {
        return Sort.GetHashCode();
    }

}