using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

/// @author     Jaakko Lipas
/// @version    Dev - 18.4.2013
/// 
/// <summary>
/// A projectile class to be used by ShipWeapons.
/// </summary>
public class LProjectile : RRProjectile
{
    /// <summary>
    /// Player weapon damage.
    /// </summary>
    public int Damage;

    public LProjectile(double width, double height, double mass, Color color)
        : base(width, height, mass, color)
    {
    }
}

