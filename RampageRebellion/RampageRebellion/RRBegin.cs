using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

/// @author     Jaakko Lipas
/// @version    Dev - 2.7.2013

/// <summary>
/// The main class of the game that uses all other classes in the project (except RRMain).
/// </summary>
public class RampageRebellion : PhysicsGame
{
    RREnemySpawner ES;

    #region Tech Data

    /**
     * Tags:
     * 
     *  B = Border
     *  P = Player
     *  SE = Consolidated Tag for Small-Class Enemies (small, medium)
     *  LE = Consolidated Tag for Large-Class Enemies (large, boss)
     *  EP = Enemy Projectile
     *  PP = Player Projectile
     *  LP = Player Laser Blaster Projectile
     *  BOMB = Player Bomb
     *  BX = Bomb Explosion
     *  ------------
     * Collision Ignore Groups
     *  1 = Walls
     *  2 = Players
     *  3 = Projectiles
     *  4 = Enemies
     *  **/

    #endregion

    #region Attributes
    /// <summary>
    /// Type of controls. Valid for a keyboard and an Xbox 360 controller.
    /// </summary>
    public enum ControllerType { Keyboard, Xbox };

    /// <summary>
    /// Meter that gauges score.
    /// </summary>
    public readonly IntMeter SCOREMETER = new IntMeter(0);
    /// <summary>
    /// Meter that gauges the player's current power.
    /// </summary>
    public readonly IntMeter POWERMETER = new IntMeter(0);
    /// <summary>
    /// Meter that gauges the player's current health.
    /// </summary>
    public readonly IntMeter HEALTHMETER = new IntMeter(0);
    /// <summary>
    /// The player's speed (X-axis).
    /// </summary>
    public readonly Vector PLAYER_SPEED_X = new Vector(750.0 * 1.5, 0.0);
    /// <summary>
    /// The player's speed (Y-axis).
    /// </summary>
    public readonly Vector PLAYER_SPEED_Y = new Vector(0.0, 750.0 * 1.5);
    /// <summary>
    /// Position of the score meter.
    /// </summary>
    public readonly Vector SMETER_POSITION = new Vector(320.0, 290.0);
    /// <summary>
    /// Position of the power meter.
    /// </summary>
    public readonly Vector PMETER_POSITION = new Vector(320.0, 210.0);
    /// <summary>
    /// Position of the health meter.
    /// </summary>
    public readonly Vector HMETER_POSITION = new Vector(252.0, 150.0);

    /// <summary>
    /// Position used for the weapon selector position
    /// </summary>
    /// 

    public readonly Vector WMETER_POSITION = new Vector(400.0, -80.0);

    /// <summary>
    /// (Base position) for arsenal upgrades
    /// </summary>
    /// 

    //REMEMBER - Base screen 1024x820
    public readonly Vector ARSENAL_BASE_POSITION = new Vector(-300, -300);


    /// <summary>
    /// Position used for respawning the player.
    /// </summary>

    public readonly Vector RESPAWN_POSITION = new Vector(-200.0, -300.0);
    /// <summary>
    /// Gravity vector.
    /// </summary>
    public readonly Vector GRAVITY = new Vector(0.0, -150.0);
    /// <summary>
    /// GameObject array that visualizes the player's current health.
    /// </summary>
    public readonly GameObject[] METER_RECTANGLES = new GameObject[PLAYER_HP];

    /// <summary>
    /// Shot power for the player's weapon.
    /// </summary>
    public const int PLAYER_WEAPONPOWER = 120;
    /// <summary>
    /// Small enemy health.
    /// </summary>
    public const double SMALLENEMY_HP = 50;
    /// <summary>
    /// Small enemy score value.
    /// </summary>
    public const double SMALLENEMY_SVALUE = 100;
    /// <summary>
    /// Small enemy power value.
    /// </summary>
    public const double SMALLENEMY_PVALUE = 1;
    /// <summary>
    /// Medium enemy health.
    /// </summary>
    public const double MEDENEMY_HP = 170;
    /// <summary>
    /// Medium enemy score value.
    /// </summary>
    public const double MEDENEMY_SVALUE = 400;
    /// <summary>
    /// Medium enemy power value.
    /// </summary>
    public const double MEDENEMY_PVALUE = 3;
    /// <summary>
    /// Big enemy health.
    /// </summary>
    public const double BIGENEMY_HP = 300;
    /// <summary>
    /// Big enemy score value.
    /// </summary>
    public const double BIGENEMY_SVALUE = 1000;
    /// <summary>
    /// Big enemy power value.
    /// </summary>
    public const double BIGENEMY_PVALUE = 6;
    /// <summary>
    /// Boss health.
    /// </summary>
    public const double BOSS_HP = 1444;
    /// <summary>
    /// Boss score value.
    /// </summary>
    public const double BOSS_SVALUE = 10000;
    /// <summary>
    /// Boss power value.
    /// </summary>
    public const double BOSS_PVALUE = 25;
    /// <summary>
    /// Player health. Also used as the value for determining the length of the player's health meter.
    /// </summary>
    public const int PLAYER_HP = 40;
    /// <summary>
    /// Player score value (the value to be deducted from the current score, if the player dies)
    /// </summary>
    public const double PLAYER_SVALUE = 4000;
    /// <summary>
    /// Radius of a small enemy.
    /// </summary>
    public const double SMALLENEMY_SIZE = 40.0;
    /// <summary>
    /// Radius of a medium enemy.
    /// </summary>
    public const double MEDENEMY_SIZE = 70.0;
    /// <summary>
    /// Radius of a big enemy.
    /// </summary>
    public const double BIGENEMY_SIZE = 110.0;
    /// <summary>
    /// Radius of a boss enemy.
    /// </summary>
    public const double BOSS_SIZE = 222.0;
    /// <summary>
    /// Enemy mass.
    /// </summary>
    public const double ENEMY_MASS = 600.0;
    /// <summary>
    /// Linear damping value for enemies.
    /// </summary>
    public const double ENEMY_DAMPING = 0.92;
    /// <summary>
    /// Fire rate of the player's weapon.
    /// </summary>
    public const double PLAYER_FIRERATE = 12;
    /// <summary>
    /// Length of a meter.
    /// </summary>
    public const double METER_LENGTH = 200.0;
    /// <summary>
    /// Length of a single bar in a health meter. Dependant on attribute METER_LENGTH.
    /// </summary>
    public const double HMETER_LENGTH = METER_LENGTH / 28;
    /// <summary>
    /// Height of a meter.
    /// </summary>
    public const double METER_HEIGHT = 25.0;
    /// <summary>
    /// Value to use when executing the tilemap.
    /// </summary>
    public const double FIELDEXEC_VALUE = 20.0;
    /// <summary>
    /// Interval between enemy spawns (in seconds).
    /// </summary>
    public const double SPAWNLOGIC_INTERVAL = 14.0;
    /// <summary>
    /// Sets the amount of enemy spawns to execute.
    /// </summary>
    public const int SPAWN_AMOUNT = 11;
    /// <summary>
    /// General initialization value for int variables.
    /// </summary>
    public const int GENERAL_INITVALUE = 0;

