using System;
using BehaviorScripts.ProjectileBehaviors;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorScripts
{
    public abstract class CombatantBehavior : MonoBehaviour
    {
        public Entity owner { get; private set; }
        
        [RangeAttribute(0.5f, 20)]
        public int healthMultiplier = 1;
        private const int BaseHealth = 100;
        public int maxHealth => (int)(BaseHealth * healthMultiplier * owner.healthMultiplier);
        public int health { get; private set; }
        public float healthRatio => (float)health / maxHealth;
        

        public float damageIntakeMultiplier => owner.DamageIntakeMultiplier;
        
        [HideInInspector]
        public bool isFriendly;


        private void Awake()
        {
            owner = transform.parent.GetComponent<Entity>();
            if (!owner)
            {
                throw new Exception("Could not get Entity Script from parent.");
            }
        }

        protected void Start()
        {
            
            health = maxHealth;
            isFriendly = owner.isFriendly;
        }

        public virtual void HandleHit(ProjectileScript bullet)
        {
            Injure((int)(bullet.damage*damageIntakeMultiplier));
        }

        public void HandleUpgrade(UpgradeScript u)
        {
            if (u.type == UpgradeType.Health)
            {
                health += u.healthIncrease;
            }
        }

        protected void Injure(int damage)
        {
            
            health -= damage;
            owner.SetColor(healthRatio);
            if (health <= 0)
            {
                owner.OnDeath();
            }
        }

        protected void Heal(int damage)
        {
            health += damage;
            owner.SetColor(healthRatio);
        }

    }
}

