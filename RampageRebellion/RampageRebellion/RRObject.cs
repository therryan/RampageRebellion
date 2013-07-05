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
/// Class that inherits PhysicsObject, used for all non-base PhysicsObject entities.
/// </summary>
[Serializable]
public class RRObject : PhysicsObject
{
    /// <summary>
    /// Health value.
    /// </summary>
    public double Health;

    /// <summary>
    /// Score value.
    /// </summary>
    public double ScoreValue;

    /// <summary>
    /// Power value.
    /// </summary>
    public double PowerValue;

    private RRWeapon MainWeapon;
    public virtual void onDeath() { }

    public RRObject(double width, double height)
        : base(width, height)
    {
    }
}