    private static RampageRebellion gameObject;
    private RRShip playerShip;
    private static Object upgradeLockObject = new Object();


    /// <summary>
    /// Internal variable, whenever the vibration features are enabled
    /// </summary>

    public bool vibrateEnabled = false;


    /// <summary>
    /// Primary weapon for the player. Fast but not too efficient
    /// </summary>

    public RRWeapon primaryWeapon;

    /// <summary>
    /// Secondary weapon. Not too fast to fire or to advance, but is effective
    /// </summary>
    /// 
    public RRWeapon secWeapon;

    /// <summary>
    /// Third weapon. Insensibly inaccurate shooter! >:D
    /// </summary>
    /// 
    public RRWeapon triWeapon;

    /// <summary>
    /// Bomb, that is.
    /// </summary>
    /// 
    public RRWeapon bomb;

    /// <summary>
    /// Player weapon arsenal! >:D
    /// </summary>

    public volatile RRWeaponArsenal playerWeaponArsenal;
    public volatile List<IntMeter> pwArsenalUpgrades;
    public volatile List<Label> pwArsenalLabels;
    public volatile GameObject pauseMenuDummy;
    public volatile Image pauseMenu;
    #endregion

    #region Initialization
    /// <summary>
    /// The title screen void that loads the title screen background, which contains control information.
    /// </summary>
    /// 

    public override void Begin()
    {
        gameObject = this;

        SetWindowSize(1024, 820, false);
        Image MenuBG = LoadImage("Title");
        Level.Background.Image = MenuBG;
        Level.BackgroundColor = Color.DarkGray;
        Camera.ZoomToLevel();
        Level.Background.ScaleToLevel();
        ControllerOne.Listen(Button.Start, ButtonState.Pressed, delegate { ControlMethod(); ControllerOne.Disable(Button.Start); }, null);
        ControllerOne.Listen(Button.Back, ButtonState.Pressed, ConfirmExit, null);
        Keyboard.Listen(Key.Z, ButtonState.Pressed, delegate { ControlMethod(); Keyboard.Disable(Key.Z); }, null);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, null);

        //Reset weapon upgrades here. They should not be lost upon accessing the pause menu - only when resetting!

