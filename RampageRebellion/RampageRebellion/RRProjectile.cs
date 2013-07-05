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
/// Base class for projectiles.
/// </summary>
public class RRProjectile : Projectile
{
    public double Damage;
    public bool explosionUponDestruction = false;
    public double explosionRadius = 1;

    public Action<RRProjectile> customDestroy;

    public RRProjectile(double width, double height, double mass, Color color)
        : base(width, height, mass, color)
    {

        this.Tag = "EP";
    }

     public override void  Destroy()
    {

        try
        {
            if (customDestroy != null)
            {
                customDestroy(this);
            }

            if (explosionUponDestruction)
            {
                Explosion expl = new Explosion(explosionRadius);
                expl.Position = this.Position;
                RampageRebellion.getGame().Add(expl);
            }
        }
        finally
        {
            base.Destroy();
        }
    }

    public RRProjectile(double width, double height, double mass, String imageName)
        : base(width, height, mass, imageName)
    {
        this.Tag = "EP";
    }

    public RRProjectile(double radius, double mass, String imageName)
        : base(radius, mass, imageName)
    {
        this.Tag = "EP";
    }

    public RRProjectile(double radius, double mass, Color color)
        : base(radius, mass, color)
    {
        this.Tag = "EP";
    }

}