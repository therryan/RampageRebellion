using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

/* Teemun TODO
 * - Arcade Mode: high score
 * - Enemy AI
 * - Common enemy framework
 * - Actual level design: -THR
 */
public class RREnemySpawner
{
    private string gameMode = "";
    private Thread spawnerThread;
    public bool isPaused = false;

    public RREnemySpawner(string requestedGameMode)
    {
        gameMode = requestedGameMode;

        if (gameMode == "arcade")
        {
            spawnerThread = new Thread(arcade);
        }
        else if (gameMode == "levels")
        {
            List<RRLevel> levelList = new List<RRLevel>();
            levelList = loadLevelsFromXML();
            spawnerThread = new Thread(delegate() { executeLevelList(levelList); });
        }
        else
        {
            // Throw an exception!
        }

        spawnerThread.Start();
        spawnerThread.IsBackground = true;
    }

    public void arcade()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        double multiplier = 1.0;

        RampageRebellion game = RampageRebellion.getGame();
        Jypeli.Timer t = new Jypeli.Timer();
        t.Interval = 1.0;

        t.Timeout += delegate {
            // The multiplier function
            multiplier = (Math.Pow(1.01, (double)stopwatch.ElapsedMilliseconds / 10000));
            Console.WriteLine(multiplier);
            game.Add(new SmallEnemy(inCenteredXCoordinates(RandomGen.NextInt(-300, 300)), 
                                    inRelativeYCoordinates(RandomGen.NextInt(0, 200), "Small"),
                                    multiplier));
            t.Interval = RandomGen.NextDouble(1.0, 2.0);
        };
        t.Start();

