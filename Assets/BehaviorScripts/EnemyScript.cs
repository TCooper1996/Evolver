using System;
using System.Collections.Generic;
using BehaviorScripts;
using BehaviorScripts.MovementBehaviors;
using BehaviorScripts.ProjectileBehaviors;
using ManagerScripts;
using UnityEngine;
using Random = UnityEngine.Random;
using RoomPositionTuple = System.Tuple<System.Tuple<int, int>, UnityEngine.Vector3>;

namespace BehaviorScripts
{
    public class EnemyScript : Entity
    {
        private static Dictionary<RoomPositionTuple, bool> _eliteKillHistory;
        private bool _active = true;
        //if this field is true, the enemy must be killed to obtain the macguffin
        [SerializeField]
        private bool chestBoss;
    
        private TurretMovementBehavior _movementComponent;

        [SerializeField] private GameObject skullPrefab;

        [SerializeField] private bool elite;
        public bool isElite => elite;
    
        public override bool isFriendly => false;
        public override bool IsAttacking => true;


        private Vector3 _position;
        public override Vector3 Position => _position;

        private float _healthMultiplier = 1;
        public override float healthMultiplier => _healthMultiplier;
        private int _damageMultiplier = 1;
        public override int Damage => shooterComponent.damage * _damageMultiplier;


        public override float BulletForceMultiplier { get => shooterComponent.bulletSpeedMultiplier; }

        private float _attackSpeedMultiplier = 1;
        public override float AttackSpeedMultiplier
        { 
            get => _attackSpeedMultiplier;
            set => _attackSpeedMultiplier = value; 
        }

        private const float WeaponDropChance = 0.3f;
        private const float UpgradeDropChance = 0.15f;


        private RoomPositionTuple _identifier; //Uniquely dentifies an enemy by it's position and room
        private static List<EnemyScript> _enemyList = new List<EnemyScript>();
        public static List<EnemyScript> EnemyList
        {
            get => _enemyList;
            set => _enemyList = value;
        }

        public int infoBoxID;

        private new void Awake()
        {
            if (_enemyList == null)
            {
                _enemyList = new List<EnemyScript>();
            }

            if (elite)
            {
                _identifier = new RoomPositionTuple(DirectorScript.CurrentRoom, _position);
                if (EliteKilled(_identifier))
                {
                    Destroy(gameObject);
                    return;
                }
                Instantiate(skullPrefab, transform);

            }

            _healthMultiplier = (float)Data.healthSample;
            _attackSpeedMultiplier /= (float)Data.attackTimeSample;
            _damageMultiplier = (int)Data.damageSample;
        
            _movementComponent = GetComponentInChildren<TurretMovementBehavior>();
            _position = model.transform.position;
            _enemyList.Add(this);
            base.Awake();
        }

        private void FixedUpdate()
        {
            if (_active)
                _movementComponent.Move(Time.deltaTime);
        }

        public override void OnCollision(Collision2D other)
        {
            switch (other.gameObject.GetComponent<MonoBehaviour>())
            {
                case ProjectileScript bullet:
                    //Landmines do not hurt entities; only their explosions do.
                    if (bullet is LandmineBehavior)
                    {
                        break;
                    }
                    combatantComponent.HandleHit(bullet);
                    OnHit(bullet.damage);
                    break;
            }
        }

    
        public void SetRotation(Quaternion q)
        {
            model.transform.rotation = q;
        }

        public void SetRotation(float zAngle)
        {
            var e = transform.rotation.eulerAngles;
            model.transform.eulerAngles = new Vector3(e.x, e.y, Mathf.Rad2Deg * (zAngle - (float)Math.PI/2));
        }


        public override void TryLock()
        {
        
            var targetPreviouslyLocked = Target;
            var relativePosition = shooterComponent.CalculateTargetVector();

            var distance = relativePosition.magnitude;
        
            var minRange = shooterComponent.MinimumLockingRange;
            var maxRange = shooterComponent.lockingRange;
            if (minRange < distance && distance < maxRange)
            {
                Target = DirectorScript.Player;
                TargetPosition = relativePosition;
                TargetDistance = distance;
            }
            else
            {
                Target = null;
                if (distance > shooterComponent.sightRange && _active)
                {
                    shooterComponent.enabled = false;
                    _active = false;
                }
                else if (distance <= shooterComponent.sightRange && !_active)
                {
                    shooterComponent.enabled = true;
                    _active = true;
                }
            }

            //Check if a target was just acquired or just lost
            if (targetPreviouslyLocked != (Target))
            {
                if (targetPreviouslyLocked)
                {
                    TargetLost();
                }
                else
                {
                    TargetAcquired();
                }
            }
        
        }

        private static bool EliteKilled(RoomPositionTuple e)
        {
            if (_eliteKillHistory == null)
            {
                _eliteKillHistory = new Dictionary<RoomPositionTuple, bool>();
            }
        
            try
            {
                return _eliteKillHistory[e];
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public override void OnDeath()
        {
            //Ensure this method is only called once
            if (!enabled)
                return;
        
            //Disable gameobject and all children to stop all behavior, including future RigidBody ticks
            //This is necessary because the gameobject won't be destroyed until all child bullets have been destroyed first.
            //gameObject.SetActive(false);
            enabled = false;
            model.SetActive(false);
            shooterComponent.enabled = false;
        
            //If this enemy is elite, mark is as being killed so it does not respawn.
            if (elite)
            {
                _eliteKillHistory[_identifier] = true;
                Destroy(transform.GetChild(4).gameObject);
            }

            if (chestBoss)
            {
                DirectorScript.CreateChest(Position);
            }
        
            //Update enemy listing
            DirectorScript.OnEnemyDeath(this);

            //Inform canvas that a target must be removed from the player
            if (Target)
            {
                TargetLost();
            }

            //If this enemy was targeted by the player, refresh player's target indicator
            if (DirectorScript.Player.Target == this)
            {
                CanvasScript.UpdatePlayerTarget();
            }
            //Calculate drop chance and potentially drop weapon.
            var rand = Random.Range(0f, 1f);
            if (rand < WeaponDropChance + 1)
            {
                shooterComponent.enabled = false;
                DirectorScript.CreatePickup(shooterComponent, transform.position);
            }else if (rand < UpgradeDropChance)
            {
                //Drop at least 1, maybe 2, or even less likely, 3
                var drops = Random.Range(1, 9);
                while (drops > 0)
                {
                    var pos = transform.position;
                    var randomPos = Random.insideUnitCircle * 20;
                    DirectorScript.CreateUpgrade(new Vector3(pos.x + randomPos.x, pos.y + randomPos.y, pos.z));
                    drops -= 3;
                }

            }

            if (shooterComponent.transform.childCount == 0)
                Destroy(gameObject);
        
        }
    
    }
}