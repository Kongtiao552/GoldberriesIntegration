using System;
using System.Collections.Generic;
using System.IO;
using Celeste.Mod.GoldberriesIntegration.Misc;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Stats;

public abstract class GBStat {

    public void Initialize(List<Submission> submissions) {
        Reset();
        Utils.Log($"Initializing {GetType().Name}", LogLevel.Info);
        InitializeStat(submissions);
        Utils.Log($"{GetType().Name} Initialized", LogLevel.Info);
        Initialized = true;
    }

    public void Load(string json) {
        Reset();
        Utils.Log($"Loading {GetType().Name} from JSON", LogLevel.Info);
        JsonConvert.PopulateObject(json, this);
        Utils.Log($"{GetType().Name} Loaded", LogLevel.Info);
    }

    public string Save() {
        Utils.Log($"Saving {GetType().Name} to JSON", LogLevel.Info);
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        Utils.Log($"{GetType().Name} Saved", LogLevel.Info);
        return json;
    }

    public abstract void InitializeStat(List<Submission> submissions);

    public abstract void Reset();

    [JsonIgnore]
    public bool Initialized { get; set; } = false;

}