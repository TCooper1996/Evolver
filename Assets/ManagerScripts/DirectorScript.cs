using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorScripts;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Random = UnityEngine.Random;

namespace ManagerScripts
{
    public enum GameState
    {
        //Entities should be awakening and placing their references in lists for the Director to later use to enable them
        Awakening = 0,
        //The Director should now be enabled these scripts
        Starting = 1,
        //The main game loop has begun
        Playing = 2,
        //The game has been Paused
        Paused = 3,
        //The game is in the middle of a room transition
        SceneTransition = 4
    }

    public class DirectorScript : MonoBehaviour
    {
        public static List<GameState> StateStack = new List<GameState>(){GameState.Awakening};
    
    
        [SerializeField]
        private GameObject burstPickup;
        [SerializeField]
        private GameObject sniperPickup;
        [SerializeField]
        private GameObject landminePickup;
        [SerializeField]
        private GameObject rapidPickup;
    
        [SerializeField]
        private GameObject[] upgrades;


        public static List<GameObject> persistantObjects;
        public static ShooterBehavior sh;


        private static int _depth = 1;
        public static int depth => _depth;

        private int numberOfLevels => 2 + _depth * 2;

        public enum Direction
        {
            Right = 0,
            Up = 1,
            Left = 2,
            Down = 3
        }

        private bool _paused;
        public bool paused => _paused;
        public static bool Initialized;

        public static PlayerScript Player;
        public static DirectorScript Director;

        private Tuple<int, int> _currentRoom;

        public static Tuple<int, int> CurrentRoom
        {
            get => Director._currentRoom;
            private set => Director._currentRoom = value;
        }
        private Vector3 _playerPosition;

        private List<EnemyScript> _enemyList;

        public static EnemyScript AddToEnemyList
        {
            set => Director._enemyList.Add(value);
        }

        //This list includes upgrade and pickup scripts to be enabled after director is initialized
        private List<MonoBehaviour> extraScripts;

