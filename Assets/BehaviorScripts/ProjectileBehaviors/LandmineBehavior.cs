using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace BehaviorScripts.ProjectileBehaviors
{
    public class LandmineBehavior : ProjectileScript
    {
        private bool falling;
        private double fallTime = 1;
        private float initialScale = -1;
        private float fuseTime = 5;
        private double birthTimer;


        private new void Awake()
        {
            base.Awake();

            initialScale = transform.localScale.x;
            fuseTime += UnityEngine.Random.Range(-0.5f, 0.5f);
        }
        
        private void Update()
        {
            var dt = Time.deltaTime;
            birthTimer += dt;
            
            fuseTime -= dt;
            if (fuseTime < 0)
            {
                Explode();
            }


            if (birthTimer > 15)
            {
                Destroy(gameObject);
            }
            
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {

                if (other.gameObject.CompareTag("Bullet"))
                {
                    Struck(other);
                }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Hole"))
            {
                if (falling)
                    return;
                
                falling = true;
                Rb.velocity = Vector2.zero;
                Rb.drag = 0;
                var vec = (other.transform.position - transform.position).normalized;
                Rb.AddForce(vec * 100);
                StartCoroutine(nameof(FallingCoroutine));
            }
        }

        private IEnumerator FallingCoroutine()
        {
            while (fallTime > 0)
            {
                fallTime -= Time.deltaTime;
                var s = (float)((fallTime / 1) * initialScale);
                transform.localScale = new Vector3(s, s, s);
                yield return null;
            }
            Destroy(gameObject);
            
        }
        

        private void Explode()
        {
            var e = Instantiate(BulletControllerScript.LandmineExplosion, Shooter.transform);
            e.transform.position = transform.position;
            SoundScript.PlaySound(SoundScript.Sound.Detonation);
            Destroy(gameObject);
        }
    }
}