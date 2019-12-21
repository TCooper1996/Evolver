using System;
using System.Collections;
using BehaviorScripts;
using UnityEngine;
using UnityEngine.UI;

namespace ManagerScripts
{
    public class EnemyInfoScript : MonoBehaviour, IComparable
    {
        [SerializeField]
        private GameObject selector;
        [SerializeField]
        private GameObject container;

        [SerializeField] private Text TotalDamage;
        [SerializeField] private Text TotalHealth;
        
        [SerializeField] private Text WeaponType;
        
        [SerializeField] private Text AttacksPerSecond;
        [SerializeField] private Text BulletsPerAttack;
        [SerializeField] private Text Damage;
        [SerializeField] private Text BulletSpeed;
        
        //The following will only be active for IReloadable shooters
        [SerializeField] private Text ReloadDuration;
        [SerializeField] private Text MagazineSize;

        //Used to assign a unique ID to each info box
        public static int Count = 0;
        public int ID;
        

        //The bound enemy
        public EnemyScript _enemy { get; private set; }
        //A bound shooter behavior only cached if no enemy is bound.
        //Used to identify it when it needs to be removed

        private int yOffset = -380;
        private int xOffset = 722;


        //Updates values that may change
        public void OnEnable()
        {
            if (_enemy)
            {
                TotalDamage.text = "Total Damage: " + _enemy.Damage;
                TotalHealth.text = $"Health: {_enemy.health}/{_enemy.maxHealth}";
            }
        }
        
        //Binds this info box to an enemy
        public void Bind(ShooterBehavior s, Vector3 pos)
        {
            ID = Count;
            Count++;
            
            
            selector.transform.position = pos;
            
            var xPos = (pos.x > 0) ? -xOffset : xOffset;
            container.transform.localPosition = new Vector3(xPos, yOffset, -15);

            if (_enemy)
            {
                TotalHealth.text = $"Health: {_enemy.health}/{_enemy.maxHealth}";
                TotalDamage.text = "Total Damage: " + _enemy.Damage;
            }
            else
            {
                //Disable the enemy label, if this is a pickup
                transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                //Disable both the enemy health and damage labels by destroying their parent
                TotalDamage.transform.parent.gameObject.SetActive(false);
            }

            WeaponType.text = "Weapon Type: " + s.weaponName;

            AttacksPerSecond.text = $"Attack Multiplier: {s.attackSpeedMultiplier} per second";
            BulletsPerAttack.text = $"Bullets per attack: {s.bulletsPerShot}";
            Damage.text = $"Damage: {s.uiDamage}";
            BulletSpeed.text = $"Bullet Speed Mult: {s.bulletSpeedMultiplier}";

            if (s is IReloadable i)
            {
                ReloadDuration.text = $"Reload Time: {i.reloadTime}";
                MagazineSize.text = $"Magazine size: {i.magazineSize}";
            }
            else
            {
                ReloadDuration.enabled = false;
                MagazineSize.enabled = false;
            }
        }

        public void Bind(EnemyScript e)
        {
            _enemy = e;
            Bind(e.shooterComponent, e.transform.position);
        }


        public int CompareTo(object obj)
        {
            if (obj is EnemyInfoScript e)
            {
                var x1 = transform.GetChild(0).position.x;
                var x2 = e.transform.GetChild(0).position.x;
                if (x1 < x2)
                {
                    return -1;
                }else if (x1 == x2)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                throw new ArgumentException("Object is not EnemyInfoScript");
            }
        }
    }
}