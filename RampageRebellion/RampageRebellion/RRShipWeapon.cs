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
/// Class that inherits Weapon, used to add a weapon to the player.
/// </summary>
public class ShipWeapon : RRWeapon
{
    public ShipWeapon(double width, double height)
        : base(width, height)
    {
        
    }

    protected override PhysicsObject CreateProjectile()
    {
        RRProjectile playerLaser = new RRProjectile(20, 3, 0.1, new Color(255, 55, 128, 128));
        playerLaser.Damage = 8;
        playerLaser.Tag = "Z";
        playerLaser.CollisionIgnoreGroup = 2;
        playerLaser.explosionUponDestruction = false;
        playerLaser.CanRotate = false;
        this.Power.Value = RampageRebellion.PLAYER_WEAPONPOWER;

        return playerLaser;
    }
}
