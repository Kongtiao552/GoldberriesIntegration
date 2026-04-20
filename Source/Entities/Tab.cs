using System;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

public abstract class Tab : Entity {

    public static GoldberriesIntegrationModuleSettings ModSettings => GoldberriesIntegrationModule.ModSettings;

    private string Title { get; set; }

    public virtual string GetTitle() => Title;

    public Tab(string title) {
        Visible = false;
        Title = Dialog.Clean(title);
    }
    
}