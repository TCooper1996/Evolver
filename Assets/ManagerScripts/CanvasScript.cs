using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorScripts;
using ManagerScripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CanvasScript : MonoBehaviour
{

    private static PlayerScript player => DirectorScript.Player;
    
    public static CanvasScript canvas;
    public static Action OnInitialize;

    //Keeps track of the target indicator object for each entity
    private static Dictionary<Entity, GameObject> _entityTargetDictionary = new Dictionary<Entity, GameObject>();
    //Keep track of which enemies are currently targeting the player
    private static List<Entity> enemiesTargetingPlayer = new List<Entity>();
    
    [SerializeField]
    private GameObject TargetIndicator;
    [SerializeField]
    private GameObject PlayerTargetIndicator;
    private GameObject _targetIndicatorParent;
    [SerializeField]
    private Text _reloadLabel;

    [SerializeField]
    private Image imgChest;
    [SerializeField]
    private Sprite checkSprite;
    [SerializeField]
    private Sprite xSprite;

    [SerializeField] private Image attackFillImage;


    private GameObject _popupParent;
    [FormerlySerializedAs("DamagePopupController")] [FormerlySerializedAs("_damagePopupController")] [SerializeField]
    private GameObject damagePopupController;
    private GameObject[] damagePopups = new GameObject[50];
    [FormerlySerializedAs("_healPopupController")] [SerializeField]
    private GameObject healPopupController;
    private GameObject[] healPopups = new GameObject[50];

    [SerializeField]
    private ExitScript exitAnimator;
    

    public GameObject btnMenu;
    public GameObject btnResume;
    [FormerlySerializedAs("PlayGUI")] public GameObject playGui;
    [FormerlySerializedAs("PauseGUI")] public GameObject pauseGui;

    [SerializeField]
    private MapScript map;


    private const String HealthLblPrefix = "Base Health: ";
    private const String BulletSpeedMultiplierLblPrefix = "Bullet speed multiplier: ";
    private const String BaseAttackSpeedMultiplierLblPrefix = "Base attack/reload speed multiplier: ";
    private const String BaseDamageLblPrefix = "Base Damage bonus: ";
    private const String WeaponTypeLblPrefix = "Weapon Type: ";
    private const String WeaponAttackSpeedMultiplierLblPrefix = "Attack/Reload Speed multiplier: ";
    private const String WeaponDamageMultiplier = "Weapon damage multiplier: ";
    private const String FinalAttackSpeedLblPrefix = "Final Attack Delay: ";
    private const String BulletsFiredLblPrefix = "Bullets Fired: ";
    
    [FormerlySerializedAs("_lblDepth")] [SerializeField]
    private Text lblDepth;

    //[SerializeField] private GameObject text;

    
    //Base stat labels
    [SerializeField] private Text lblBaseAttackSpeedMultiplier;
    [SerializeField] private Text lblHealth;
    [SerializeField] private Text lblDamageAdditive;
    [SerializeField] private Text lblFinalAttackSpeed;

    
    //Weapon stat labels
    [SerializeField] private Text lblWeaponType;
    [SerializeField] private Text lblWeaponAttackSpeedMultiplier;
    [SerializeField] private Text lblWeaponDamageMultiplier;
    [SerializeField] private Text lblWeaponBulletSpeedMultiplier;
    [SerializeField] private Text lblWeaponBulletsFired;
    
    [FormerlySerializedAs("_sliderAttackTime")] [SerializeField]
    private Slider sliderAttackTime;
    [FormerlySerializedAs("_sliderAbsorbCooldown")] [SerializeField]
    private Slider sliderAbsorbCooldown;
    [SerializeField]
    private Image sliderImage;

    private RectTransform _canvasRect;
    private Vector2 _viewportPosition;

    //private List<Text> _labels;
    private Dictionary<EnemyScript, Image> enemyAttackTimeImages;
    [SerializeField]
    private GameObject InfoBoxParent;
    [SerializeField]
    private GameObject infoBox;

    //Contains the infoBoxes for all enemies in the scene
    private List<EnemyInfoScript> infoBoxes;
    private int infoBoxSelection;


    private void Awake()
    {
        if (canvas != null)
        {
            Destroy(gameObject);
            return;
        }
        
        canvas = this;
        
        //enemyInfoObjects = new List<EnemyInfoScript>();
        enemyAttackTimeImages = new Dictionary<EnemyScript, Image>();
        _canvasRect = gameObject.GetComponent<RectTransform>();

        if (OnInitialize != null)
            OnInitialize();

        _popupParent = new GameObject();
        _popupParent.transform.parent = transform;
        _popupParent.transform.localScale = new Vector3(1, 1, 1);
        for (int i = 0; i < damagePopups.Length; i++)
        {
            damagePopups[i] = Instantiate(damagePopupController, _popupParent.transform);
            healPopups[i] = Instantiate(healPopupController, _popupParent.transform);
        }
        
        
        gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateStatDisplay();
        UpdateChestStatus();
        sliderAbsorbCooldown.value = 1;

    }

    // Update is called once per frame
    private void Update()
    {
        sliderAttackTime.value = 1 - DirectorScript.Player.AttackRatio;

        foreach (var e in DirectorScript.GetEnemyList())
        {
            enemyAttackTimeImages[e].fillAmount = 1 - e.AttackRatio;
        }
        
    }

    public static void PauseToggle()
    {
    }

    
    public static void UpdateStatDisplay()
    {
        canvas.lblDepth.text = "Depth: " + DirectorScript.depth;
        canvas.lblBaseAttackSpeedMultiplier.text = BaseAttackSpeedMultiplierLblPrefix + player.AttackSpeedMultiplier;
        canvas.lblDamageAdditive.text = BaseDamageLblPrefix + player.damageAdditive;
        canvas.lblHealth.text = $"{HealthLblPrefix} {Math.Round(player.health, 2)} / {player.maxHealth}";
        UpdatePlayerWeapon();
        canvas.lblWeaponAttackSpeedMultiplier.text = WeaponAttackSpeedMultiplierLblPrefix + player.shooterComponent.attackSpeedMultiplier;
        canvas.lblWeaponBulletSpeedMultiplier.text = BulletSpeedMultiplierLblPrefix + player.BulletForceMultiplier;
        canvas.lblWeaponDamageMultiplier.text =
            WeaponDamageMultiplier + player.shooterComponent.damageMultiplier;
        canvas.lblFinalAttackSpeed.text = FinalAttackSpeedLblPrefix + Math.Round(player.AttackDelay, 2) + " seconds";
        canvas.lblWeaponBulletsFired.text = BulletsFiredLblPrefix + player.shooterComponent.bulletsPerShot;
    }

    public static void OnSceneChange()
    {
        canvas.UpdateEnemyListing();
        enemiesTargetingPlayer = new List<Entity>();
        UpdateChestStatus();
        UpdateMap();
        canvas._targetIndicatorParent = new GameObject();
        UpdateTargetIndicatorDictionary();
        for (int i = 0; i < canvas.damagePopups.Length; i++)
        {
            canvas.damagePopups[i].SetActive(false);
            canvas.healPopups[i].SetActive(false);
        }
    }
    public void UpdateEnemyListing()
    {
        //Set count to 0 in EnemyInfoScript to refresh the IDs. Optional.
        EnemyInfoScript.Count = 0;
        
        //Destroy old info boxes
        foreach (Transform t in InfoBoxParent.transform)
        {
            Destroy(t.gameObject);
        }
        //Create new info boxes
        var count = DirectorScript.GetEnemyList().Count;
        infoBoxes = new List<EnemyInfoScript>(count);
        for (int i = 0; i < count; i ++)
        {
            var e = DirectorScript.GetEnemyList()[i];
            infoBoxes.Add(Instantiate(infoBox, InfoBoxParent.transform).GetComponent<EnemyInfoScript>());
            infoBoxes[i].Bind(e);
            e.infoBoxID = infoBoxes[i].ID;

        }
        //Sort the info boxes by the x axis (the comparator is implemented in the EnemyInfoScript.cs) so that selecting
        //each enemy is intuitive
        infoBoxes.Sort();
        infoBoxSelection = 0;
        if(count > 0)
            infoBoxes[0].gameObject.SetActive(true);
        
        //Destroy old enemy health images
        foreach (var i in enemyAttackTimeImages.Values)
        {
            Destroy(i.gameObject);
        }
        enemyAttackTimeImages = new Dictionary<EnemyScript, Image>();
        
        //Create new enemy health images
        foreach (var e in DirectorScript.GetEnemyList())
        {
            enemyAttackTimeImages[e] = Instantiate(attackFillImage, transform);
            enemyAttackTimeImages[e].transform.position = e.Position;
        }
        
    }

    //Adds an infoBox for a dropped weapon
    public static int AddInfoBox(ShooterBehavior s, Vector3 pos)
    {
        var info = Instantiate(canvas.infoBox, canvas.InfoBoxParent.transform).GetComponent<EnemyInfoScript>();
        canvas.infoBoxes.Add(info);
        info.Bind(s, pos);
        canvas.infoBoxes.Sort();
        return info.ID;
    }


    public static void RemoveInfoBox(int pId)
    {
        var box = canvas.infoBoxes.Find(x => x.ID == pId);
        canvas.infoBoxes.Remove(box);
        Destroy(box.gameObject);
    }


    private static void UpdateTargetIndicatorDictionary()
    {
        _entityTargetDictionary = new Dictionary<Entity, GameObject>();
        _entityTargetDictionary[DirectorScript.Player] = Instantiate(canvas.PlayerTargetIndicator, canvas._targetIndicatorParent.transform);
        foreach (var e in DirectorScript.GetEnemyList())
        {
            _entityTargetDictionary[e] = Instantiate(canvas.TargetIndicator, canvas._targetIndicatorParent.transform);
        }
    }

    public static void CreateDamagePopup(string damage, Vector3 pos, bool isHeal)
    {
        var array = isHeal ? canvas.healPopups : canvas.damagePopups;
        var textPos = new Vector2(pos.x + Random.Range(-5f, 5f), pos.y + Random.Range(-5f, 5f));
        GameObject obj = null;
        foreach (var d in array)
        {
            if (!d.activeSelf)
            {
                d.SetActive(true);
                obj = d;
                break;
            }
        }

        if (obj)
        {
            obj.transform.position = new Vector3(textPos.x, textPos.y, 0);
            obj.GetComponent<PopupDamageScript>().SetText(damage, Int32.Parse(damage) < 0);
        }
    }

    public static void UpdatePlayerWeapon()
    {
        String weaponType;
        switch (PlayerScript.ShooterComponent)
        {
            case RapidShooterBehavior _:
                weaponType = "Rapid";
                break;
            
            case SniperShooterBehavior _:
                weaponType = "Sniper";
                break;
            
            case BurstShooterBehavior _:
                weaponType = "Burst";
                break;
            
            case LandmineShooterBehavior _:
                weaponType = "Landmine";
                break;
            
            default:
                throw new Exception($"Invalid Shooter Component type when setting weapon label: {PlayerScript.ShooterComponent}");
            
        }
        canvas.lblWeaponType.text = $"{WeaponTypeLblPrefix} {weaponType}";
    }

    public void Quit()
    {
        DirectorScript.Quit();
    }

    public void Unpause()
    {
        DirectorScript.Pause();
    }

    public void QuitToMenu()
    {
        DirectorScript.ReturnToMenu();
    }

    public static void DisplayMenu()
    {
        
        canvas.playGui.SetActive(false);
        canvas.pauseGui.SetActive(true);
        canvas.StartCoroutine(ColorCurrentRoom());
        UpdateStatDisplay();
        canvas.btnResume.GetComponent<Button>().Select();
        canvas.lblHealth.text = $"{HealthLblPrefix} {Math.Round(DirectorScript.Player.health, 2)} / {DirectorScript.Player.maxHealth}";
    }

    public static void HideMenu()
    {
        canvas.playGui.SetActive(true);
        canvas.pauseGui.SetActive(false);
        canvas.StopCoroutine(ColorCurrentRoom());
    }

    public static void UpdateChestStatus()
    {
        
        canvas.imgChest.sprite = (DirectorScript.IsMacGuffinObtained) ? canvas.checkSprite : canvas.xSprite;
    }


    public static void UpdateMap()
    {
        canvas.map.UpdateMap();
    }

    public static void RemoveEnemyListing(EnemyScript e)
    {
        //Find the infobox associated with this enemy and remove it
        RemoveInfoBox(e.infoBoxID);
            
        Destroy(canvas.enemyAttackTimeImages[e]);
        canvas.enemyAttackTimeImages.Remove(e);

        if (canvas.infoBoxes.Count > 0)
        {
            canvas.infoBoxSelection = 0;
            canvas.infoBoxes[0].gameObject.SetActive(true);
        }

    }
    
    //Reposition the target indicators below the players new position
    public static void RefreshTargetIndicators()
    {
        foreach (var e in enemiesTargetingPlayer)
        {
            var tarInd = _entityTargetDictionary[e];
            tarInd.transform.position = player.Position;
            var relativePos = e.TargetPosition.normalized * 20;
            tarInd.transform.Translate(-relativePos.x, -relativePos.y, 0, Space.World);

            var angleDegrees = (float)Math.Atan2(-relativePos.y, -relativePos.x) * Mathf.Rad2Deg + 90;
            tarInd.transform.localEulerAngles = new Vector3(0, 0, angleDegrees);
        }
    }

    public static void TargetAdded(Entity e)
    {
        enemiesTargetingPlayer.Add(e);
        var tarInd = _entityTargetDictionary[e];
        tarInd.SetActive(true);
        tarInd.transform.position = player.Position;
        var relativePos = e.TargetPosition.normalized * 20;
        tarInd.transform.Translate(-relativePos.x, -relativePos.y, 0, Space.World);

        var angleDegrees = (float)Math.Atan2(-relativePos.y, -relativePos.x) * Mathf.Rad2Deg + 90;
        tarInd.transform.localEulerAngles = new Vector3(0, 0, angleDegrees);

    }

    public static void TargetLost(Entity e)
    {
        _entityTargetDictionary[e].SetActive(false);
        enemiesTargetingPlayer.Remove(e);
    }

    public static void UpdatePlayerTarget()
    {
        var pTarget = player.Target;
        var tarInd = _entityTargetDictionary[player];
        if (pTarget && pTarget.isAlive)
        {
            tarInd.SetActive(true);
            tarInd.transform.position = player.Target.Position;
            tarInd.transform.Translate(0, 0, -1);
        }
        else
        {
            tarInd.SetActive(false);
            
        }
    }

    public static void UpdatePlayerAbsorbSlider()
    {
        var v = (float) PlayerScript.absorbUsageRatio;
        canvas.sliderAbsorbCooldown.value = v;
        if (v <= 0.3f)
        {
            canvas.sliderImage.color = new Color(40, 97, 0);
        }
        else
        {
            canvas.sliderImage.color = new Color(0, 250, 0);
        }
    }

    private static IEnumerator ColorCurrentRoom()
    {
        double t = 0;
        var room = MapScript.CurrentRoom.GetComponent<Image>();
        while (room)
        {
            t += Time.deltaTime;
            room.color = new Color(0, 0, 0.5f + (float)Math.Cos(t*4)*0.5f);
            if (t >= Math.PI / 2)
                t = 0;
            
            yield return null;//new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DisplayReloadLabelCoroutine()
    {
        var duration = 2f;
        _reloadLabel.enabled = true;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            _reloadLabel.color =  new Color(1, 0, 0, duration/2f);
            yield return null;
        }

        _reloadLabel.enabled = false;
    }

    public static void DisplayReloadLabel()
    {
        canvas.StopCoroutine(nameof(DisplayReloadLabel));
        canvas.StartCoroutine(nameof(DisplayReloadLabelCoroutine));
    }

    public static void SelectNextEnemy()
    {
        //Keep track of how many infoBoxes are check. If they have all been found null, break.
        if (canvas.infoBoxes.Count > 0)
        {
            var index = canvas.infoBoxSelection;
            canvas.infoBoxes[index].gameObject.SetActive(false);
            canvas.infoBoxSelection = (index + 1) % canvas.infoBoxes.Count;
            canvas.infoBoxes[canvas.infoBoxSelection].gameObject.SetActive(true);
        }
    }

    public static void SelectPreviousEnemy()
    {
        
        if (canvas.infoBoxes.Count > 0)
        {
            var index = canvas.infoBoxSelection;
            canvas.infoBoxes[index].gameObject.SetActive(false);
            canvas.infoBoxSelection = (index + canvas.infoBoxes.Count - 1) % canvas.infoBoxes.Count;
            canvas.infoBoxes[canvas.infoBoxSelection].gameObject.SetActive(true);
        }
    }
    
    
}