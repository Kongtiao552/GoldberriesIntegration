using System;
using Celeste.Mod.GoldberriesIntegration.Models.Goldberries;
using Celeste.Mod.GoldberriesIntegration.Stats;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration.Entities;

public class AnnualRecapTab : PageTab {

    public AnnualRecapTab(string title) : base(title) {

    }

    private static AnnualRecapStat AnnualRecapStatInstance => AnnualRecapStat.Instance;

    public override int GetPageAmount() => AnnualRecapStatInstance.AnnualRecaps.Count;

    public override void SetPageAmount(int amount) {
        throw new NotSupportedException("You cannot set the page amount of AnnualRecapTab as it is determined by how long the player has played the game.");
    }

    private AnnualRecap AnnualRecap => AnnualRecapStatInstance.AnnualRecaps[CurrentPage - 1];

    public override void Render() {
        Vector2 pointer = GBStatsHUD.TabPosition;
        Vector2 offset = new Vector2(30f, 30f);
        pointer += offset;

        ActiveFont.Draw(AnnualRecap.Year.ToString(), pointer, Vector2.Zero, Vector2.One, Color.Black);
    }

}