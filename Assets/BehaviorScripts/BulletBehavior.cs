/*using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorScripts
{
    public class BulletBehavior : IBulletBehavior
    {
        public Action<Vector3, int, int, bool> OnLandmineFuse;
        public Action OnExpiration;
        private readonly Vector3 _initialPosition;
        private readonly int _damage;
        public double _angle;
        public readonly int _speed;
        private readonly bool _isFriendly;
        private readonly float _noiseMultiplier;
        private bool _live = true;
        private readonly float  _expirationTime;
        private float _expirationTimeLeft;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private float _noise;
        private Vector2 _forceVector;
        private ProjectileScript _owner;
        public readonly BulletType bulletType;
        private double landMineFuseTime;
        public bool detonated => landMineFuseTime < 0;

        public bool falling;
        public double fallTime = 1;
        public float initialScale;
        public double landMineDistance = 60;
        public double birthTimer;

        public enum BulletType
        {
            Rapid,
            Burst,
            Sniper,
            Landmine,
            Shrapnel,
        }

        public BulletBehavior(Vector3 pInitialPosition, double pAngle, int pDamage, int pSpeed, 
            bool pIsFriendly, float noiseMultiplier = 0, BulletType t = BulletType.Rapid)
        {
            bulletType = t;
            _initialPosition = pInitialPosition;
            _damage = pDamage;
            _angle = pAngle;
            _speed = pSpeed;
            _isFriendly = pIsFriendly;
            _noiseMultiplier = noiseMultiplier;

            switch (bulletType)
            {
                case BulletType.Sniper:
                    _expirationTime = 0;
                    break;
                
                case BulletType.Burst:
                    _expirationTime = .5f;
                    break;
                
                case BulletType.Rapid:
                    _expirationTime = 1f;
                    break;
                
                case BulletType.Landmine:
                    _expirationTime = 0f;
                    landMineFuseTime = 5 + Random.Range(-0.5f, 0.5f);
                    break;
                
                case BulletType.Shrapnel:
                    _expirationTime = 0.2f;
                    _live = false;
                    break;
                    
            }

            _expirationTimeLeft = _expirationTime;
        }

        public void Start(ProjectileScript owner)
        {
            _owner = owner;
            initialScale = _owner.transform.localScale.x;
            _rb = owner.GetComponent<Rigidbody2D>();
            _sr = owner.GetComponent<SpriteRenderer>();
            
            var noise = Random.Range((float)-Math.PI / 32, (float)Math.PI / 32);
            noise *= _noiseMultiplier;
            _angle += noise;
            
            _forceVector = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle)) * _speed;

            _rb.AddForce(_forceVector);
        }

        public void Update(double dt)
        {
            birthTimer += dt;

            if (bulletType == BulletType.Landmine && birthTimer * (_rb.velocity.magnitude) >= landMineDistance)
            {
                    _rb.velocity = Vector2.zero;
                    var d = (_owner.transform.parent.gameObject.GetComponent<ShooterBehavior>().Owner.Position -
                        _owner.transform.position).magnitude;
                    Debug.Log(d);
            }
            
            if (falling)
            {
                fallTime -= dt;
                var s = (float)((fallTime / 1) * initialScale);
                _owner.transform.localScale = new Vector3(s, s, s);
                
            }
            if (!_live)
            {
                if (_expirationTimeLeft < 0)
                {
                    OnExpiration();
                }
                _expirationTimeLeft -= (float)dt;
                Vector3 c = (Vector4)_sr.color;
                _sr.color =
                    new Vector4(c[0], c[1], c[2], _expirationTimeLeft / _expirationTime);
            }
            else if (bulletType == BulletType.Landmine)
            {
                landMineFuseTime -= dt;
                if (landMineFuseTime < 0)
                {
                    _live = false;
                }
            }
                
        }

        public int GetDamage()
        {
            return _damage;
        }

        public bool GetFriendly()
        {
            return _isFriendly;
        }

        public Vector3 GetInitialPosition()
        {
            return _initialPosition;
        }

        public void HandleCollision(Collision2D collision)
        {
            
            //Set bullet to be destroyed and to ignore following collisions unless it's speed is >1000 and has hit a bullet
            if (bulletType == BulletType.Sniper && collision.gameObject.CompareTag("Bullet"))
            //Reapply physics
            {
                _rb.velocity = Vector2.zero;
                _rb.AddForce(_forceVector);
                
            }else
            {
                _live = false;
                var a = collision.collider;
                var b = collision.otherCollider;
                Physics2D.IgnoreCollision(a, b);
            }

            if (!(collision.gameObject.layer == 10 ||
                collision.gameObject.layer == 11))
            {
                SoundScript.PlaySound(SoundScript.Sound.Impact);
            }

            if (collision.gameObject.CompareTag("Hole"))
            {
                if (bulletType == BulletType.Landmine)
                {
                    falling = true;
                }
                else
                {
                    var a = collision.collider;
                    var b = collision.otherCollider;
                    Physics2D.IgnoreCollision(a, b);
                    _rb.velocity = Vector2.zero;
                    _rb.AddForce(_forceVector);
                }
            }
        }
    }
}*/