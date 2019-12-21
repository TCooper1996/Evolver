using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorScripts
{
    public class RapidShooterBehavior : ShooterBehavior , IReloadable
    {

        public override string weaponName => "Rapid";
        protected override int baseBulletsPerShot => 1;
        protected override float baseAttackTime => 0.2f;
        protected override int baseDamage => 10;
        protected override int baseLockingRange => 140;
        protected override int baseBulletSpeed => 800;
        protected override int baseNoise => 2;
        public override int sightRange => 300;
        

        //Override attacktime to be constant.
        //Attack speed multipliers will instead affect reload time.
        public override float attackTime => 0.2f;

        
        //These are implementations for the IReloadable interface
        public int baseReloadTime => 3;
        public int magazineSize => 25;
        
        //Properties pertaining to IReloadable but not required
        private int _roundsRemaining = 25;
        public float reloadTime => baseReloadTime / attackSpeedMultiplier;
        private float _reloadTimeLeft = 3;
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
            var noise = HasTarget ? 0.1f : NoiseMultiplier;
            _roundsRemaining--;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                Instantiate(BulletControllerScript.RapidBullet, Position, GetCalculatedRotation(noise),  transform);
                
            }
            SoundScript.PlaySound(SoundScript.Sound.RapidBullet);
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
