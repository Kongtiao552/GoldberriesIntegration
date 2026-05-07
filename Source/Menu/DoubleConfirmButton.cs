using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesStat.Menu;

public class DoubleConfirmButton : ColoredButton {

    private bool PressedOnce { get; set; } = false;

    public Action OnDoubleConfirm { get; set; }

    private static readonly string confirmString = " - Confirm";

    public DoubleConfirmButton(string label, Color color) : base(label, color) {
        OnPressed = () => {
            if (PressedOnce) {
                UpdateStatus(false);
                OnDoubleConfirm?.Invoke();
            } else {
                UpdateStatus(true);
            }
        };

        OnLeave = () => UpdateStatus(false);
    }

    private void UpdateStatus(bool pressedOnce) {
        if (pressedOnce) {
            Label += confirmString;
            PressedOnce = true;
        } else {
            Label = Label.Replace(confirmString, "");
            PressedOnce = false;
        }
    }
    
}