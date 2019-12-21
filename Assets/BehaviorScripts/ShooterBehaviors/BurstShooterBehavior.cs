using System;
using System.Collections;
using BehaviorScripts;
using ManagerScripts;
using UnityEngine;
using Random = System.Random;

namespace BehaviorScripts
{
    public class BurstShooterBehavior : ShooterBehavior , IReloadable
    {
        public override string weaponName => "Burst";
        protected override int baseBulletsPerShot => 5;
        protected override float baseAttackTime => 0.5f;
        protected override int baseDamage => 20;
        protected override int baseLockingRange => 80;
        protected override int baseBulletSpeed => 1800;
        protected override int baseNoise => 3;
        public override int sightRange => (int)(200 * sightMultiplier);
        
        
        //Override attacktime to be constant.
        //Attack speed multipliers will instead affect reload time.
        public new float attackTime => 0.5f;

        
        //These are implementations for the IReloadable interface
        public int baseReloadTime => 5;
        public int magazineSize => 3;
        
        //Properties pertaining to IReloadable but not required
        private int _roundsRemaining = 3;
        public float reloadTime => baseReloadTime / attackSpeedMultiplier;
        private float _reloadTimeLeft = 5;
        private bool _reloading;

        
        //Override attack ratio to instead measure reload time and rounds remaining
        public override float attackRatio
        {
            get => _reloading ? _reloadTimeLeft / reloadTime : 1 - (float)_roundsRemaining / magazineSize;
        }
        
        private void Start()
        {
            //Enemies must reload before firing but the player is given a full magazine
            if (Owner is EnemyScript)
                _roundsRemaining = 0;
        }
        
        public override void TryAttack(float dt)
        {
            TimeUntilAttack -= dt;

            if (TimeUntilAttack < 0 && Owner.IsAttacking && _roundsRemaining > 0)
            {
                TimeUntilAttack = attackTime;
                Shoot();

            }
            else if (_roundsRemaining <= 0 && !_reloading)
            {
                StartCoroutine(nameof(ReloadCoroutine));
            }
        }
        
        protected override void Shoot()
        {
            var extraBullets = HasTarget ? 3 : 0;
            _roundsRemaining--;
            
            for (int i = 0; i < bulletsPerShot + extraBullets; i++)
            {
                Instantiate(BulletControllerScript.BurstBullet, Position, GetCalculatedRotation(baseNoise), transform);
                SoundScript.PlaySound(SoundScript.Sound.BurstBullet);
            }
        }
        
        private IEnumerator ReloadCoroutine()
        {
            _reloading = true;
            _reloadTimeLeft = (1 - ((float)_roundsRemaining/magazineSize)) * reloadTime;
            _roundsRemaining = 0;
            while (_reloadTimeLeft > 0)
            {
                _reloadTimeLeft -= Time.deltaTime;
                yield return null;
            }

            _reloading = false;
            _roundsRemaining = magazineSize;
        }
        
        public void Reload()
        {
            if (!_reloading)
                StartCoroutine(nameof(ReloadCoroutine));
        }

    }
}
