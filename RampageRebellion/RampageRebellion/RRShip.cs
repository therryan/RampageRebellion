using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class RRShip : RRObject
{
    Action<RRShip, PhysicsObject> collisionFunc;
    Action<RRShip> updateMeters;

    private Object thisLock = new Object();

    public RRShip(PhysicsGame me, double width, double height, int startHP, double scoreValue, Vector spawnPosition, Action<RRShip, PhysicsObject> onCollision, Action<RRShip> updateHPMeter)
        : base(width, height)
    {
        this.Shape = Shape.Diamond;
        this.Tag = "P"; //It is a player..
        this.Health = startHP;
        this.ScoreValue = scoreValue;
        this.Position = spawnPosition;

        this.CanRotate = false;
        this.Color = Color.Red;
        this.CollisionIgnoreGroup = 2;
        this.IgnoresGravity = true;
        this.IgnoresExplosions = true;

        // We know what to do
        this.Add(new GameObject(PhysicsGame.LoadImage("PlayerSprite")));
        this.collisionFunc = onCollision;
        this.updateMeters = updateHPMeter;

        me.AddCollisionHandler<RRShip, PhysicsObject>(this, collisionHandler);

        // Call the meter handler just to reinit
        updateMeters(this);
    }

    public void collisionHandler(RRShip collidee, PhysicsObject collider)
    {
        lock (thisLock)
        {
            bool modifiedHP = false;

                if ((collider.Tag.ToString() == "B") && ((Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y)) > 600))
                {
                    Health -= 1;
                    modifiedHP = true;
                }
                else if (collider.Tag.ToString().EndsWith("E"))
                {
                    Health -= ((Health > 1) ? 2 : 1);
                    modifiedHP = true;
                }

            collisionFunc(collidee, collider);

            if (modifiedHP) updateMeters(this);
        }
    }
}

