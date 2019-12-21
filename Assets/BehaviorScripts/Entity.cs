using System;
using ManagerScripts;
using UnityEngine;
using Utilities;
using Random = System.Random;

namespace BehaviorScripts
{
    public abstract class Entity : MonoBehaviour
    {
        protected CombatantBehavior combatantComponent;
        [HideInInspector]
        public ShooterBehavior shooterComponent;
        [SerializeField]
        public GameObject model;

        private SpriteRenderer sprite;
        

        //public float MaxHea
        public float health => combatantComponent.health;

        public float maxHealth => combatantComponent.maxHealth;
        //The below property will automatically be adjusted according to the depth for the enemyScript.
        //The value for the player may also be adjusted.
        public abstract float healthMultiplier { get; }
        public bool isAlive => combatantComponent.health > 0;
        public abstract bool isFriendly { get; }
        
        [SerializeField]
        protected float damageIntakeMultiplier = 1;
        public float DamageIntakeMultiplier => damageIntakeMultiplier;

        public float AttackDelay => shooterComponent.attackTime;

        public int damageAdditive = 0;
        public virtual int Damage => shooterComponent.damage;
        

        public abstract float BulletForceMultiplier { get; }
        public abstract float AttackSpeedMultiplier { get; set; }
        public abstract bool IsAttacking { get; }
        public float AttackRatio => shooterComponent.attackRatio;
        public abstract void TryLock();
        public Entity Target { get; protected set; }
        public Vector2 TargetPosition { get; protected set; }
        public float TargetDistance { get; protected set; }

        public bool HasTarget => (Target && Target.isAlive);

        public float Rotation => (model.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
        public abstract Vector3 Position { get; }
        public int Depth => DirectorScript.depth;

        protected void Awake()
        {
            sprite = model.GetComponent<SpriteRenderer>();
            combatantComponent = transform.GetComponentInChildren<CombatantBehavior>();
            shooterComponent = transform.GetComponentInChildren<ShooterBehavior>();
            if (!DirectorScript.Initialized)
            {
                enabled = false;
            }
        }

        public void SetColor(float r)
        {
            sprite.color = new Color(1, r, r);
        }

        public abstract void OnCollision(Collision2D other);

        protected void OnHit(int pDamage)
        {
            
            DirectorScript.CreateDamagePopup(Math.Abs(pDamage).ToString(), Position);
            if (UnityEngine.Random.Range(0, 1) < .05)
            {
                SoundScript.PlaySound(SoundScript.Sound.Struck);
            }
        }

        protected void OnHeal(int pHeal)
        {
            DirectorScript.CreateHealPopup(Math.Abs(pHeal).ToString(), Position);
            SoundScript.PlaySound(SoundScript.Sound.Item);
        }

        public abstract void OnDeath();

        protected void TargetAcquired()
        {
            CanvasScript.TargetAdded(this);
        }

        protected void TargetLost()
        {
            CanvasScript.TargetLost(this);
        }
        //public float targetDistance => shooterComponent.GetLockedTargetDistance();
    }

}