        generateArsenalData();
    }

    /// <summary>
    /// Sets visibility for arsenal labels
    /// </summary>
    /// <param name="visible"></param>

    private void hideArsenalLabels()
    {
        pauseMenuDummy.Image = null;
        for (int i = 0; i < 4; i++) {
            pwArsenalLabels[i].Destroy();
        }

        pwArsenalLabels = null;
    }

    private void generateArsenalData()
    {
        pwArsenalUpgrades = new List<IntMeter>(new IntMeter[] { new IntMeter(0, 0, 3), new IntMeter(0, 0, 3), new IntMeter(0, 0, 3), new IntMeter(0, 0, 3) });
    }

    private void showArsenalLabels()
    {
        pwArsenalLabels = new List<Label>(new Label[] { new Label(METER_LENGTH, METER_HEIGHT), new Label(METER_LENGTH, METER_HEIGHT), new Label(METER_LENGTH, METER_HEIGHT), new Label(METER_LENGTH, METER_HEIGHT) });

        pauseMenuDummy = new GameObject(1600, 1200, Shape.Rectangle);
        pauseMenuDummy.Position = new Vector(-100.0, 150.0);
        pauseMenuDummy.Color = new Color(0, 0, 0, 0);
        pauseMenu = LoadImage("PauseBckg-WUpgrades-NoBckg");
        pauseMenuDummy.Image = pauseMenu;
        Add(pauseMenuDummy);

        for (int i = 0; i < 4; i++)
        {
            pwArsenalLabels[i].BindTo(pwArsenalUpgrades[i]);
            
            pwArsenalLabels[i].Position = new Vector(ARSENAL_BASE_POSITION.X+i*200, ARSENAL_BASE_POSITION.Y);
            pwArsenalLabels[i].Color = Color.AshGray;
            pwArsenalLabels[i].TextColor = Color.Black;

            Add(pwArsenalLabels[i]);
            pwArsenalLabels[i].IsVisible = true;
        }
    }

    /// <summary>
    /// Reads which type of control the player wants to use - Controller or Keyboard - and passes the information to the other control settings.
    /// </summary>
    public void ControlMethod()
    {
        ClearAll();
        Image CntrlBG = LoadImage("ControlScreen");
        Level.Background.Image = CntrlBG;
        Level.BackgroundColor = Color.DarkGray;
        Level.Background.ScaleToLevel();
        ControllerOne.Listen(Button.Start, ButtonState.Pressed, delegate { GameStart(ControllerType.Xbox, true); }, null);
        ControllerOne.Listen(Button.Y, ButtonState.Pressed, delegate { GameStart(ControllerType.Xbox, false); }, null);
        Keyboard.Listen(Key.Z, ButtonState.Pressed, delegate { GameStart(ControllerType.Keyboard, true); }, null);
        Keyboard.Listen(Key.X, ButtonState.Pressed, delegate { GameStart(ControllerType.Keyboard, false); }, null);
    }

    /// <summary>
    /// Get game object
    /// </summary>
    /// <returns></returns>

    public static RampageRebellion getGame()
    {
        return gameObject;
    }

    /// <summary>
    /// Loads a color tilemap to generate the field elements using subprograms. Zooms the camera accordingly. Adds menu controls.
    /// </summary>
    /// <param name="controllerType">The type of controls being used.</param>
    public void GameStart(ControllerType controllerType, bool levelsOn)
    {
        Camera.ZoomToAllObjects();

        vibrateEnabled = (controllerType == ControllerType.Xbox);

        SCOREMETER.Reset();
        POWERMETER.Reset();
        HEALTHMETER.Reset();

        ColorTileMap Field = ColorTileMap.FromLevelAsset("GameTilemap");

        Field.SetTileMethod(new Color(0, 0, 0), CreateBorder);
        Field.SetTileMethod(new Color(255, 0, 0), CreatePlayer, controllerType);
        CreateScoreMeter();
        CreatePowerMeter();

        CreateHealthMeter(GENERAL_INITVALUE);
        Field.Execute(FIELDEXEC_VALUE, FIELDEXEC_VALUE);

        Image LevelUI = LoadImage("UI");
        Level.Background.Image = LevelUI;

        // Starts enemy spawning; ES.levels() for the pre-designed levels and ES.arcade() for arcade.
        if (levelsOn) ES = new RREnemySpawner("levels");
        else ES = new RREnemySpawner("arcade");

        Gravity = GRAVITY;
    }
    #endregion

    #region In-Games
    /// <summary>
    /// A pause menu that pops up if the pause button is pressed.
    /// </summary>
    /// <param name="controllerType">The type of controls being used.</param>
    /// <param name="player">The player.</param>
    /// <param name="primaryWeapon">The player's weapon.</param>
    public void PauseMenu(PhysicsObject player, ControllerType controllerType, RRWeaponArsenal primaryWeapon)
    {
        ClearControls();

        showArsenalLabels();

        MediaPlayer.Pause();
        Pause();
        ES.isPaused = true;
        switch (controllerType)
        {
            case ControllerType.Keyboard:
                Keyboard.Listen(Key.Escape, ButtonState.Pressed, delegate { Pause(); ResetGame(); }, null);
                Keyboard.Listen(Key.P, ButtonState.Pressed, delegate { Unpause(player, controllerType, primaryWeapon); }, null);
                //JAKE: Weapon upgrade keys here
                Keyboard.Listen(Key.Q, ButtonState.Pressed, triggerUpdateKeys, null, 0);
                Keyboard.Listen(Key.W, ButtonState.Pressed, triggerUpdateKeys, null, 1);
                Keyboard.Listen(Key.E, ButtonState.Pressed, triggerUpdateKeys, null, 2);
                Keyboard.Listen(Key.R, ButtonState.Pressed, triggerUpdateKeys, null, 3);
                break;

            case ControllerType.Xbox:
                ControllerOne.Listen(Button.Back, ButtonState.Pressed, delegate { Pause(); ResetGame(); }, null);
                ControllerOne.Listen(Button.Start, ButtonState.Pressed, delegate { Unpause(player, controllerType, primaryWeapon); }, null);
                break;
        }
    }

    /// <summary>
    /// Unpauses the game and resumes music.
    /// </summary>
    /// <param name="controllerType">The type of controls being used.</param>
    /// <param name="player">The player.</param>
    /// <param name="primaryWeapon">The player's weapon.</param>
    public void Unpause(PhysicsObject player, ControllerType controllerType, RRWeaponArsenal primaryWeapon)
    {
        ClearControls();
        MediaPlayer.Resume();
        Pause();

        hideArsenalLabels();

        CreateControls(player, controllerType, primaryWeapon);

        // Informs RREnemySpawner that the game is paused
        ES.isPaused = false;
    }

    /// <summary>
    /// Clears all game data and resets the game.
    /// </summary>
    public void ResetGame()
    {
        ClearAll();
        Begin();
    }

    /// <summary>
    /// Void that creates an invisible (fully transparent) border object. The playable field is visually bounded by the background image,
    /// so there is no need to apply a visible texture or color to the borders.
    /// </summary>
    /// <param name="position">Vector location of the object, as specified in GameStart</param>
    /// <param name="width">Width of the object, as specified in GameStart</param>
    /// <param name="height">Height of the object, as specified in GameStart</param>
    public void CreateBorder(Vector position, double width, double height)
    {
        PhysicsObject border = PhysicsObject.CreateStaticObject(width, height);
        border.Position = position;
        border.Color = new Color(0, 0, 0, 0);
        border.CollisionIgnoreGroup = 1;
        border.Tag = "B";
        Add(border);
    }

    /// <summary>
    /// Void that creates the player's collision detector box and sprite, adding controls for them.
    /// </summary>
    /// <param name="position">Vector location of the object, as specified in GameStart</param>
    /// <param name="width">Width of the object, as specified in GameStart</param>
    /// <param name="height">Height of the object, as specified in GameStart</param>
    /// <param name="controllerType">The type of controls used</param>
    /// 

    public void vibrateController()
    {
        if (((ControllerOne.GetType()) == typeof(GamePad)) && vibrateEnabled)
        {
            GamePad gp = ControllerOne as GamePad;
            gp.Vibrate(0.5, 0.5, 0.0, 0.0, 0.1);
        }
    }

    public void collisionHandler(RRShip collider, PhysicsObject collidee)
    {
        //System.Diagnostics.Debugger.Log(0, "Info", collider.Tag + "on" + collidee.Tag + "\n");
        if ((String)collidee.Tag != "Z") vibrateController();
    }

    public void meterHandlerFunc(RRShip player)
    {

        for (int i = 0; i < METER_RECTANGLES.Length; i++)
        {
            METER_RECTANGLES[i].Color = ((i < player.Health) ? Color.Green : Color.Red);
        }

        if (player.Health == 0)
        {
            onPlayerDeath(player);
        }
    }

    public void onPlayerDeath(RRShip player)
    {
        if (((ControllerOne.GetType()) == typeof(GamePad)) && vibrateEnabled)
        {
            GamePad gp = ControllerOne as GamePad;
            gp.Vibrate(1, 1, 1, 1, 1);
        }

        Explosion expl = new Explosion(200);
        expl.Position = player.Position;
        Add(expl);

        player.Destroy();

        SCOREMETER.Value -= (int)player.ScoreValue;
        POWERMETER.Reset();
        ClearControls();

        Keyboard.Listen(Key.Z, ButtonState.Pressed, delegate
        {
            for (int i = 0; i < METER_RECTANGLES.Length; i++)
            {
                METER_RECTANGLES[i].Color = Color.Green;
            }
            CreatePlayer(RESPAWN_POSITION, 20, 20, ControllerType.Keyboard);
        }, null);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, delegate
        {
            for (int i = 0; i < METER_RECTANGLES.Length; i++)
            {
                METER_RECTANGLES[i].Color = Color.Green;
            }
            CreatePlayer(RESPAWN_POSITION, 20, 20, ControllerType.Xbox);
        }, null);
    }

    public void resetAllWeapons()
    {
        //Cleanup required
        this.playerShip.Remove(playerWeaponArsenal);
        playerWeaponArsenal.Destroy();

        GeneratePlayerWeapons(this.playerShip);
    }


    /// <summary>
    /// Trigger for weapon updates. Triggered in pause menu
    /// 
    /// DOCUMENTATION:
    /// 
    /// 1st upgrade level: 2x size bullet
    /// 2nd upgrade: damage OR fire rate
    /// 3rd upgrade damage OR fire rate
    /// 4rd upgrade (bomb only) - free bomb!
    /// 
    /// FOR JAKE: When adding the 4th weapon - ensure that the bomb remains to be the last of the weapons in WeaponArsenal
    /// 
    /// </summary>
    /// <param name="i">Weapon triggering parameters</param>

    //((pwArsenalUpgrades[x] >= 1) ? (2) : (1))

    public void triggerUpdateKeys(int i) {
        lock (upgradeLockObject)
        {
            int upgradeTo = pwArsenalUpgrades[i].Value + 1;
            int upgradePrice = (int)(Math.Pow(2,upgradeTo)+2); //Prices: 4, 6, 10, 18

            if (upgradeTo > 4) return; //No. No cake for you.

            if ((POWERMETER >= upgradePrice) && (i == (playerWeaponArsenal.Weapons.Count-1)) && (upgradeTo == 4)) {
                POWERMETER.Value -= upgradePrice; //Special case
                playerWeaponArsenal.Weapons[i].Ammo.AddValue(1);
                resetAllWeapons();
            } else if ((POWERMETER.Value >= upgradePrice) && (upgradeTo < 4))
            {
                POWERMETER.Value -= upgradePrice;
                pwArsenalUpgrades[i].Value++;
                resetAllWeapons();
            }
            else return;
        }
    }

    /// <summary>
    /// Triggers the bomb - aka THE LAST of all weapons, as expected so far!
    /// </summary>

    public void triggerBomb()
    {
        playerWeaponArsenal.singleShoot(playerWeaponArsenal.Weapons.Count - 1);
    }


    public void CreatePlayer(Vector position, double width, double height, ControllerType controllerType)
    {
        // Replaced by RRShip
        /*RRObject player = new RRObject(width, height);
        player.Tag = "P";
        player.Shape = Shape.Diamond;
        player.Health = PLAYER_HP;
        player.ScoreValue = PLAYER_SVALUE;
        player.Position = position;
        player.CanRotate = false;
        player.Color = Color.Red;
        player.CollisionIgnoreGroup = 2;
        Add(player);
        player.Add(new GameObject(LoadImage("PlayerSprite")));
        player.IgnoresGravity = true;
        AddCollisionHandler<RRObject, PhysicsObject>(player, PlayerCollision);*/

        playerShip = new RRShip(this, width, height, PLAYER_HP, PLAYER_SVALUE, position, collisionHandler, meterHandlerFunc);
        Add(playerShip);
        HEALTHMETER.Value = PLAYER_HP;

        //Let's generate this first..

        GeneratePlayerWeapons(playerShip);

        CreateControls(playerShip, controllerType, playerWeaponArsenal);
    }

    /// <summary>
    /// Generates weapons for the player's use
    /// </summary>
    /// <param name="ship"></param>

    private void GeneratePlayerWeapons(RRShip ship)
    {
        SoundEffect WeaponFire = LoadSoundEffect("Shot");
        SoundEffect WeaponFireLarge = LoadSoundEffect("LargeShot");

        // Primary Weapon (Plasma Cannon): all-purpose

        //Upgrade levels. Same for each weapon for now
        double pwSizeMultiplier = ((pwArsenalUpgrades[0] >= 1) ? (2) : (1));
        double pwFireBonusMultiplier = 1 + (2 * ((pwArsenalUpgrades[0] >= 3) ? (1) : (0)));
        double pwDamageBonusMultiplier = 1 + (1.5 * ((pwArsenalUpgrades[0] >= 2) ? (1) : (0)));

        primaryWeapon = new RRWeapon(5, 10, true, PLAYER_WEAPONPOWER, PLAYER_FIRERATE * pwFireBonusMultiplier, new RRProjectileGenerator(14 * pwSizeMultiplier, 14 * pwSizeMultiplier, 0.1, new Color(55, 255, 128, 128), 10 * pwDamageBonusMultiplier));
        primaryWeapon.Position += new Vector(0, 30);
        primaryWeapon.projectileGenerator.projectileShape = Shape.Diamond;
        primaryWeapon.projectileGenerator.defaultCollisionIgnoreGroup = 3;
        primaryWeapon.projectileGenerator.defaultTag = "PP";
        primaryWeapon.projectileGenerator.canRotate = false;
        primaryWeapon.projectileGenerator.shotSound = WeaponFire;
        primaryWeapon.Color = new Color(0, 0, 0, 0);
        primaryWeapon.Angle += Angle.FromDegrees(90);

        // Secondary Weapon (Laser Blaster): laser, pierces smaller enemies

        double swSizeMultiplier = ((pwArsenalUpgrades[1] >= 1) ? (2) : (1));
        double swFireBonusMultiplier = 1 + (2 * ((pwArsenalUpgrades[1] >= 2) ? (1) : (0)));
        double swDamageBonusMultiplier = 1 + (2.2 * ((pwArsenalUpgrades[1] >= 3) ? (1) : (0)));

        secWeapon = new RRWeapon(5, 10, true, PLAYER_WEAPONPOWER / 2, PLAYER_FIRERATE * 10 * swFireBonusMultiplier, new RRProjectileGenerator(80 * swSizeMultiplier, 3 * swSizeMultiplier, 0.01, new Color(233, 111, 111, 128), 0.85 * swDamageBonusMultiplier));
        secWeapon.Position += new Vector(0, 60);
        secWeapon.projectileGenerator.defaultCollisionIgnoreGroup = 3;
        secWeapon.projectileGenerator.ignoresCollisionResponse = true;
        secWeapon.projectileGenerator.defaultTag = "LP";
        secWeapon.projectileGenerator.canRotate = false;
        secWeapon.projectileGenerator.shotSound = WeaponFire;
        secWeapon.Color = new Color(0, 0, 0, 0);
        secWeapon.Angle += Angle.FromDegrees(90);

        // Third Weapon (Minigun): inaccurate but powerful

        double triSizeMultiplier = ((pwArsenalUpgrades[2] >= 1) ? (2) : (1));
        double triFireBonusMultiplier = 1 + (5.2 * ((pwArsenalUpgrades[2] >= 3) ? (1) : (0)));
        double triDamageBonusMultiplier = 1 + (4.2 * ((pwArsenalUpgrades[2] >= 2) ? (1) : (0)));

        triWeapon = new RRWeapon(5, 10, true, PLAYER_WEAPONPOWER, PLAYER_FIRERATE * 5 * triFireBonusMultiplier, new RRProjectileGenerator(9 * triSizeMultiplier, 9 * triSizeMultiplier, 0.1, new Color(222, 222, 11, 128), 2.25 * triDamageBonusMultiplier));
        triWeapon.Position += new Vector(0, 35);
        triWeapon.projectileGenerator.defaultCollisionIgnoreGroup = 3;
        triWeapon.projectileGenerator.defaultTag = "PP";
        triWeapon.projectileGenerator.canRotate = false;
        triWeapon.projectileGenerator.shotSound = WeaponFire;
        triWeapon.Color = new Color(0, 0, 0, 0);
        triWeapon.Angle += Angle.FromDegrees(90);
        triWeapon.setBaseAngle(triWeapon.Angle);
        triWeapon.randomizedAngleSlate = 14 + pwArsenalUpgrades[2].Value*10.9;

        // Bomb: special weapon, deals extreme damage and clears all enemy bullets from the screen for its duration. Access the explosion's parameters from BombExplosion, under the region Colliders.

        double bombSizeMultiplier = ((pwArsenalUpgrades[3] >= 1) ? (2) : (1));
        double bombFireBonusMultiplier = 1 + (1 * ((pwArsenalUpgrades[3] >= 3) ? (1) : (0)));
        double bombDamageBonusMultiplier = 1 + (3 * ((pwArsenalUpgrades[3] >= 2) ? (1) : (0)));

        bomb = new RRWeapon(30, 30, true, PLAYER_WEAPONPOWER * 2, PLAYER_FIRERATE / 10 * bombFireBonusMultiplier, new RRProjectileGenerator(30 * bombSizeMultiplier, 30 * bombSizeMultiplier, 0.2, new Color(111, 111, 111, 128), 50 * bombDamageBonusMultiplier));
        bomb.projectileGenerator.projectileShape = Shape.Circle;
        bomb.projectileGenerator.defaultCollisionIgnoreGroup = 3;
        bomb.projectileGenerator.defaultTag = "BOMB";
        bomb.projectileGenerator.canRotate = false;
        bomb.projectileGenerator.shotSound = WeaponFireLarge;
        bomb.Color = new Color(0, 0, 0, 0);
        bomb.Angle += Angle.FromDegrees(90);
        bomb.countInWeaponArsenal = false;

        this.playerWeaponArsenal = new RRWeaponArsenal(primaryWeapon);
        //rP = primaryWeapon;
        this.playerWeaponArsenal.ProjectileCollision = playerCollisionHandler;
        this.playerWeaponArsenal.addNewWeapon(secWeapon);
        this.playerWeaponArsenal.addNewWeapon(triWeapon);
        this.playerWeaponArsenal.addNewWeapon(bomb);
        ship.Add(playerWeaponArsenal);
        

        CreateWeaponMeter();
    }

    public void shiftWeapons()
    {
        //Attempt to shift a weapon
        playerWeaponArsenal.setNextWeapon();
    }

    /// <summary>
    /// Creates controls for the player depending on the chosen control method.
    /// </summary>
    /// <param name="player">The player to be moved</param>
    /// <param name="controllerType">The current control type</param>
    /// <param name="primaryWeapon">The player's weapon</param>
    private void CreateControls(PhysicsObject player, ControllerType controllerType, RRWeaponArsenal primaryWeapon)
    {
        ClearControls();
        vibrateEnabled = (controllerType == ControllerType.Xbox);
        switch (controllerType)
        {
            case ControllerType.Keyboard:
                Keyboard.Listen(Key.Up, ButtonState.Down, MovePlayer, null, PLAYER_SPEED_Y, player);
                Keyboard.Listen(Key.Up, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                Keyboard.Listen(Key.Down, ButtonState.Down, MovePlayer, null, -PLAYER_SPEED_Y, player);
                Keyboard.Listen(Key.Down, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                Keyboard.Listen(Key.Left, ButtonState.Down, MovePlayer, null, -PLAYER_SPEED_X, player);
                Keyboard.Listen(Key.Left, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                Keyboard.Listen(Key.Right, ButtonState.Down, MovePlayer, null, PLAYER_SPEED_X, player);
                Keyboard.Listen(Key.Right, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                Keyboard.Listen(Key.P, ButtonState.Pressed, delegate { PauseMenu(player, controllerType, primaryWeapon); }, null);
                Keyboard.Listen(Key.Z, ButtonState.Down, FireWeapon, null, primaryWeapon);
                Keyboard.Listen(Key.X, ButtonState.Pressed, triggerBomb, null);
                Keyboard.Listen(Key.LeftShift, ButtonState.Pressed, shiftWeapons, null);



                break;
            case ControllerType.Xbox:
                ControllerOne.Listen(Button.DPadUp, ButtonState.Down, MovePlayer, null, PLAYER_SPEED_Y, player);
                ControllerOne.Listen(Button.DPadUp, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                ControllerOne.Listen(Button.DPadDown, ButtonState.Down, MovePlayer, null, -PLAYER_SPEED_Y, player);
                ControllerOne.Listen(Button.DPadDown, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, MovePlayer, null, -PLAYER_SPEED_X, player);
                ControllerOne.Listen(Button.DPadLeft, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                ControllerOne.Listen(Button.DPadRight, ButtonState.Down, MovePlayer, null, PLAYER_SPEED_X, player);
                ControllerOne.Listen(Button.DPadRight, ButtonState.Released, MovePlayer, null, Vector.Zero, player);
                ControllerOne.ListenAnalog(AnalogControl.LeftStick, 0.1, movePlayerAnalog, null, player);
                ControllerOne.Listen(Button.Start, ButtonState.Pressed, delegate { PauseMenu(player, controllerType, primaryWeapon); }, null);
                ControllerOne.Listen(Button.RightTrigger, ButtonState.Down, FireWeapon, null, primaryWeapon);
                ControllerOne.Listen(Button.Y, ButtonState.Pressed, shiftWeapons, null);
                break;
            default:
                break;
        }
    }

    public void movePlayerAnalog(AnalogState al, PhysicsObject player)
    {
        double xPos = al.StateVector.X;
        double yPos = al.StateVector.Y;
        Vector pPushVector = new Vector((xPos) * PLAYER_SPEED_X.X, (yPos) * PLAYER_SPEED_Y.Y);
        player.Push(pPushVector);

        double maxVibration = 0.15;
        double maxBias = 0.5;

        double prcBias = (0.5 * Math.Abs((xPos + yPos / 2))) +
                         (0.5 * Math.Max(0, Math.Min(1, (1 - Math.Abs((player.Velocity.Magnitude / 100))))));

        //Bias the control
        double rightControllerVibr = ((maxVibration / 2) - ((maxVibration / 2 * maxBias) * xPos)) * prcBias;
        double leftControllerVibr = ((maxVibration / 2) + ((maxVibration / 2 * maxBias) * xPos)) * prcBias;

        if (((ControllerOne.GetType()) == typeof(GamePad)) && vibrateEnabled)
        {
            GamePad gp = ControllerOne as GamePad;
            gp.Vibrate(leftControllerVibr, rightControllerVibr, 0, 0, 0.1);
        }

        //player.Velocity = pPushVector;
    }

    /// <summary>
    /// Fires the primary weapon of the player.
    /// </summary>
    /// <param name="weapon">The weapon to be fired</param>
    public void FireWeapon(RRWeaponArsenal weapon)
    {
        //TODO - Clean up FireWeapon, and the superflous "weapon" clause.
        PhysicsObject projectile = playerWeaponArsenal.Shoot();
    }

    /// <summary>
    /// Applies the force vector specified in the controls to the player object.
    /// </summary>
    /// <param name="force">The force vector to be applied, specified in a previous Listen method</param>
    /// <param name="player">The player that the force vector is to be applied to, specified in a previous Listen method</param>
    public void MovePlayer(Vector force, PhysicsObject player)
    {
        player.Push(force);
        //player.Velocity = force;
    }


    public void CreateWeaponMeter()
    {
        Label weaponMeter = new Label(METER_LENGTH, METER_HEIGHT);
        weaponMeter.Position = WMETER_POSITION;
        weaponMeter.Color = Color.Azure;
        weaponMeter.BindTo(playerWeaponArsenal.currentWeaponIndexMeter);
        Add(weaponMeter);
    }

    /// <summary>
    /// Creates a score meter on the screen. Value represented by the attribute SCOREMETER.
    /// </summary>
    public void CreateScoreMeter()
    {
        Label scoreMeter = new Label(METER_LENGTH, METER_HEIGHT);
        scoreMeter.Position = SMETER_POSITION;
        scoreMeter.TextColor = Color.Green;
        scoreMeter.BindTo(SCOREMETER);
        Add(scoreMeter);
    }

    /// <summary>
    /// Creates a power meter on the screen. Value represented by the attribute POWERMETER.
    /// </summary>
    public void CreatePowerMeter()
    {
        Label powerMeter = new Label(METER_LENGTH, METER_HEIGHT);
        powerMeter.Position = PMETER_POSITION;
        powerMeter.TextColor = Color.BloodRed;
        powerMeter.BindTo(POWERMETER);
        Add(powerMeter);
    }

    /// <summary>
    /// Creates a health meter on the screen recursively. Length of the health meter is dependant on player maximum health.
    /// </summary>
    /// <param name="i"></param>
    public void CreateHealthMeter(int i)
    {
        if (i >= PLAYER_HP) return;
        METER_RECTANGLES[i] = new GameObject(HMETER_LENGTH, METER_HEIGHT);
        METER_RECTANGLES[i].Position = new Vector(HMETER_POSITION.X + (HMETER_LENGTH * i), HMETER_POSITION.Y);
        METER_RECTANGLES[i].Color = Color.Green;
        Add(METER_RECTANGLES[i]);
        CreateHealthMeter(i + 1);
    }

    public void UpgradeMenu()
    {
    }

    #endregion

    #region Colliders
    /// <summary>
    /// Handles collisions between player projectiles and other objects.
    /// </summary>
    /// <param name="projectile">A projectile fired by the player (collider)</param>
    /// <param name="target">Target of collision</param>
    public void playerCollisionHandler(PhysicsObject projectile, PhysicsObject target)
    {
        if (target.Tag.ToString() == "B" & projectile.Tag.ToString() != "BX") projectile.Destroy(); // If the bullet hits the top wall, destroy it
        else if (target.Tag.ToString() == "EP" || target.Tag.ToString() == "PP")
        {
            if (projectile.Tag.ToString() != "BOMB" || projectile.Tag.ToString() != "BE") projectile.Destroy();
            target.Destroy();
        }

        else if (target.Tag.ToString().EndsWith("E"))
        {
            RREnemy enemy = target as RREnemy;
            RRProjectile proj = projectile as RRProjectile;
            if (enemy.Health >= 0) enemy.Health -= proj.Damage;
            if (enemy.Health <= 0)
            {
                //System.Diagnostics.Debugger.Log(0, "Info", Convert.ToString(enemy.ScoreValue) + "\n");
                SCOREMETER.Value += (int)enemy.ScoreValue;
                POWERMETER.Value += (int)enemy.PowerValue;
                enemy.Destroy();
            }
            if ((projectile.Tag.ToString() == "LP" & target.Tag.ToString() != "LE") || projectile.Tag.ToString() == "BX" || projectile.Tag.ToString() == "BS") return;
            else if (projectile.Tag.ToString() == "BOMB" & (target.Tag.ToString().EndsWith("E") || target.Tag.ToString() == "B")) BombExplosion(projectile);
            projectile.Destroy();
        }
    }

    public void BombExplosion(PhysicsObject bomb)
    {
        SoundEffect BombExplosion = LoadSoundEffect("Bomb");
        BombExplosion.Play();

        RRProjectile bombExplosion = new RRProjectile(5.0, 999999999999, "explosion");
        bombExplosion.Position = bomb.Position;
        bombExplosion.Damage = 150;
        bombExplosion.Tag = "BX";
        bombExplosion.IgnoresCollisionResponse = true;
        bombExplosion.IgnoresGravity = true;
        AddCollisionHandler<RRProjectile, RREnemy>(bombExplosion, playerCollisionHandler);
        Add(bombExplosion);

        RRProjectile bombShockwave = new RRProjectile(5.0, 999999999999, new Color(212, 0, 52, 30));
        bombShockwave.Position = bomb.Position;
        bombShockwave.Damage = 0.1;
        bombShockwave.Tag = "BS";
        bombShockwave.IgnoresCollisionResponse = true;
        bombShockwave.IgnoresGravity = true;
        AddCollisionHandler<RRProjectile, RREnemy>(bombShockwave, playerCollisionHandler);
        Add(bombShockwave);

        Timer growExplosion = new Timer();
        growExplosion.Interval = 0.02;
        growExplosion.Start();
        growExplosion.Timeout += delegate 
        {
            if (bombExplosion.Width >= 2500 & bombExplosion.Height >= 2500) { growExplosion.Reset(); bombExplosion.Destroy(); bombShockwave.Destroy(); }
            else { bombExplosion.Width = bombExplosion.Width * 1.05; bombExplosion.Height = bombExplosion.Height * 1.05; bombShockwave.Width = bombShockwave.Width * 2.5; bombShockwave.Height = bombShockwave.Height * 2.5; }
        };
    }

    #endregion

    #region Enemy Logic


    /// <summary>
    /// After a successfully ran game, opens score screen and starts the program over after closing it.
    /// </summary>
    public void GameEnd()
    {
        Pause();
        ScoreList scoreList = new ScoreList(10, false, 2500);
        scoreList = DataStorage.TryLoad<ScoreList>(scoreList, "scorelist.xml");
        HighScoreWindow scoreWindow = new HighScoreWindow("High Scores", "You have made the high scores! Please enter your name: ", scoreList, SCOREMETER.Value);
        scoreWindow.Closed += delegate { DataStorage.Save<ScoreList>(scoreList, "scorelist.xml"); Pause(); ClearAll(); Begin(); };
        Add(scoreWindow);
    }
    #endregion

    #region Legacy
    // Below: legacy code to document previous logic errors

    //Old enemy generator code. Replaced by RREnemySpawner!

    /** /// <summary>
    /// Creates an enemy on the field with the given parameters. Returns the created enemy.
    /// </summary>
    /// <param name="position">Position to create enemy in</param>
    /// <param name="radius">Radius of the enemy</param>
    /// <param name="hp">Health of the enemy</param>
    /// <param name="powerValue">Power given by the enemy on death</param>
    /// <param name="scoreValue">Score given by the enemy on death</param>
    /// <returns>The created enemy.</returns>
    public RRObject CreateEnemy(Vector position, double radius, ushort hp, ushort powerValue, ushort scoreValue)
    {
        RRObject enemy = new RRObject(radius, radius);
        enemy.Tag = "E";
        enemy.Position = position;
        enemy.Color = Color.HotPink;
        enemy.CollisionIgnoreGroup = 3;
        enemy.Mass = ENEMY_MASS;
        enemy.LinearDamping = ENEMY_DAMPING;
        enemy.CanRotate = false;
        enemy.Health = hp;
        enemy.ScoreValue = scoreValue;
        enemy.PowerValue = powerValue;
        Add(enemy);
        return enemy;
    } **/

    /// <summary>
    /// Main process for enemy spawns. If this isn't called in GameStart, no enemies will spawn.
    /// REPLACED BY RREnemySpawner
    /// </summary>
    /*public void MainEnemyProcess(int spawnLogic)
    {
        for (int i = 0; i < SPAWN_COUNT.Length; i++)
        {
            SPAWN_COUNT[i] = i;
        }
        Timer spawns = new Timer();
        spawns.Interval = SPAWNLOGIC_INTERVAL;
        spawns.Start();
        spawns.Timeout += delegate
        {
            if (spawnLogic == SPAWN_COUNT[0] || spawnLogic == SPAWN_COUNT[2] || spawnLogic == SPAWN_COUNT[4] || spawnLogic == SPAWN_COUNT[5])
            {
                for (int i = 0; i < ENEMY_SPAWNS1.Length; i++)
                {
                    CreateEnemy(ENEMY_SPAWNS1[i], SMALLENEMY_SIZE, SMALLENEMY_HP, SMALLENEMY_PVALUE, SMALLENEMY_SVALUE);
                }
            }
            else if (spawnLogic == SPAWN_COUNT[1] || spawnLogic == SPAWN_COUNT[3] || spawnLogic == SPAWN_COUNT[6] || spawnLogic == SPAWN_COUNT[10])
            {
                for (int i = 0; i < ENEMY_SPAWNS2.Length; i++)
                {
                    CreateEnemy(ENEMY_SPAWNS2[i], MEDENEMY_SIZE, MEDENEMY_HP, MEDENEMY_PVALUE, MEDENEMY_SVALUE);
                }
            }
            else if (spawnLogic >= SPAWN_COUNT[7] & spawnLogic < SPAWN_COUNT[10])
            {
                for (int i = 0; i < ENEMY_SPAWNS3.Length; i++)
                {
                    CreateEnemy(ENEMY_SPAWNS3[i], BIGENEMY_SIZE, BIGENEMY_HP, BIGENEMY_PVALUE, BIGENEMY_SVALUE);
                }
            }
            else if (spawnLogic == 11)
            {
                RRObject boss = CreateEnemy(BOSS_SPAWN, BOSS_SIZE, BOSS_HP, BOSS_PVALUE, BOSS_SVALUE);
                boss.Destroyed += delegate { GameEnd(); };
            }
            spawnLogic++;
        };
    }*/

    /// <summary>
    /// Handles collisions between the player and objects that can damage the player.
    /// </summary>
    /// <param name="player">The player (collider)</param>
    /// <param name="target">Target of collision</param>
    /* public void PlayerCollision(RRObject player, PhysicsObject target)
     {
         if (player.Velocity.X + player.Velocity.Y > 200.0)
         {
             if (target.Tag.ToString() == "B")
             {
                 if (player.Health == 1) METER_RECTANGLES[0].Color = Color.Red;
                 else METER_RECTANGLES[player.Health - 1].Color = Color.Red;
                 player.Health -= 1;
             }
             else if (target.Tag.ToString() == "E")
             {
                 target.Destroy();
                 if (player.Health == 1)
                 {
                     METER_RECTANGLES[0].Color = Color.Red;
                     player.Health -= 1;
                 }
                 else METER_RECTANGLES[player.Health - 1].Color = Color.Red;
                 if (player.Health > 1)
                 {
                     METER_RECTANGLES[player.Health - 2].Color = Color.Red;
                     player.Health -= 2;
                 }
             }
             if (player.Health == 0)
             {
                 player.Destroy();
                 SCOREMETER.Value -= player.ScoreValue;
                 POWERMETER.Reset();
                 ClearControls();
                 Keyboard.Listen(Key.Z, ButtonState.Pressed, delegate
                 {
                     for (int i = 0; i < METER_RECTANGLES.Length; i++)
                     {
                         METER_RECTANGLES[i].Color = Color.Green;
                     }
                     CreatePlayer(RESPAWN_POSITION, 20, 20, ControllerType.Keyboard);
                 }, null);
                 ControllerOne.Listen(Button.A, ButtonState.Pressed, delegate
                 {
                     for (int i = 0; i < METER_RECTANGLES.Length; i++)
                     {
                         METER_RECTANGLES[i].Color = Color.Green;
                     }
                     CreatePlayer(RESPAWN_POSITION, 20, 20, ControllerType.Xbox);
                 }, null);
             }
         }
     } */

    /*
    /// <summary>
    /// Void that adds an invisible PhysicsObject entity to an 
    /// existing PhysicsObject, to serve as an origin point for Bullet entities.
    /// </summary>
    /// <param name="shooter">The entity that the origin object is to be attached to</param>
    /// <param name="controllerType">The type of controls used</param>
    public void AddWeapons(PhysicsObject shooter)
    {
        PhysicsObject origin = new PhysicsObject(shooter.Width, shooter.Height, Shape.Rectangle);
        origin.IgnoresCollisionResponse = true;
        shooter.Add(origin);
    }

    /// <summary>
    /// Uses a timer delegate to set the rate-of-fire for the player's weapon. Allows "rapid fire" controls.
    /// </summary>
    /// <param name="origin">The object to originate shots from</param>
    /// <param name="weapon">The timer to set. The timer is created in a previous Listen method</param>
    /// <param name="controlType">The type of controls used (Controller true/Keyboard false)</param>
    public void WeaponRate(PhysicsObject origin, Timer weapon, bool controlType)
    {
        weapon.Interval = 0.225;
        weapon.Timeout += delegate { FireWeapon(origin, Shape.Rectangle); };
        weapon.Start();
        if (controlType == true) { 
            ControllerOne.Listen(Button.RightTrigger, ButtonState.Released, StopRate, null, weapon); 
        }
        else { Keyboard.Listen(Key.Z, ButtonState.Released, StopRate, null, weapon); }
    }

    /// <summary>
    /// Stops the weapon timer from cycling the next shot.
    /// </summary>
    /// <param name="weapon">The timer to stop</param>
    public void StopRate(Timer weapon)
    {
        weapon.Stop();
    }

    /// <summary>
    /// Calls a subprogram which creates a Bullet at the specified location and sends it off flying.
    /// </summary>
    /// <param name="origin">The object to set origin point for</param>
    /// <param name="shape">Shape of the bullet's collision detector</param>
    public void FireWeapon(PhysicsObject origin, Shape shape)
    {
        CreateBullet(origin.X + 5, origin.Y + 10, 15.0, 4.0, shape);
        CreateBullet(origin.X - 5, origin.Y + 10, 15.0, 4.0, shape);
    }

    public void CreateBullet(double x, double y, double width, double height, Shape shape)
    {
        Bullet Weapon1 = new Bullet(width, height);
        Weapon1.Damage = 5;
        Weapon1.X = x;
        Weapon1.Y = y;
        Weapon1.Shape = shape;
        Weapon1.Hit(new Vector(0.0, 600.0));
    } 
    */
    #endregion
}
