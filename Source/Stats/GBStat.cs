using System;
using System.Collections.Generic;
using System.IO;
using Celeste.Mod.GoldberriesStat.Misc;
using Celeste.Mod.GoldberriesStat.Models;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesStat.Stats;

public abstract class GBStat {

    public void Initialize(List<Submission> submissions) {
        Reset();
        Utils.Log($"Initializing {GetType().Name}...");
        InitializeStat(submissions);
        Utils.Log($"{GetType().Name} Initialized");
    }

    public abstract void InitializeStat(List<Submission> submissions);

    public abstract void Reset();

}