using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorScripts;
using ManagerScripts;
using UnityEngine;
using Random = System.Random;
using RoomPositionTuple = System.Tuple<System.Tuple<int, int>, UnityEngine.Vector3>;

public class PickupScript : MonoBehaviour
{
    
    
    
    //This dictionary is used to keep track of pickups that are set to spawn at specific places in specific scenes.
    //It is used to determine whether a pickup has already been picked up
    //A True value for a given roomPosition means that the pickup at the position in that room has already
    //been collected and should not be spawned.
    private static Dictionary<RoomPositionTuple, bool> staticPickups;

    private ShooterBehavior m_ShooterBehavior;

    private SpriteRenderer s;
    
    private double lifespan = 10;

    
    //An ID matching the ID of the infobox belonging to the enemy that dropped this weapon
    public int infoBoxID;
    

    // Start is called before the first frame update
    private void Start()
    {
        s = GetComponent<SpriteRenderer>();
        if (staticPickups == null)
        {
            //staticPickups = new Dictionary<RoomPositionTuple, bool>();
            staticPickups = new Dictionary<RoomPositionTuple, bool>();
            
        }
        
        RoomPositionTuple r = new RoomPositionTuple(DirectorScript.CurrentRoom, transform.position);
        if (staticPickups.ContainsKey(r))
        {
            if (staticPickups[r])
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            staticPickups[r] = false;
        }

        m_ShooterBehavior = DirectorScript.sh;
    }

    public void Update()
    {
        lifespan -= Time.deltaTime;
        var c = s.color;
        s.color = new Color(c.r, c.g, c.b, 0.4f + (float)Math.Abs(Math.Cos(lifespan))*0.6f);
        if (lifespan < 0)
        {
            Destroy(gameObject);
        }
    }


    public ShooterBehavior ConsumePickup()
    {
        RoomPositionTuple r = new RoomPositionTuple(DirectorScript.CurrentRoom, transform.position);
        staticPickups[r] = true;
        SoundScript.PlaySound(SoundScript.Sound.Item);
        CanvasScript.RemoveInfoBox(infoBoxID);
        Destroy(gameObject);
        return m_ShooterBehavior;
    }

    public static void ClearCache()
    {
        staticPickups = new Dictionary<RoomPositionTuple, bool>();
    }
    
}
