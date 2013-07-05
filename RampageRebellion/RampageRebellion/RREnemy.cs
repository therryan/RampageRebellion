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
/// Base class for enemies.
/// </summary>
public class RREnemy : RRObject
{
    public double shootingInterval = 0;
    private PhysicsGame thisGame = null;
    private Action<PhysicsObject, PhysicsObject> onCollision;

    private Timer shootingTimer;

    public RRWeapon weapon;

    public RREnemy(double width, double height)
        : base(width, height)
    {
        this.Tag = "E";
        this.CollisionIgnoreGroup = 4;
        this.IgnoresExplosions = true;
        this.CanRotate = false;
        //this.IgnoresGravity = true;
    }

    public override void Destroy()
    {
        if (shootingTimer != null) { shootingTimer.Stop(); shootingTimer = null; }

        Explosion expl = new Explosion(100);
        expl.Position = this.Position;
        RampageRebellion.getGame().Add(expl);

        base.Destroy();
    }

    /// <summary>
    /// Creates a new RREnemy
    /// </summary>
    /// <param name="x">XPosition</param>
    /// <param name="y">YPosition</param>
    /// <param name="gameObject">Game object</param>
    /// <param name="shootingInterval">How often this object shall shoot?</param>
    /// <param name="onColl">Collision handler</param>
    public RREnemy(double x, double y, PhysicsGame gameObject, double shootingInterval, Action<PhysicsObject, PhysicsObject> onColl)
        : this(x, y)
    {
        thisGame = gameObject;
        this.shootingInterval = shootingInterval;
        this.onCollision = onColl;

        weapon = new RRWeapon(20, 20, true, 50.0, 10.0);
        weapon.randomizedAngleSlate = 10;
        this.Add(weapon);

        createShooterDelegates();

        RampageRebellion.getGame().AddCollisionHandler<RREnemy, PhysicsObject>(this, collisionOnBorders);
        SoundEffect EnemyShot = Jypeli.Game.LoadSoundEffect("BlankSound");
        weapon.projectileGenerator.ignoresCollisionResponse = true;
        weapon.projectileGenerator.shotSound = EnemyShot;
        weapon.ProjectileCollision = projectileCollisionHandler;
    }

    /// <summary>
    /// Timeout trigger. Execute whatever you need to do here
    /// </summary>
    public virtual void onTimerTimeout()
    {
        //We need to align things. So let's set the angle
        //Let's set both for the heck of it.
        weapon.Angle = this.Angle + Angle.FromDegrees(-90);
        weapon.setBaseAngle(this.Angle + Angle.FromDegrees(-90));
        PhysicsObject p = weapon.Shoot();

        //So, we might need to shoot. Shall we?
    }

    /// <summary>
    /// Creates the timer methods required for enemies shooting at their own pace
    /// </summary>

    public virtual void createShooterDelegates()
    {
        //Assume we need a delegate

        shootingTimer = new Timer();
        //shootingTimer.Interval = shootingInterval; // should get inherited from SmallEnemy!
        shootingTimer.Interval = RandomGen.NextDouble(0.7, 1.5);
        shootingTimer.Start();

        shootingTimer.Timeout += onTimerTimeout;
    }

    /// <summary>
    /// Handles collision between an enemy and the level boundary.
    /// </summary>
    /// <param name="enemy">Colliding enemy</param>
    /// <param name="target">Target of collision</param>
    public void collisionOnBorders(RREnemy enemy, PhysicsObject target)
    {
        //if (!(enemy.Tag.ToString() == "B" || target.Tag.ToString() == "B")) System.Diagnostics.Debugger.Log(0, "-", "2: " + enemy.Tag.ToString() + " on " + target.Tag.ToString() + "\n");

        if (onCollision != null) onCollision(enemy, target);

        if (target.Tag.ToString() == "B")
        {
            enemy.Destroy();
            RampageRebellion.getGame().SCOREMETER.Value -= (int)Math.Round(enemy.ScoreValue);
        }
    }

