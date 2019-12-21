using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorScripts.ProjectileBehaviors
{
    public class LandmineExplosion: ProjectileScript
    {
        public Vector3 position; 
        private float explosionDuration = 0.1f;
        private float explosionAge = 0;
        private float explosionSize = 15f;
        private int explosionType;

        protected new void Awake()
        {
            //If coming from a player, it can damage the player and enemy. If from an enemy, it can only harm the player
            gameObject.layer = transform.parent.GetComponent<ShooterBehavior>().IsFriendly
                ? LayerMask.NameToLayer("explosion")
                : LayerMask.NameToLayer("unfriendly_bullet");
            
            position = transform.position;
            Shooter = transform.parent.GetComponent<ShooterBehavior>();
            transform.localScale = new Vector3(1, 1, 1);
            //a 2 bit number determining how the x and y axes will scale
            explosionType = Random.Range(0, 3);

        }

        private void Update()
        {
            if (explosionAge < explosionDuration)
            {
                explosionAge += Time.deltaTime;

                var period = (explosionAge / explosionDuration);
                var xScale = explosionType % 2 == 0 ? Math.Cos(period) : Math.Sin(period);
                var yScale = (explosionType / 2) % 2 == 0 ? Math.Cos(period) : Math.Sin(period);
                transform.localScale = new Vector3((float)xScale * explosionSize, (float)yScale * explosionSize, 1);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Obstacle"))
            {
                Destroy(gameObject);
            }

        }
    }
    
}