using System;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

public abstract class PageTab : Tab {

    public PageTab(string title) : base(title) {}

    public int CurrentPage { get; set; } = 1;

    private int PageAmount { get; set; } = 1;

    public virtual int GetPageAmount() => PageAmount;
    public virtual void SetPageAmount(int amount) => PageAmount = amount;

    public override string GetTitle() {
        return base.GetTitle() + $" ({CurrentPage}/{GetPageAmount()})";
    }

    public override void Update() {
        if (CurrentPage > GetPageAmount()) {
            CurrentPage = GetPageAmount();
        }

        if (Input.MenuRight.Pressed && ModSettings.ButtonTogglePageModifier.Check && CurrentPage < GetPageAmount()) {
            CurrentPage++;
            Input.MenuRight.ConsumePress();
        }

        if (Input.MenuLeft.Pressed && ModSettings.ButtonTogglePageModifier.Check && CurrentPage > 1) {
            CurrentPage--;
            Input.MenuLeft.ConsumePress();
        }
    }

}