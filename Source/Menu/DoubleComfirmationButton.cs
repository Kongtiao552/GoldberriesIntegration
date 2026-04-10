using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesIntegration.Menu;

public class DoubleComfirmationButton : ColoredButton {

    private bool PressedOnce { get; set; } = false;

    public Action OnDoubleComfirmation { get; set; }

    private static readonly string confirmString = " - Confirm";

    public DoubleComfirmationButton(string label, Color color) : base(label, color) {
        OnPressed = () => {
            if (PressedOnce) {
                UpdateStatus(false);
                OnDoubleComfirmation?.Invoke();
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