        public static MonoBehaviour AddToExtraScripts
        {
            set => Director.extraScripts.Add(value);
        }
        private bool _macGuffinObtained;
        public static bool IsMacGuffinObtained
        {
            get
            {
                if (Director != null)
                {
                    return Director._macGuffinObtained;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool exitsPlaced;
        private AnalyticsTracker _analytics;
        public static AnalyticsTracker analytics => Director._analytics;
    
        [SerializeField]
        private GameObject exit;
        public GameObject canvas;

        [SerializeField] private GameObject tilemap;

        [SerializeField] private GameObject boundary;

        public static GameState gameState => StateStack[StateStack.Count - 1];

        private void Awake()
        {

            if (Director)
            {
                Destroy(gameObject);
                return;
            }
        
            Director = this;
            _enemyList = new List<EnemyScript>();
            extraScripts = new List<MonoBehaviour>();
            _analytics = new AnalyticsTracker();
            World.CreateLevels(10, 10, numberOfLevels);
            _currentRoom = new Tuple<int, int>(4, 4);
            World.GetRoom(_currentRoom).visited = true;
        }


        private void Start()
        {
            Data.SetDepth(_depth);
            SetDontDestroyOnLoad(gameObject);
            SoundScript.PlaySound(SoundScript.Sound.NextLevel);
            if (Player)
            {
                Player.enabled = true;
            }
            else
            {
                throw new Exception("No Player in Scene");
            }
        
            canvas.SetActive(true);
            CanvasScript.OnSceneChange();
            if (EnemyScript.EnemyList != null)
            {
                foreach (var enemy in EnemyScript.EnemyList)
                {
                    enemy.enabled = true;
                }
            }

            foreach (var extra in extraScripts)
            {
                extra.gameObject.SetActive(true);
            }
            Initialized = true;

        }

        //This method does not reset the game completely, it just loads the next world after each level is completed.
        //Here, 'game' means one level.
        public static void ResetGame()
        {
            foreach (var enemy in Director._enemyList)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            Director._enemyList = new List<EnemyScript>();
            EnemyScript.EnemyList = new List<EnemyScript>();
            PickupScript.ClearCache();
            Destroy(Player.gameObject);
            Destroy(Director.gameObject);

            Initialized = false;
            SceneManager.LoadScene("Scenes/Alpha", LoadSceneMode.Single);
        
        }

        public static void OnExit()
        {
            if (Director._macGuffinObtained)
            {
                _depth++;
                CanvasScript.UpdateStatDisplay();
                ResetGame();
                CanvasScript.UpdateChestStatus();
            }
        }
    
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneChange;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneChange;
        }


        public static void Pause()
        {
        
            SoundScript.PlaySound(SoundScript.Sound.Menu);
            if (!Director._paused)
            {
                //PushState(GameState.Paused);
                Time.timeScale = 0;
                CanvasScript.DisplayMenu();
            }
            else
            {
                //PopState(GameState.Paused);
                Time.timeScale = 1;
                CanvasScript.HideMenu();
            }

            Director._paused = !Director._paused;
            CanvasScript.PauseToggle();
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        //Returns a tuple representing a coordinate in direction d of coordinate pos
        //For example, if pos= (0, 0) and d=Right, it will return (1, 0)
        public static Tuple<int, int> GetAdjacentTuple(Tuple<int, int> pos, Direction d)
        {
            var nextX = pos.Item1 + (int) Math.Cos(Math.PI / 2 * (int) d);
            var nextY = pos.Item2 + (int) Math.Sin(Math.PI / 2 * (int) d);

            return new Tuple<int, int>(nextX, nextY);
        }

        //Returns a list of currently active enemies
        public static List<EnemyScript> GetEnemyList()
        {
            return Director._enemyList;
        }
    

        public void OnSceneChange(Scene s, LoadSceneMode l)
        {
            exitsPlaced = false;
            if (s.buildIndex != 1)
            {

                //Obtain enemy references
                _enemyList = EnemyScript.EnemyList;
                foreach (var b in ProjectileScript.bullets)
                {
                    Destroy(b);
                }
            

                //Color boundaries
                PlaceExits();
            
                //Inform Canvas of Scene change
                if (Initialized)
                {
                    CanvasScript.OnSceneChange();
                    CanvasScript.UpdateMap();
                }
            
            }

        
        }

        public static void MacGuffinObtained()
        {
            Director._macGuffinObtained = true;
            CanvasScript.UpdateChestStatus();
        }

        public static void HandleBoundaryCollision(Direction d)
        {
            if (World.RoomConnected(CurrentRoom, d))
            {
                //Update current room
                CurrentRoom = GetAdjacentTuple(CurrentRoom, d);
                World.GetRoom(CurrentRoom).visited = true;
                EnemyScript.EnemyList = new List<EnemyScript>();
            
                SceneManager.LoadScene(World.GetWorld()[CurrentRoom].SceneIndex, LoadSceneMode.Single);

                Player.transform.position = new Vector3((int) -Math.Cos(Math.PI / 2 * (int) d) * 270,
                    (int) -Math.Sin(Math.PI / 2 * (int) d) * 128, 0);
            }
        }

        public static Vector3 GetPlayerPosition()
        {
            return Player.Position;
        }

        public static void UpdateStatDisplay()
        {
            CanvasScript.UpdateStatDisplay();
        }


        public static void CreateDamagePopup(String text, Vector3 pos)
        {
        
            CanvasScript.CreateDamagePopup(text, pos, false);
        }
    
        //If fromPlayer is true, the healing pertains to the player. This is for knowing whether or not to log the healing amount
        public static void CreateHealPopup(String text, Vector3 pos)
        {
            CanvasScript.CreateDamagePopup(text, pos, true);
        }

        public static void OnEnemyDeath(EnemyScript e)
        {
        
            Director._enemyList.Remove(e);
            CanvasScript.RemoveEnemyListing(e);
            analytics.enemiesKilled++;
        }

        public static void OnPlayerDeath()
        {
            analytics.depth = _depth;
            AnalyticsTracker.instance = Director._analytics;
        
            foreach (var e in GetEnemyList().Where(e => e != null))
            {
                Destroy(e.gameObject);
            }
            ResetGameState();
            SceneManager.LoadScene("Scenes/Score", LoadSceneMode.Single);
        }

        public static void ReturnToMenu()
        {
            ResetGameState();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        
        }

        //This should be called on Objects that should persist through scenes but be destroyed when returning the the main menu 
        public static void SetDontDestroyOnLoad(GameObject g)
        {
            if (persistantObjects == null)
            {
                persistantObjects = new List<GameObject>();
            }

            DontDestroyOnLoad(g);
            persistantObjects.Add(g);
            
        }

        public static void ResetGameState()
        {
            foreach (var g in persistantObjects)
            {
                Destroy(g);
            }

            persistantObjects = new List<GameObject>();
            Initialized = false;
            _depth = 1;
            Time.timeScale = 1;
            Director._paused = false;
            CurrentRoom = null;

        }

        private void PlaceExits()
        {
            if (exitsPlaced)
            {
                return;
            }

            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var xOffset = 320;
                var yOffset = 168;
                if (World.GetRoom(CurrentRoom).HasNeighbor(d))
                {
                    var e = Instantiate(exit);
                    e.transform.localEulerAngles = new Vector3(0, 0, (int)d*90);
                    switch ((int) d)
                    {
                        //Right
                        case 0:
                            e.transform.Translate(xOffset, 0, 0, Space.World);
                            break;
                    
                        //Up
                        case 1:
                            e.transform.Translate(0, yOffset, 0, Space.World);
                            break;
                    
                        //Left
                        case 2:
                            e.transform.Translate(-xOffset, 0, 0, Space.World);
                            break;
                    
                        //Down
                        case 3:
                            e.transform.Translate(0, -yOffset, 0, Space.World);
                            break;
                        
                    }
                }
            }

            exitsPlaced = true;

        }

        public static void CreatePickup(ShooterBehavior type, Vector3 pos)
        {
            GameObject obj;

            if (type is RapidShooterBehavior)
            {
                obj = Director.rapidPickup;
            }
            else if (type is SniperShooterBehavior)
            {
                obj = Director.sniperPickup;
            }else if (type is BurstShooterBehavior)
            {
                obj = Director.burstPickup;
            }else if (type is LandmineShooterBehavior)
            {
                obj = Director.landminePickup;
            }
            else
            {
                throw new ArgumentException($"No implementation for pickup {type}");
            }

            var p =  Instantiate(obj);
            p.transform.position = pos;
            sh = Instantiate(type.gameObject).GetComponent<ShooterBehavior>();
            p.GetComponent<PickupScript>().infoBoxID = CanvasScript.AddInfoBox(sh, pos);
        }

        public static void CreateUpgrade(Vector3 pos)
        {
            var u = Instantiate(Director.upgrades[Random.Range(0, Director.upgrades.Length)]);
            u.transform.position = pos;
        
        }

        public static void OnPlayerMove()
        {
            CanvasScript.RefreshTargetIndicators();
            foreach (var e in GetEnemyList())
            {
                e.TryLock();
            }
        }

        //Create the chest at the given position
        //Used when bosses are killed
        public static void CreateChest(Vector3 pos)
        {
            var c = Instantiate(Resources.Load("chest")) as GameObject;
            c.transform.position = pos;
        }
    
    
    }
}