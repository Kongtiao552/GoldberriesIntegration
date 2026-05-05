using System;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

public abstract class Tab {

    public int SelectedPage { get; set; } = 1;
    public int PageAmount { get; set; } = 1;

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    private string Title { get; set; }

    public virtual string GetTitle() => Title;

    public Tab(string title) {
        Title = Dialog.Clean(title);
    }

    public void MovePage(int step) {
        SelectedPage += step;
    
        if (SelectedPage > PageAmount) {
            SelectedPage = 1;
        } else if (SelectedPage < 1) {
            SelectedPage = PageAmount;
        }

        Update();
    }

    public virtual void Update() {
        if (SelectedPage > PageAmount) {
            SelectedPage = PageAmount;
        }
    }

    public abstract void Render();
    
}