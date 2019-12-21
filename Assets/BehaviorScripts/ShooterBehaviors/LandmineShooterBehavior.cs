using System;
using UnityEngine;

namespace BehaviorScripts
{
    public class LandmineShooterBehavior : ShooterBehavior
    {
        public override string weaponName => "Landmine";
        protected override int baseBulletsPerShot => 4;
        protected override float baseAttackTime => 3f;
        protected override int baseDamage => 40;
        protected override int baseLockingRange => 60;
        protected override int baseBulletSpeed => 800;
        public override int sightRange => 200;
        protected override int baseNoise => 4;

        
        protected override void Shoot()
        {
            var bulletsAdded = 0;
            if (HasTarget)
            {
                bulletsAdded = 2;
            }
            else
            {
                bulletSpeedMultiplier = 1;
            }

            for (int i = 0; i < bulletsPerShot + bulletsAdded; i++)
            {
                var b = Instantiate(BulletControllerScript.LandmineBullet, Position, GetCalculatedRotation(baseNoise), transform);
                if (HasTarget)
                {
                    b.GetComponent<Rigidbody2D>().mass = 1.2f;
                }
            }
            SoundScript.PlaySound(SoundScript.Sound.LandmineBullet);
        }
    }
}