using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorScripts;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ProjectileScript : MonoBehaviour
{
    protected ShooterBehavior Shooter;
    public int damage => Shooter.damage;
    public bool isFriendly => Shooter.IsFriendly;
    protected float Angle;
    protected float Speed;

    protected Rigidbody2D Rb;
    private SpriteRenderer sr;

    private readonly float  _expirationTime = 2;
    private float _expirationTimeLeft;
    protected Vector2 _forceVector;
    public static List<ProjectileScript> bullets = new List<ProjectileScript>();


    public double angleRadians => Angle * Mathf.Deg2Rad;
    public double angleDegrees => Angle;

    protected void Awake()
    {
        
        Shooter = transform.parent.gameObject.GetComponent<ShooterBehavior>();
        try
        {
            gameObject.layer = isFriendly ? 8 : 9;
        }
        catch (NullReferenceException) //Destroy if bullet created after weapon dropped
        {
            Destroy(gameObject);
            return;
        }
        transform.localScale = new Vector3(1, 1, 1);
        Rb = GetComponent<Rigidbody2D>();
        Angle = transform.eulerAngles.z;
        Speed = Shooter.bulletSpeed;

        _expirationTimeLeft = _expirationTime;
        bullets.Append(this);

        _forceVector = new Vector2((float) Math.Cos(angleRadians), (float) Math.Sin(angleRadians)) * Speed;

        sr = GetComponent<SpriteRenderer>();
        Rb.AddForce(_forceVector);
    }

    protected void Struck(Collision2D collision)
    {
        var a = collision.collider;
        var b = collision.otherCollider;
        Physics2D.IgnoreCollision(a, b);
        PlayImpactSound(collision);
        StartCoroutine(nameof(FadeExpireCoroutine));
    }

    protected void PlayImpactSound(Collision2D collision)
    {
        var layer = collision.gameObject.layer;
        if (!(10 == layer || layer == 11))
        {
            SoundScript.PlaySound(SoundScript.Sound.Impact);
        }
    }

    protected void PassThrough()
    {
        Rb.velocity = Vector2.zero;
        Rb.angularVelocity = 0;
        Rb.AddForce(_forceVector);
        transform.eulerAngles = new Vector3(0, 0, Angle);
    }

    protected void IgnoreCollisions(Collision2D collision)
    {
        var a = collision.collider;
        var b = collision.otherCollider;
        Physics2D.IgnoreCollision(a, b);
    }
    
    

    public void OnBulletExpiration()
    {
/*
        if (bullet.bulletType == BulletBehavior.BulletType.Landmine)
        {
            
            var bulletBehavior = new BulletBehavior(transform.position, 0, bullet.GetDamage()/2, speed, bullet.GetFriendly(), 0, BulletBehavior.BulletType.Shrapnel);
            BulletControllerScript.CreateShrapnel(bulletBehavior);
            SoundScript.PlaySound(SoundScript.Sound.Detonation);
        }
*/
    }

    protected IEnumerator FadeExpireCoroutine()
    {
        while (_expirationTimeLeft > 0)
        {

            _expirationTimeLeft -= Time.deltaTime;

            Vector3 c = (Vector4) sr.color;
            sr.color =
                new Vector4(c[0], c[1], c[2], _expirationTimeLeft / _expirationTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    protected void OnDestroy()
    {
        try
        {
            if (Shooter.transform.childCount == 1 && !Shooter.Owner.isAlive)
            {
                Destroy(Shooter.Owner.gameObject);
            }
        }
        catch (NullReferenceException)
        {
            //Do nothing if parent does not exist
        }
    }

}