        // Stops the stopwatch when paused, and resumes it when unpaused. // DOESN'T WORK!!
        while (true)
        {
            if (isPaused) { stopwatch.Stop(); Console.WriteLine("Stopped"); }
            while (isPaused) { Thread.Sleep(1); }
            if (!isPaused) { stopwatch.Start(); Console.WriteLine("Resumed"); }
        }
    }

    private List<RRLevel> loadLevelsFromXML()
    {
        // The string to contain the XML data
        String XMLFile;

        // The type of the enemy to be created
        String enemyType = "";

        // The coordinates for the enemy
        double x = 0.0;
        double y = 0.0;

        List<RRLevel> levelList = new List<RRLevel>();

        // Read the XML data from the file
        using (StreamReader sr = new StreamReader("Content/levels.xml"))
        {
            XMLFile = sr.ReadToEnd();
        }

        using (XmlReader reader = XmlReader.Create(new StringReader(XMLFile)))
        {
            while (reader.ReadToFollowing("level"))
            {
                // Creates a new level object to which the waves are added
                RRLevel currentLevel = new RRLevel();

                // The XMLReader first reads to the first element and then to that element's siblings
                // This way it doesn't "leak" and read every element in the file
                reader.ReadToFollowing("wave");
                do
                {
                    reader.ReadToFollowing("enemy");
                    do
                    {
                        reader.MoveToFirstAttribute();      // The type of the enemy (small, medium etc.)
                        enemyType = reader.Value;
                        reader.MoveToNextAttribute();       // The x-coordinate
                        x = Convert.ToDouble(reader.Value);
                        reader.MoveToNextAttribute();       // The y-coordinate
                        y = Convert.ToDouble(reader.Value);

                        // In the relative coordinates, the top left corner of the enemy
                        // is in the top left corner of the screen when (x, y) = (0, 0)
                        x = inCenteredXCoordinates(x);
                        y = inRelativeYCoordinates(y, enemyType);

                        // Adds an enemy to the current RRLevel object
                        if (enemyType == "Small") currentLevel.addEnemy(new SmallEnemy(x, y));
                        else if (enemyType == "Medium") currentLevel.addEnemy(new MediumEnemy(x, y));
                        else if (enemyType == "Big") currentLevel.addEnemy(new BigEnemy(x, y));
                        else if (enemyType == "Boss") currentLevel.addEnemy(new BossEnemy(x, y));
                        else
                        {
                            // Throw an exception, since the type of the enemy is unknown.
                        }

                    } while (reader.ReadToNextSibling("enemy"));

                    // Notifies the Level object that we're moving to a new wave
                    currentLevel.initNewWave();

                } while (reader.ReadToNextSibling("wave"));

                // Adds the current Level object to the list of level objects
                levelList.Add(currentLevel);
                currentLevel = null;
            }
        }

        // Debugging: Reads back the structure
        /*foreach (RRLevel level in levelList)
        {
            Console.WriteLine("Level");
            level.listAll();
        }*/

        return levelList;
    }

    private void executeLevelList(List<RRLevel> levelList)
    {
        while (levelList.Count > 0)
        {
            while (levelList[0].wavesLeft() > 0)
            {
                levelList[0].spawnThisWave();

                // Wait until there are no more enemies, and then spawn the next wave.
                while (levelList[0].enemiesLeftInCurrentWave() > 0) { Thread.Sleep(1); }
                levelList[0].removeThisWave();

                // Wait for two seconds before launching the next wave.
                Thread.Sleep(2000);

                // Play wave end music
                SoundEffect waveIncoming = Jypeli.Game.LoadSoundEffect("Bomb2-Warp2");
                waveIncoming.Play();

                Thread.Sleep(2890);

                while (isPaused) { Thread.Sleep(1); }
            }

            // Remove the finished level
            levelList.RemoveAt(0);

            RampageRebellion.getGame().UpgradeMenu();
        }

        // We're done!
        RampageRebellion.getGame().GameEnd();
    }

    // The first correction term gets the enemy's center to the top left screen
    // and the second term moves the enemy so that it isn't immediately destroyed.
    private double inRelativeXCoordinates(double originalX, string enemyType)
    {
        return originalX - 517 + (getEnemySizeByType(enemyType) / 2);
    }

    private double inRelativeYCoordinates(double originalY, string enemyType)
    {
        return (-originalY) + 485 - (getEnemySizeByType(enemyType) / 2);
    }

    private double inCenteredXCoordinates(double originalX)
    {
        return originalX - 190;
    }
    private double getEnemySizeByType(string enemyType)
    {
        if (enemyType == "Small") return RampageRebellion.SMALLENEMY_SIZE;
        else if (enemyType == "Medium") return RampageRebellion.MEDENEMY_SIZE;
        else if (enemyType == "Big") return RampageRebellion.BIGENEMY_SIZE;
        else if (enemyType == "Boss") return RampageRebellion.BOSS_SIZE;
        else return 0; // Shouldn't ever happen!
    }

    private class RRLevel
    {
        private List<RRWave> waveList = new List<RRWave>();
        private RRWave currentWave = new RRWave();

        public void addEnemy(RREnemy enemy)
        {
            currentWave.addEnemy(enemy);
        }

        // Called when the parser is moving to the next wave, so the current wave should be added to the list
        public void initNewWave()
        {
            waveList.Add(currentWave);

            // Resets the wave
            currentWave = null;
            currentWave = new RRWave();
        }

        public void spawnThisWave()
        {
            if (waveList.Count > 0)
            {
                waveList[0].spawn();
            }
            else
            {
                // Throw an exception!
            }
        }

        // Mostly for debugging; should use spawnThisWave() instead
        public void spawnAllWaves()
        {
            foreach (RRWave wave in waveList)
            {
                wave.spawn();
            }
        }

        public int enemiesLeftInCurrentWave()
        {
            return waveList[0].enemiesLeft();
        }

        // Mostly for debugging purposes
        public void listAll()
        {
            foreach (RRWave wave in waveList)
            {
                Console.WriteLine("\tWave");
                wave.listAll();
            }
        }

        public int wavesLeft()
        {
            return waveList.Count;
        }

        public void removeThisWave()
        {
            waveList.RemoveAt(0);
        }
    }

    private class RRWave
    {
        private List<RREnemy> enemyList = new List<RREnemy>();

        public void addEnemy(RREnemy enemy)
        {
            enemyList.Add(enemy);
        }

        public void spawn()
        {
            RampageRebellion game = RampageRebellion.getGame();

            foreach (RREnemy enemy in enemyList)
            {
                game.Add(enemy);
            }
        }

        public int enemiesLeft()
        {
            // We must manually remove destroyed enemies
            enemyList.RemoveAll(delegate(RREnemy e)
            {
                if (e.IsDestroyed) return true;
                else return false;
            });
            return enemyList.Count;
        }

        public void listAll()
        {
            foreach (RREnemy enemy in enemyList)
            {
                Console.WriteLine("\t\tEnemy");
            }
        }
    }
}