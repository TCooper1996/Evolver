using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using RoomPositionTuple = System.Tuple<System.Tuple<int, int>, UnityEngine.Vector3>;
using MathNet.Numerics.Distributions;

public enum UpgradeType
{
    Speed = 0,
    Damage = 1,
    Health = 2,
}
public class UpgradeScript : MonoBehaviour
{
    public UpgradeType type;
    
    private static Dictionary<RoomPositionTuple, bool> _staticPickups;
    //Additive increase
    public int damageIncrease { get; private set; }

    //Multiplicative increase
    public float attackTimeScalar { get; private set; }

    //Additive increase
    public int healthIncrease { get; private set; }

    //These bonuses, with the exception of the attack speed upgrade, are additive
    private static Normal _damageDistribution;
    private static Normal _attackSpeedDistribution;
    private static Normal _healthDistribution;
    private static int depth;

    private void Awake()
    {
        if (depth != DirectorScript.depth)
        {
            depth = DirectorScript.depth;
            _damageDistribution = new Normal(DirectorScript.depth, 2);
            _attackSpeedDistribution = new Normal(0.9, 0.05);
            _healthDistribution = new Normal(DirectorScript.depth*2, 5);
        }

    }


    private void Start()
    {
        if (_staticPickups == null)
        {
            _staticPickups = new Dictionary<RoomPositionTuple, bool>();
        }
        
        RoomPositionTuple r = new RoomPositionTuple(DirectorScript.CurrentRoom, transform.position);
        if (_staticPickups.ContainsKey(r))
        {
            if (_staticPickups[r])
            {
                Destroy(gameObject);
            }
        }
        else
        {
            _staticPickups[r] = false;
        }

        switch (type)
        {
            case UpgradeType.Damage:
                damageIncrease = (int)Math.Max(1, _damageDistribution.Sample());
                break;
            
            case UpgradeType.Health:
                healthIncrease = (int)Math.Max(1, _healthDistribution.Sample());
                break;
            
            case UpgradeType.Speed:
                attackTimeScalar = (float)Math.Max(0.9, _attackSpeedDistribution.Sample());
                break;
        }

    }
    
    public void ConsumePickup()
    {
        RoomPositionTuple r = new RoomPositionTuple(DirectorScript.CurrentRoom, transform.position);
        _staticPickups[r] = true;
        SoundScript.PlaySound(SoundScript.Sound.Item);
        Destroy(gameObject);
    }
}