    public void projectileCollisionHandler(PhysicsObject collider, PhysicsObject collidee)
    {
        if (onCollision != null) onCollision(collider, collidee);

        if (!(collider.Tag.ToString() == "B" || collidee.Tag.ToString() == "B")) System.Diagnostics.Debugger.Log(0, "-", "1: " + collider.Tag.ToString() + " on " + collidee.Tag.ToString() + "\n");

        RRProjectile projectile = collider as RRProjectile;
        if (collidee.Tag.ToString() == "B" || collidee.Tag.ToString() == "BS") projectile.Destroy(); // If the bullet hits the wall or the bomb shockwave, destroy it
        else if (collidee.Tag.ToString() == "P") //It is a player? Oh noes!
        {
            RRShip ship = collidee as RRShip;
            ship.Health -= projectile.Damage;
            if (ship.Health < 0) ship.Health = 0;

            //System.Diagnostics.Debugger.Log(0, "Info", Convert.ToString(proj.ScoreValue) + "\n");
            //RampageRebellion.getGame().SCOREMETER.Value -= rr.ScoreValue;
            //RampageRebellion.getGame().POWERMETER.Value -= rr.PowerValue;
            RampageRebellion.getGame().meterHandlerFunc(ship);

            projectile.Destroy();
            if (ship.Health <= 0)
            {
                RampageRebellion.getGame().onPlayerDeath(ship);
            }
        }
    }
}

public class SmallEnemy : RREnemy
{
    // Base constructor requires *some* size, so give it 1.0 for now
    public SmallEnemy(double x, double y, double multiplier = 1.0)
        : base(1.0, 1.0, null, 1, null)
    {
        this.X = x;
        this.Y = y;
        this.Width = RampageRebellion.SMALLENEMY_SIZE;
        this.Height = RampageRebellion.SMALLENEMY_SIZE;
        this.Color = Color.HotPink;
        this.Mass = 20;
        this.LinearDamping = 0.92;
        this.Tag = "SE";
        this.Health = (double)RampageRebellion.SMALLENEMY_HP * multiplier;
        this.ScoreValue = (double)RampageRebellion.SMALLENEMY_SVALUE * multiplier;
        this.PowerValue = (double)RampageRebellion.SMALLENEMY_PVALUE * multiplier;
        this.shootingInterval = RandomGen.NextDouble(0.1, 3.0);
        Image smallEnemySprite = Jypeli.Game.LoadImage("SmallEnemySprite");
        this.Image = smallEnemySprite;
    }
}

public class MediumEnemy : RREnemy
{
    // Base constructor requires *some* size, so give it 1.0 for now
    public MediumEnemy(double x, double y)
        : base(1.0, 1.0, null, 1, null)
    {
        this.X = x;
        this.Y = y;
        this.Width = RampageRebellion.MEDENEMY_SIZE;
        this.Height = RampageRebellion.MEDENEMY_SIZE;
        this.Color = Color.HotPink;
        this.Mass = 50;
        this.LinearDamping = 0.92;
        this.Tag = "SE";
        this.Health = RampageRebellion.MEDENEMY_HP;
        this.ScoreValue = RampageRebellion.MEDENEMY_SVALUE;
        this.PowerValue = RampageRebellion.MEDENEMY_PVALUE;
        this.shootingInterval = 1.0;
        Image mediumEnemySprite = Jypeli.Game.LoadImage("MediumEnemySprite");
        this.Image = mediumEnemySprite;
    }
}

public class BigEnemy : RREnemy
{
    // Base constructor requires *some* size, so give it 1.0 for now
    public BigEnemy(double x, double y)
        : base(1.0, 1.0, null, 1, null)
    {
        this.X = x;
        this.Y = y;
        this.Width = RampageRebellion.BIGENEMY_SIZE;
        this.Height = RampageRebellion.BIGENEMY_SIZE;
        this.Color = Color.HotPink;
        this.Mass = 75;
        this.LinearDamping = 0.92;
        this.Tag = "LE";
        this.Health = RampageRebellion.BIGENEMY_HP;
        this.ScoreValue = RampageRebellion.BIGENEMY_PVALUE;
        this.shootingInterval = 1.0;
    }
}

public class BossEnemy : RREnemy
{
    // Base constructor requires *some* size, so give it 1.0 for now
    public BossEnemy(double x, double y)
        : base(1.0, 1.0, null, 1, null)
    {
        this.X = x;
        this.Y = y;
        this.Width = RampageRebellion.BOSS_SIZE;
        this.Height = RampageRebellion.BOSS_SIZE;
        this.Color = Color.HotPink;
        this.Mass = 5000;
        this.LinearDamping = 0.92;
        this.Tag = "LE";
        this.Health = RampageRebellion.BOSS_HP;
        this.ScoreValue = RampageRebellion.BOSS_SVALUE;
        this.PowerValue = RampageRebellion.BOSS_PVALUE;
        this.shootingInterval = 1.0;
        Image bossEnemySprite = Jypeli.Game.LoadImage("BossSprite");
        this.Image = bossEnemySprite;
    }
}