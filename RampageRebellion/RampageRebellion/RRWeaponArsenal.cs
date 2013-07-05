using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class RRWeaponArsenal : GameObject
{

    private List<RRWeapon> weapons = new List<RRWeapon>();
    private int _currentWeaponIndex;
    private static Object lockObject = new Object();
    private static Object collisionLockObject = new Object();

    public readonly IntMeter currentWeaponIndexMeter = new IntMeter(0);
    //private static PhysicsObject trueParentObj;


    public override void Destroy()
    {
        lock (lockObject)
        {

            for (int i = 0; i < weapons.Count; i++)
            {
                this.Remove(weapons[i]);

            }
        }
        //weapons = null;
        base.Destroy();
    }

    //Overriden properties


    public List<RRWeapon> Weapons
    {
        get { return weapons; }
    }



    public int currentWeaponIndex
    {
        get { return _currentWeaponIndex; }
        set { lock (lockObject) { _currentWeaponIndex = value; } }
    }

    public bool IsReady
    {
        get { return weapons[_currentWeaponIndex].IsReady; }
    }

    public bool CanHitOwner
    {
        get { return weapons[_currentWeaponIndex].CanHitOwner; }
        set { weapons[_currentWeaponIndex].CanHitOwner = value; }
    }

    public bool InfiniteAmmo
    {
        get { return weapons[_currentWeaponIndex].InfiniteAmmo; }
        set { weapons[_currentWeaponIndex].InfiniteAmmo = value; }
    }

    public bool AmmoIgnoresGravity
    {
        get { return weapons[_currentWeaponIndex].AmmoIgnoresGravity; }
        set { weapons[_currentWeaponIndex].AmmoIgnoresGravity = value; }
    }

    public bool AmmoIgnoresExplosions
    {
        get { return weapons[_currentWeaponIndex].AmmoIgnoresExplosions; }
        set { weapons[_currentWeaponIndex].AmmoIgnoresExplosions = value; }
    }

    public TimeSpan MaxAmmoLifetime
    {
        get { return weapons[_currentWeaponIndex].MaxAmmoLifetime; }
        set { weapons[_currentWeaponIndex].MaxAmmoLifetime = value; }
    }

    public TimeSpan TimeBetweenUse
    {
        get { return weapons[_currentWeaponIndex].TimeBetweenUse; }
        set { weapons[_currentWeaponIndex].TimeBetweenUse = value; }
    }

    public SoundEffect AttackSound
    {
        get { return weapons[_currentWeaponIndex].AttackSound; }
        set { weapons[_currentWeaponIndex].AttackSound = value; }
    }

    public DoubleMeter Power 
    {
        get { return weapons[_currentWeaponIndex].Power; }
        set { weapons[_currentWeaponIndex].setPower(value); }
    }

    protected IntMeter Ammo
    {
        get { return weapons[_currentWeaponIndex].Ammo; }
        set { weapons[_currentWeaponIndex].setAmmo(value); }
    }

    public double FireRate
    {
        get { return weapons[_currentWeaponIndex].FireRate; }
        set { weapons[_currentWeaponIndex].FireRate = value; }
    }

    public CollisionHandler<PhysicsObject, PhysicsObject> ProjectileCollision = null;


    /// <summary>
    /// A "bucket" of sorts for collision handling - it is automatically added to each weapon as its collision handler
    /// </summary>
    /// <param name="p"></param>
    /// <param name="d"></param>

    public void weaponCollisionBucket(PhysicsObject p, PhysicsObject d)
    {
        if (ProjectileCollision != null)
        {
            lock (collisionLockObject)
            {
                ProjectileCollision(p, d);
            }
        }
    }

    // Constructors, overriden methods
    public RRWeaponArsenal()
        : this(1, 1)
    {
        this.IsVisible = false; //Do not show ANYTHING!
        //base.Ammo.Value = 0;
        //base.InfiniteAmmo = false;
        base.Color = Color.LimeGreen;
        //setWeapon(0);
    }

    private RRWeaponArsenal(double width, double height)
        : base(width, height)
    {
        addNewWeapon(new RRWeapon(width, height));
        setWeapon(0);
        //Power = new DoubleMeter(1);
    }

    public RRWeaponArsenal(RRWeapon bw)
        : base(1, 1)
    {
        addNewWeapon(bw);
        setWeapon(0);
        //Power = new DoubleMeter(1);
    }

    public void addNewWeapon(RRWeapon r) {
        lock (lockObject)
        {
            weapons.Add(r);
            this.Add(r);
            r.IsVisible = false;
            r.ProjectileCollision = weaponCollisionBucket;
        }
    }

    public void deleteWeapon(int i)
    {
        lock (lockObject)
        {
            RRWeapon wp = weapons[i];
            weapons.Remove(wp);
            wp.Destroy();
        }
    }



    public void resetAndAddNew(RRWeapon rrw)
    {
        lock (lockObject)
        {
            for (int i = 0; i < weapons.Count; i++) deleteWeapon(i);
        

        weapons.Add(rrw);
        this.Add(rrw);
        rrw.IsVisible = false;
        rrw.ProjectileCollision = weaponCollisionBucket;
        _currentWeaponIndex = 0;
        }
    }

    /// <summary>
    /// Internal SetWeapon - does NOT mind thread lockings - PRIVATE for a reason!
    /// </summary>
    /// <param name="dI"></param>

    private void _setWeapon(int dI)
    {
            _currentWeaponIndex = dI;

            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].IsVisible = (dI == i);

            }
    }

    /// <summary>
    /// Set a new weapon
    /// </summary>
    /// <param name="dI">Index for this specific weapon</param>

    public void setWeapon(int dI)
    {
        lock (lockObject)
        {
            _currentWeaponIndex = dI;

            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].IsVisible = (dI == i);

            }

            //Count the meter display

            int indExcluding = -1;

            for (int i = 0; i < weapons.Count; i++)
            {
                if (i > dI) break;
                if (weapons[i].countInWeaponArsenal) indExcluding++;
                
            }

            currentWeaponIndexMeter.SetValue(Math.Max(0, indExcluding));
        }
    }

    /// <summary>
    /// Activates the next ready weapon in the arsenal - or remains in the current one
    /// </summary>

    public void setNextWeapon()
    {
        lock (lockObject)
        {
            if (weapons.Count <= 1) { setWeapon(0); return; }
            int wTC = weapons.Count - 1; //How many weapons forward we need to look?
            int wCounter = 1;

            int setToThisWeapon = -1;

            while (wCounter <= wTC)
            {
                int trueIndex = (_currentWeaponIndex + wCounter) % weapons.Count;

                if (weapons[trueIndex].countInWeaponArsenal) { setToThisWeapon = trueIndex; break; }

                wCounter++;
            }

            setWeapon((setToThisWeapon == -1) ? _currentWeaponIndex : setToThisWeapon);
        }
    }

    /// <summary>
    /// Sets the next weapon - WHENEVER it is ready or not!
    /// </summary>

    public void setNextReadyWeapon()
    {
        lock (lockObject)
        {
            if (weapons.Count <= 1) { setWeapon(0); return; }
            int wTC = weapons.Count - 1; //How many weapons forward we need to look?
            int wCounter = 1;

            int setToThisWeapon = -1;

            while (wCounter <= wTC)
            {
                int trueIndex = (_currentWeaponIndex + wCounter) % weapons.Count;

                if ((weapons[trueIndex].IsReady) && weapons[trueIndex].countInWeaponArsenal) { setToThisWeapon = trueIndex; break; }

                wCounter++;
            }

            setWeapon((setToThisWeapon == -1) ? _currentWeaponIndex : setToThisWeapon);
        }
    }

    public PhysicsObject Shoot()
    {
        lock (lockObject)
        {

            if ((weapons[_currentWeaponIndex].Ammo.Value <= 0) && !weapons[_currentWeaponIndex].InfiniteAmmo) setNextReadyWeapon();
            PhysicsObject pw = weapons[_currentWeaponIndex].Shoot();

            return pw;

        }
    }

    public PhysicsObject singleShoot(int index)
    {

        int oldIndex = _currentWeaponIndex;
        PhysicsObject ss;

        lock (lockObject)
        {
            lock (collisionLockObject)
            {
                _setWeapon(index);
                ss = weapons[_currentWeaponIndex].Shoot();
                _setWeapon(oldIndex);
            }
        }

        return ss;
    }

}
