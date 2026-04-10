using System;
using System.Globalization;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesIntegration;
public static class Extensions
{

    public static Color ToColor(this string colorString) {
        colorString = colorString.Replace("#", "");
        
        int r = int.Parse(colorString.Substring(0, 2), NumberStyles.HexNumber);
        int g = int.Parse(colorString.Substring(2, 2), NumberStyles.HexNumber);
        int b = int.Parse(colorString.Substring(4, 2), NumberStyles.HexNumber);

        return new Color(r, g, b);
    }

}