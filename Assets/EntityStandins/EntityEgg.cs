/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using BehaviorScripts;
using UnityEngine;

namespace EntityStandins
{
    //This class is simply a stand in for an enemy that will be created at runtime.
    //Enemies have interfaced object fields that can be set to a number of different classes.
    //In order to easily set an enemy to be created from the editor, and to determine what properties and what
    //behavior classes the enemy will use, the EnemyEgg class is used. EnemyEggs will be placed in the game
    //from the editor, and will have serialized fields that determine how an enemy will be created.
    //On Start(), the EnemyEgg will create the described enemy, and destroy itself.

    public enum CombatantClass
    {
        Standard,
        
    }

    public enum ShooterClass
    {
        Burst,
        Rapid,
        Sniper,
        Landmine,
    }
    
    public class EntityEgg : MonoBehaviour
    {
        public bool isPlayer;
        public bool elite;
        public float weaponDropChance;
        public float upgradeDropChance;
        [SerializeField]
        private GameObject player;
        [SerializeField]
        private GameObject turret;
        [SerializeField] private GameObject skull;

        [Header("Combatant Class")]
        public CombatantClass combatantClass;
        [RangeAttribute(0.5f, 20)]
        public int healthMultiplier = 1;
        [RangeAttribute(0.2f, 4)]
        public float recoveryRateMultiplier = 1;
        [RangeAttribute(0.1f, 1)]
        public float recoveryThreshold = 0.3f;
        
        [Header("Shooter Class")]
        public ShooterClass shooterClass;
        public float damageMultiplier = 1;
        [RangeAttribute(0.2f, 3f)]
        public float projectileSpeedMultiplier = 1;
        [RangeAttribute(0.2f, 15f)]
        public float attackSpeedMultiplier = 1;

        [RangeAttribute(0, 5)]
        public int additionalBullets = 0;

        [Header("Movement Class")]
        public float rotationSpeedMultiplier = 1;

        
        private GameObject m_AttackIndicator;
        private float attackIndicatorRotationPeriod;
        public GameObject targetIndicatorPrefab;
        private GameObject targetIndicatorObject;
        private Entity entityScript;
        private GameObject skullIcon;

        public static List<EntityEgg> enemyEggs;
        


        public GameObject AttackIndicator
        {
            set
            {
                if (isPlayer)
                    throw new InvalidOperationException(
                        "Attempting to set attack indicator on player, which should not have one.");
                m_AttackIndicator = value;
            }
        }

        private void Awake()
        {
            if (enemyEggs == null)
            {
                enemyEggs = new List<EntityEgg>();
            }
            if (isPlayer)
            {
                DirectorScript.playerEgg = this;
            }
            else
            {
                enemyEggs.Add(this);
            }
            gameObject.SetActive(false);
        }

        public void Start()
        {
            CombatantBehavior combatantBehavior;
            ShooterBehavior shooterBehavior;
            //GameObject prefab;
            var prefab = isPlayer ? player : turret;
            if (isPlayer && PlayerScript._Initialized)
            {
                Destroy(gameObject);
                return;
            }            
            var p = transform.position;
            var r = transform.rotation;
            targetIndicatorObject = Instantiate(targetIndicatorPrefab, transform);
            
            //Create gameobject for player/enemy
            var instance = Instantiate(prefab, p, r);
            //prefab.SetActive(false);
            entityScript = instance.GetComponent<Entity>();
            

            switch (combatantClass)
            {
                case CombatantClass.Standard:
                    combatantBehavior = new RecoveringCombatant(entityScript, healthMultiplier,
                        recoveryRateMultiplier, recoveryThreshold, isPlayer);
                    break;
                    
                default:
                    throw new InvalidEnumArgumentException($"No protocol for enum {combatantClass}");
            }

            switch (shooterClass)
            {
                case ShooterClass.Burst:
                    shooterBehavior = new BurstShooterBehavior(entityScript, damageMultiplier, attackSpeedMultiplier, projectileSpeedMultiplier, additionalBullets);
                    break;
                
                case ShooterClass.Rapid:
                    shooterBehavior = new RapidShooterBehavior(entityScript, damageMultiplier, attackSpeedMultiplier, projectileSpeedMultiplier, additionalBullets);
                    break;
                
                case ShooterClass.Sniper:
                    shooterBehavior = new SniperShooterBehavior(entityScript, damageMultiplier, attackSpeedMultiplier, projectileSpeedMultiplier);
                    break;
                
                case ShooterClass.Landmine:
                    shooterBehavior = new LandmineShooterBehavior(entityScript, damageMultiplier, attackSpeedMultiplier, projectileSpeedMultiplier);
                    break;
                    
                default:
                    throw new InvalidEnumArgumentException($"No protocol for enum {shooterClass}");
                    
            }
            
            //Inherit sprite from egg
            instance.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            //Inherit scale
            instance.transform.localScale = transform.localScale;
            
            
            //Create correct movementbehavior
            switch (entityScript)
            {
                case PlayerScript player:
                {
                    var movementBehavior = new PlayerMovement(player);
                    player.Initialize(combatantBehavior, movementBehavior, shooterBehavior, this);
                    //If this is not the first level, restore the player's max health.
                    if (DirectorScript.depth > 1)
                    {
                        player.maxHealth = DirectorScript.playerMaxHealth;
                    }
                    DirectorScript.SetDontDestroyOnLoad(gameObject);
                    break;
                }
                
                case EnemyScript enemy:
                {
                    var movementBehavior = new TurretMovementBehavior(enemy, p, rotationSpeedMultiplier);
                    enemy.Initialize(combatantBehavior, movementBehavior, shooterBehavior, this, weaponDropChance, upgradeDropChance, elite);
                    if (elite)
                    {
                        skullIcon = Instantiate(skull, transform);
                    }
                    break;
                }
            }

            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }


        private void Update()
        {
            if (!isPlayer && m_AttackIndicator)
                m_AttackIndicator.transform.RotateAround(entityScript.Position, 
                    Vector3.forward, 360 * Time.deltaTime / entityScript.AttackDelay);

            targetIndicatorObject.SetActive(false);
            try
            {
                if (entityScript.shooterComponent.HasTarget())
                {
                    targetIndicatorObject.SetActive(true);
                    var t = entityScript.Target.lockedTarget.transform;
                    targetIndicatorObject.transform.position = t.position;
                    targetIndicatorObject.transform.localScale = t.localScale;
                }
            }
            catch (NullReferenceException e)
            {
                Debug.Log("idk");
            }
        }


        public void OnDestroy()
        {
            if (m_AttackIndicator)
            {
                Destroy(m_AttackIndicator);
            }

            if (targetIndicatorObject)
            {
                Destroy(targetIndicatorObject);
            }

            if (skullIcon)
            {
                Destroy(skullIcon);
            }
            
        }

    }
}*/