using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

/// <summary>
/// RRWeapon class. Deviates from Weapon class by NOT resetting Power on each shot
/// </summary>
public class RRWeapon : Weapon
{

    public RRProjectileGenerator projectileGenerator;
    public double weaponPower;
    public bool countInWeaponArsenal = true;
    public double randomizedAngleSlate = 0;
    private Angle baseAngle;
    private Boolean angleSet = false;

    public RRWeapon(double width, double height, RRProjectileGenerator generator) : base(width, height)
    {
        projectileGenerator = generator;
        this.IsVisible = false;
        this.AmmoIgnoresExplosions = true;
        this.AmmoIgnoresGravity = true;
    }

    public RRWeapon(double width, double height)
        : this(width, height, new RRProjectileGenerator())
    {
    }


    public void setPower (DoubleMeter power) 
    {
        this.Power = power;
    }

    public void setAmmo(IntMeter ammo)
    {
        this.Ammo = ammo;
    }

    /// <summary>
    /// Create a new RRWeapon class
    /// </summary>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="infiniteAmmo">Has infinite ammo?</param>
    /// <param name="power">How much force this weapon uses?</param>
    /// <param name="fireRate">Fire rate?</param>

    public RRWeapon(double width, double height, bool infiniteAmmo, double power, double fireRate) : this(width, height)
    {
        this.InfiniteAmmo = infiniteAmmo;
        this.weaponPower = power;
        this.Power.Value = power;
        this.FireRate = fireRate;
    }

    public RRWeapon(double width, double height, bool infiniteAmmo, double power, double fireRate, RRProjectileGenerator generator)
        : this(width, height, infiniteAmmo, power, fireRate)
    {
        this.projectileGenerator = generator;
    }

    public void setBaseAngle(Angle a)
    {
        baseAngle = a;
        angleSet = true;
    }

    protected override PhysicsObject CreateProjectile()
    {
        if (projectileGenerator == null) return null;

        if ((randomizedAngleSlate > 0) && angleSet) this.Angle = baseAngle + RandomGen.NextAngle(Angle.FromDegrees(-randomizedAngleSlate), Angle.FromDegrees(randomizedAngleSlate));

        Power.Value = weaponPower;

        return projectileGenerator.projectile();
    }
}