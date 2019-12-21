using System;
using ManagerScripts;
using UnityEngine;

namespace BehaviorScripts
{
    public class SniperShooterBehavior : ShooterBehavior
    {
        public override string weaponName => "Sniper";
        protected override int baseBulletsPerShot => 1;
        protected override float baseAttackTime => 5f;
        protected override int baseDamage => 140;
        protected override int baseLockingRange => 800;
        protected override int baseBulletSpeed => 2000;
        public override int sightRange => 2000;
        protected override int baseNoise => 0;

        public new int minimumLockingRange => 50;

        
        protected override void Shoot()
        {
            
            Instantiate(BulletControllerScript.SniperBullet, Position, GetCalculatedRotation(baseNoise), transform);
            SoundScript.PlaySound(SoundScript.Sound.SniperBullet);
        }
    }
    
    
}