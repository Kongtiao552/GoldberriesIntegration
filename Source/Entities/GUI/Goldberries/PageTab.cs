using System;

namespace Celeste.Mod.GoldberriesIntegration.Entities.GUI.Goldberries;

public abstract class PageTab : StatTab {

    public PageTab(string title) : base(title) {}

    public int CurrentPage { get; set; } = 1;
    public int PageAmount { get; set; } = 1;

    public override string GetTitle() {
        return base.GetTitle() + $" ({CurrentPage}/{PageAmount})";
    }

    public override void Update() {
        if (Input.MenuRight.Pressed && ModSettings.ButtonTogglePageModifier.Check && CurrentPage < PageAmount) {
            CurrentPage++;
            Input.MenuRight.ConsumePress();
        }

        if (Input.MenuLeft.Pressed && ModSettings.ButtonTogglePageModifier.Check && CurrentPage > 1) {
            CurrentPage--;
            Input.MenuLeft.ConsumePress();
        }
    }

}