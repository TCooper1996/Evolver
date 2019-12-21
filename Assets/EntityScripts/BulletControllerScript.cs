using System;
using System.Collections.Generic;
using BehaviorScripts;
using UnityEngine;

public class BulletControllerScript : MonoBehaviour
{
    public static BulletControllerScript bcs;

    [SerializeField]
    public GameObject sniperBullet;
    public static GameObject SniperBullet => bcs.sniperBullet;
    
    [SerializeField]
    public GameObject rapidBullet;
    public static GameObject RapidBullet => bcs.rapidBullet;
    
    [SerializeField]
    public GameObject burstBullet;
    public static GameObject BurstBullet => bcs.burstBullet;
    [SerializeField]
    
    public GameObject landmineBullet;
    public static GameObject LandmineBullet => bcs.landmineBullet;
    [SerializeField]
    
    public GameObject landmineExplosion;
    public static GameObject LandmineExplosion => bcs.landmineExplosion;

    //public static Dictionary<BulletBehavior.BulletType, GameObject> typeMatching;
    
    // Start is called before the first frame update
    private void Awake()
    {
        bcs = this;
    }
    
}