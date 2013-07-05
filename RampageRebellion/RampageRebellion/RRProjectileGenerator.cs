using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

/// @author arttuys
/// 
/// <summary>
/// 
/// A projectile generator that generates projectiles for RRWeapon. Designed to be reusable
/// 
/// </summary>

public class RRProjectileGenerator
{
    public double pRadius;
    public double pMass;
    public string pImageName;

    public double pWidth;
    public double pHeight;

    public double damageCaused = 1;
    public int defaultCollisionIgnoreGroup = 3;
    public bool ignoresCollisionResponse = false;
    public bool explosionUponDestruction = false;
    public bool canRotate = false;
    public string defaultTag = "EP";
    public Angle slateAtAngle = new Angle();
    public Shape projectileShape = null;
    public double explosionRadius = 1;
    public SoundEffect shotSound = null;

    public Action<RRProjectile> customDestructor = null;

    public Color pColor;

    private bool imageMode;
    private bool rectMode;

    public RRProjectileGenerator() : this (2, 0.1, Color.Turquoise)
    {
    }

    public RRProjectileGenerator(double radius, double mass, Color color)
    {
        rectMode = false;
        imageMode = false;

        pRadius = radius;
        pMass = mass;
        pColor = color;
    }

    public RRProjectileGenerator(double radius, double mass, string imageName, double damage)
    {
        rectMode = false;
        imageMode = true;

        pRadius = radius;
        pMass = mass;
        pImageName = imageName;

        damageCaused = damage;
    }

    public RRProjectileGenerator(double radius, double mass, Color color, double damage)
    {
        rectMode = false;
        imageMode = false;

        pRadius = radius;
        pMass = mass;
        pColor = color;

        damageCaused = damage;
    }

    public RRProjectileGenerator(double width, double height, double mass, Color color, double damage)
    {
        rectMode = true;
        imageMode = false;

        pWidth = width;
        pHeight = height;
        pMass = mass;
        pColor = color;

        damageCaused = damage;
    }

    public RRProjectileGenerator(double width, double height, double mass, string imageName, double damage)
    {
        rectMode = true;
        imageMode = true;

        pWidth = width;
        pHeight = height;
        pMass = mass;
        pImageName = imageName;

        damageCaused = damage;
    }

    public RRProjectile projectile()
    {
        RRProjectile proj;

        if (imageMode)
        {
            if (rectMode)
            {
                proj = new RRProjectile(pWidth, pHeight, pMass, pImageName);
            }
            else proj = new RRProjectile(pRadius, pMass, pImageName);
        }
        else
        {
            if (rectMode)
            {
                proj = new RRProjectile(pWidth, pHeight, pMass, pColor);
            }
            else proj = new RRProjectile(pRadius, pMass, pColor);
        }

        proj.Angle = slateAtAngle;
        proj.Damage = damageCaused;
        proj.CollisionIgnoreGroup = defaultCollisionIgnoreGroup;
        proj.IgnoresCollisionResponse = ignoresCollisionResponse;
        proj.Tag = defaultTag;
        proj.explosionUponDestruction = this.explosionUponDestruction;
        proj.explosionRadius = explosionRadius;
        proj.CanRotate = this.canRotate;
        proj.customDestroy = this.customDestructor;
        shotSound.Play();

        if (projectileShape != null) proj.Shape = projectileShape;

        return proj;

    }
}