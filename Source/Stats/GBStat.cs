using System;
using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public abstract class GBStat {

    public void Initialize(List<Submission> submissions) {
        Reset();
        Utils.Log($"Initializing {GetType().Name}", LogLevel.Info);
        InitializeStat(submissions);
        Utils.Log($"{GetType().Name} Initialized", LogLevel.Info);
    }

    public abstract void InitializeStat(List<Submission> submissions);

    public abstract void Reset();

    public bool Initialized { get; set; } = false;

}