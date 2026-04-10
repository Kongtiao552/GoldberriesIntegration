using System.Collections.Generic;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

public abstract class StatTab : Tab {

    public StatTab(string title) : base(title) {
        
    }
    
    public abstract List<GBStat> RequiredStats { get; set; }

    public override void Render() {
        base.Render();

        foreach (GBStat stat in RequiredStats) {
            if (!stat.Initialized) {
                InGameWindow.DrawTextInScreenCenter("MODOPTION_KONGTIAO_TOOLBOX_GOLDBERRIES_STATS_NOT_INITIALIZED", 0.8f, Color.Black, false);
                return;
            }
        }

        RenderTab();
    }

    public abstract void RenderTab();
    
}