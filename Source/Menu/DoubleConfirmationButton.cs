using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GoldberriesIntegration.Menu;

public class DoubleConfirmationButton : ColoredButton {

    private bool PressedOnce { get; set; } = false;

    public Action OnDoubleConfirmation { get; set; }

    private static readonly string confirmString = " - Confirm";

    public DoubleConfirmationButton(string label, Color color) : base(label, color) {
        OnPressed = () => {
            if (PressedOnce) {
                UpdateStatus(false);
                OnDoubleConfirmation?.Invoke();
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