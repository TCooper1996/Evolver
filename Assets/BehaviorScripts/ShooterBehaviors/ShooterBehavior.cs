using System;
using UnityEditor;
using UnityEngine;
using Utilities;
using Random = System.Random;

namespace BehaviorScripts
{
    public abstract class ShooterBehavior : MonoBehaviour
    {
        private const float MinimumAttackDelay = 0.05f;
        
        public abstract String weaponName { get; }
        //These properties will represent the immutable properties of each weapon type.
        //These should always return the same value for any subtype, regardless of existing upgrades
        //Or the multipliers of the owner Entity. They should only be changed from inside the editor
        //by directly editing the source code
        protected abstract int baseBulletsPerShot { get;}
        protected abstract float baseAttackTime { get;}
        protected abstract int baseDamage { get;}
        protected abstract int baseLockingRange { get;} //Range within which a shootercomponent will have improved attributes
        protected abstract int baseBulletSpeed { get;}
        protected abstract int baseNoise { get; } //A quantity of randomness added to each bullets. High noise means unpredictable trajectories.
        public abstract int sightRange { get; } //Range within which an enemy will shoot at the player


        //These are the mutable values that scale the above properties and are combined with the entity
        //multipliers to return the final damage, attack speed, etc
        //These values are set from the inspector individually for each entity, and should NOT change 
        //during runtime
        [SerializeField]
        public float damageMultiplier = 1;
        [SerializeField]
        public float attackSpeedMultiplier = 1;
        [SerializeField]
        protected int additionalBulletsFired = 0;
        [SerializeField]
        protected float lockingRangeMultiplier = 1;
        [SerializeField]
        public float bulletSpeedMultiplier = 1;
        [SerializeField]
        protected float noiseMultiplier = 1;

        [SerializeField] protected float sightMultiplier = 1;

        
        //These properties should be visible to the rest of the game and may return different values at runtime
        public int bulletsPerShot => Math.Max(1, baseBulletsPerShot + additionalBulletsFired);
        public virtual float attackTime => Math.Max((baseAttackTime / attackSpeedMultiplier) * Owner.AttackSpeedMultiplier, MinimumAttackDelay);
        public int damage => (int)(baseDamage * damageMultiplier + Owner.damageAdditive);
        public int lockingRange => (int)(baseLockingRange * lockingRangeMultiplier);
        public int bulletSpeed => (int) (baseBulletSpeed * bulletSpeedMultiplier);
        public virtual float attackRatio => TimeUntilAttack / attackTime;
        public float NoiseMultiplier => baseNoise * noiseMultiplier;


        //These properties return results of the above without multipliers or additions from the owner
        //Used by EnemyInfoScripts.cs
        public int uiDamage => (int) (baseDamage * damageMultiplier);
        public float uiAttackTime => Math.Max(baseAttackTime / attackSpeedMultiplier, MinimumAttackDelay);
        public float uiAttacksPerSecond => 1 / (baseAttackTime / attackSpeedMultiplier);
        public float uiDamagePerSecond => uiDamage * uiAttacksPerSecond;
        
        public Entity Owner
        {
            get;
            protected set;
        }
        public bool IsFriendly => Owner.isFriendly;
        protected Vector3 Position => Owner.Position;
        protected bool HasTarget => Owner.HasTarget;
        protected Vector2 TargetPosition => Owner.TargetPosition;

        public int MinimumLockingRange { get => 0; }
        protected float TimeUntilAttack;

        private void Awake()
        {
            try
            {
                Owner = transform.parent.GetComponent<MonoBehaviour>() as Entity; //If currently bound to enemy
                TimeUntilAttack = IsFriendly ? 0 : TimeUntilAttack;

                //Damage, speed, and bullets fired are randomly sampled from normal distributions if the owner is an enemy and non-elite
                if (Owner is EnemyScript e && !e.isElite)
                {
                    //Shooter damage scales 2/3 as much as the enemy multiplier
                    damageMultiplier = (float)Data.damageSample*(2f/3);
                    //Shooter attack speed scales 2/3 as much as the enemy multiplier
                    attackSpeedMultiplier = (float)Math.Max(1, Data.attackTimeSample*(2f/3));
                    additionalBulletsFired = Data.bulletsSample;
                }

            }
            catch (NullReferenceException)
            {
                enabled = false; //If dropped from enemy
            }

        }


        public void Update()
        {
            var dt = Time.deltaTime;
            TryAttack(dt);
        }

        protected abstract void Shoot();

        public virtual void TryAttack(float dt)
        {
            TimeUntilAttack -= dt;

            if (TimeUntilAttack < 0 && Owner.IsAttacking)
            {
                TimeUntilAttack = attackTime;
                Shoot();
            }
        }


        public void HandleUpgrade(UpgradeScript u)
        {
            if (u.type == UpgradeType.Damage)
            {
                Owner.damageAdditive += u.damageIncrease;
            }else if (u.type == UpgradeType.Speed)
            {
                //attackSpeedMultiplier = Math.Max(attackSpeedMultiplier / u.attackDelayScalar, MinimumAttackDelay);
                Owner.AttackSpeedMultiplier /= u.attackTimeScalar;
            }
        }

        //Called after the component has been picked up to recalculate values based on new owner.
        public void Refresh()
        {
            Owner = transform.parent.GetComponent<Entity>();
            if (!Owner)
            {
                throw new Exception("Could not get Entity Script from parent.");
            } 
            enabled = true;
        }

        protected Quaternion GetCalculatedRotation(float noise)
        {
            var vec = TargetPosition;
            var targetAngle = HasTarget ? Math.Atan2(vec.y, vec.x) : Owner.Rotation;
            var actualAngle = (float)targetAngle + UnityEngine.Random.Range((float) -Math.PI / 32, (float) Math.PI / 32) *
                   noise;
            return Quaternion.AngleAxis(actualAngle * Mathf.Rad2Deg, Vector3.forward);
        }

        //Calculate the angle to fire at as a vector.
        //For most enemies this will simply be the relative position of the player to the enemy
        //But the sniper enemy will have a vector that is placed in front of the player in the direction they move
        //This should only be used by EnemyScripts
        public Vector2 CalculateTargetVector()
        {
            return DirectorScript.Player.Position - Position;
        }

        public void TryReload()
        {
            if (this is IReloadable r)
            {
                r.Reload();
                CanvasScript.DisplayReloadLabel();
            }
        }

    }
}