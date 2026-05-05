using System;
using System.Collections.Generic;
using System.IO;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public abstract class GBStat {

    public void Calculate(List<Submission> submissions) {
        Reset();
        Utils.Log($"Initializing {GetType().Name}");
        CalculateStat(submissions);
        Utils.Log($"{GetType().Name} Initialized");
    }

    public void Load(string json) {
        Reset();
        Utils.Log($"Loading {GetType().Name} from JSON");
        JsonConvert.PopulateObject(json, this);
        Utils.Log($"{GetType().Name} Loaded");
    }

    public string Save() {
        Utils.Log($"Saving {GetType().Name} to JSON");
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        Utils.Log($"{GetType().Name} Saved");
        return json;
    }

    public abstract void CalculateStat(List<Submission> submissions);

    public abstract void Reset();

}