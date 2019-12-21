using System;
using UnityEngine;

namespace BehaviorScripts.ProjectileBehaviors
{
    public class SniperBulletBehavior : ProjectileScript
    {
/*
        private void Start()
        {
            transform.eulerAngles = new Vector3(0, 0, shooter.Owner.Rotation * Mathf.Rad2Deg - 90);
        }
*/

        private void OnCollisionEnter2D(Collision2D other)
        {
            var g = other.gameObject;
            if (g.CompareTag("Bullet"))
            {
                IgnoreCollisions(other);
                PassThrough();
            }
            else
            {
                var l = other.gameObject.layer;
                if (l == 10 || l == 11)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    SoundScript.PlaySound(SoundScript.Sound.Impact);
                }
            }
            PlayImpactSound(other);
        }
    }
}