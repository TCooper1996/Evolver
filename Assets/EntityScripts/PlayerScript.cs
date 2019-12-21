using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorScripts;
using BehaviorScripts.ProjectileBehaviors;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;
using static Ability;


public class PlayerScript : Entity
{
    public static int absorbDuration = 2;
    public static int absorbCooldown = 15;
    public static double absorbUsageRatio = 1; //This should always be between 0 and 1;
    private static bool absorbReady;
    private static bool absorbCooling;

    public override float healthMultiplier => 1;

    private static PlayerScript _player;
    public static ShooterBehavior ShooterComponent => _player.shooterComponent;
    [SerializeField]
    private PlayerMovement movementComponent;

    public override bool isFriendly => true;
    public override bool IsAttacking => !DirectorScript.Director.paused && aim.magnitude > 0.9;

    private Rigidbody2D _Rb;

    public override float BulletForceMultiplier => shooterComponent.bulletSpeedMultiplier;

    //These attributes persists across levels, and are modified by upgrades
    [SerializeField]
    private float attackSpeedMultiplier = 1;
    public override float AttackSpeedMultiplier
    {
        get => attackSpeedMultiplier; 
        set => attackSpeedMultiplier = value;
    }
    
    //Following are all properties used to create the interface that the components and other entities use to communicate with others.
    public override Vector3 Position => model.transform.position;
    public Vector2 velocity => movementComponent.velocity;

    public Vector2 aim
    {
        set => movementComponent.aim = value;
        get => movementComponent.aim;
    }

    private ControlsManager _controlsManager;

    private bool _isMovementCoroutineRunning;

    private new void Awake()
    {
        if (DirectorScript.Player)
        {
            Destroy(gameObject);
            return;
        }

        //shooterComponent.Owner = this;
        _Rb = model.GetComponent<Rigidbody2D>();
        _controlsManager = new ControlsManager(this);
        //ControlsManager.AssignAbility("LeftShoulder", AbilityName.Guard);
        DirectorScript.Player = this;
        _player = this;
        DirectorScript.SetDontDestroyOnLoad(gameObject);
        PlayerStartTests();

        base.Awake();

    }


    private void FixedUpdate()
    {
        movementComponent.Move(Time.deltaTime);
        
    }


    public override void OnCollision(Collision2D other)
    {

        var g = other.gameObject;
        
        switch (g.GetComponent<MonoBehaviour>())
        {
            case ProjectileScript bullet:
                
                if (bullet is LandmineExplosion e)
                {
                    int layerMask = ~LayerMask.GetMask("unfriendly_bullet", "unfriendly_combatant", "friendly_bullet", "friendly_combatant");
                    var vecExplosionToPlayer = Position - e.position;
                    if (Physics2D.Raycast(e.position, vecExplosionToPlayer, vecExplosionToPlayer.magnitude, layerMask))
                    {
                        break;
                    }
                }

                //Landmines do not hurt entities; only their explosions do.
                if (bullet is LandmineBehavior)
                {
                    break;
                }
                
                combatantComponent.HandleHit(bullet);

                //Determine if the player is being damge or healed
                var value = (int)(bullet.damage * combatantComponent.damageIntakeMultiplier);
                if (value > 0)
                {
                    OnHit(value);
                    absorbUsageRatio += Math.Pow(0.5, 10*combatantComponent.healthRatio)/2;
                }
                else
                {
                    OnHeal(value);
                }

                if (!_isMovementCoroutineRunning)
                {
                    StartCoroutine(UpdatePositionUntilStill());
                }
                break;
            
            case MacGuffinScript _:
                DirectorScript.MacGuffinObtained();
                SoundScript.PlaySound(SoundScript.Sound.ACK);
                Destroy(g);
                break;
            
            case ExitScript _:
                DirectorScript.OnExit();
                break;
            
            case PickupScript pickupScript:
                Destroy(shooterComponent.gameObject);
                shooterComponent = pickupScript.ConsumePickup();
                shooterComponent.transform.parent = gameObject.transform;
                shooterComponent.transform.localScale = new Vector3(1, 1, 1);
                shooterComponent.Refresh();
                break;
            
            case UpgradeScript upgradeScript:
                shooterComponent.HandleUpgrade(upgradeScript);
                combatantComponent.HandleUpgrade(upgradeScript);
                upgradeScript.ConsumePickup();
                break;
                
            default:
                if (g.CompareTag("boundary"))
                {
                    foreach (Transform b in shooterComponent.transform)
                    {
                        Destroy(b.gameObject);
                    }
                    //Determine direction of boundary from player angle
                    //The + Math.PI/8 is added so that an angle slightly less than 90 degrees, which should result in up, doesn't result in a right direction.
                    var d = (int)Math.Floor((Math.Atan2(Position.y, Position.x) + Math.PI/8)/(Math.PI/2));
                    //Add 4 and mod 4 in case Atan2 provides a negative, in the case of bottom facing angles
                    transform.position = new Vector3((int) -Math.Cos(Math.PI / 2 * d) * 18,
                    (int) -Math.Sin(Math.PI / 2 * d) * 8, 0);
                    model.transform.localPosition = new Vector3(0, 0, 0);
                    DirectorScript.HandleBoundaryCollision((DirectorScript.Direction)((d + 4) % 4));
                }
                break;
        }
            
    }

