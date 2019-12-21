using UnityEngine;

namespace BehaviorScripts.ProjectileBehaviors
{
    public class BurstBulletBehavior : ProjectileScript
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            var l = other.gameObject.layer;
            if (l == 10 || l == 11)
            {
                Destroy(gameObject);
            }
            else
            {
                Struck(other);
            }
        }
    }
}