    private void OnEnable()
    {
        _controlsManager.EnableControls();
    }

    public void OnDisable()
    {
        _controlsManager?.DisableControls();
    }


    private void PlayerStartTests()
    {
        #if UNITY_EDITOR
            var numberOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
            Assert.IsTrue(numberOfPlayers <= 1);
        #endif
    }


    public void SetMove(Vector2 vec)
    {
        if (DirectorScript.Director.paused)
            return;
        
        movementComponent.move = vec;
        if (vec != Vector2.zero)
        {
            DirectorScript.OnPlayerMove();
        }
        else if (!_isMovementCoroutineRunning)
        {
            StartCoroutine(nameof(UpdatePositionUntilStill));
        }
            

    }

    public void SetAim(Vector2 vec)
    {
        if (DirectorScript.Director.paused)
            return;
        
        aim = vec;
        TryLock();
        if (aim != Vector2.zero)
        {
            var angleVector = IsTargetLocked() ? GetTargetAngle() : vec;
            var angleDegrees = UtilityFunctions.ToAngle(angleVector);
            SetRotation(angleDegrees);
        }        
    }

    public void SetRotation(float zAngle)
    {
        var e = model.transform.rotation.eulerAngles;
        model.transform.eulerAngles = new Vector3(e.x, e.y, Mathf.Rad2Deg * (zAngle - (float)Math.PI/2));
    }

    public void AddVelocity(Vector2 velocity)
    {
        if (_Rb.velocity.magnitude > 100)
        {
            var vec = _Rb.velocity.normalized;
            _Rb.velocity = vec * 100;
        }
        else
        {
            _Rb.AddForce(velocity);
        }
    }


    public static bool IsTargetLocked()
    {

        try
        {
            return _player.HasTarget;//shooterComponent.HasTarget();
        }
        catch (NullReferenceException e)
        {
            //Determine why a NullReference is ever thrown
            Console.WriteLine(e);
            return false;
        }
    }


    public static Entity GetLockedTarget()
    {
        return _player.Target;
    }

    public static Vector2 GetTargetAngle()
    {
        return _player.TargetPosition; //shooterComponent.GetTargetVector();
    }



    public override void TryLock()
    {
        var oldTarget = Target;
        Target = null;
        
        foreach (var e in DirectorScript.GetEnemyList())
        {
            if (aim == Vector2.zero)
            {
                break; //Don't bother looking for locks if the player isn't aiming
            }
            
            var relativePosition = e.Position - Position;
            var distance = relativePosition.magnitude;
            var dot = Vector2.Dot(aim.normalized, relativePosition.normalized);
            if (dot > 0.85 && shooterComponent.MinimumLockingRange < distance && distance < shooterComponent.lockingRange)
            {
                //Assign target if no current target exists or if new target is closer than current target
                if (!Target || distance < TargetDistance)
                {
                    Target = e;
                    TargetPosition = relativePosition;
                    TargetDistance = distance;
                }
            }
        }
        
        
        //Check if target changed
        if (oldTarget != Target)
        {
            CanvasScript.UpdatePlayerTarget();
        }
    }


    //This is called when the button associated with absorb, (as of now, left shoulder) is pressed or released.
    public static void OnAbsorbInteraction(bool pressed)
    {
        
        if (pressed && absorbUsageRatio >= 0.5f)
        {
            _player.StopCoroutine(nameof(AbsorbCooldownCoroutine));
            absorbCooling = false;
            _player.StartCoroutine(nameof(AbsorbCoroutine));
        }else if (!pressed)
        {
            _player.StopCoroutine(nameof(AbsorbCoroutine));
            _player.damageIntakeMultiplier = 1;
            //Start cooling coroutine only if it isn't already active
            if (!absorbCooling)
            {
                _player.StartCoroutine(nameof(AbsorbCooldownCoroutine));
            }
        }
        
    }

    public override void OnDeath()
    {
        DirectorScript.OnPlayerDeath();
    }

    //This coroutine is called when the player let's go of the movement control and it becomes (0,0)
    //It broadcasts the fact that the player is moving (despite the player letting go of the stick, because of inertia)
    //until the velocity is 0. This is to avoid putting if checks in the update method instead.
    private IEnumerator UpdatePositionUntilStill()
    {
        _isMovementCoroutineRunning = true;
        while (_Rb.velocity.magnitude > 1)
        {
            TryLock();
            DirectorScript.OnPlayerMove();
            yield return null;
        }
        _isMovementCoroutineRunning = false;
    }


    private IEnumerator AbsorbCoroutine()
    {
        _player.damageIntakeMultiplier = -1;
        while (absorbUsageRatio > 0f)
        {
            absorbUsageRatio -= Time.deltaTime/absorbDuration;
            CanvasScript.UpdatePlayerAbsorbSlider();
            yield return null;
        }
        
        _player.damageIntakeMultiplier = 1;
        _player.StartCoroutine(nameof(AbsorbCooldownCoroutine));
    }

    private IEnumerator AbsorbCooldownCoroutine()
    {
        absorbCooling = true;
        while (absorbUsageRatio <= 1)
        {
            absorbUsageRatio += Time.deltaTime/absorbCooldown;
            CanvasScript.UpdatePlayerAbsorbSlider();
            yield return null;
        }
        absorbCooling = false;
    }
    
    
}