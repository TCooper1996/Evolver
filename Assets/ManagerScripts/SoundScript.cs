using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundScript : MonoBehaviour
{
    

    public enum Sound
    {
        RapidBullet,
        BurstBullet,
        SniperBullet,
        LandmineBullet,
        Item,
        Background,
        Menu,
        Impact,
        Struck,
        NextLevel,
        Death,
        Detonation,
        ACK,
        NAK,
        Reload,
        
    }
    public AudioClip SniperBullet, Item, Menu, NextLevel, Death, LandmineBullet, Detonation, ACK, NAK, Reload;
    public AudioClip[] Impacts;
    public AudioClip[] Strucks;
    public AudioClip[] BurstBullets;
    public AudioClip[] RapidBullets;
    public static AudioSource a;
    public static SoundScript soundInstance;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (soundInstance)
        {
            Destroy(gameObject);
            return;
        }
        soundInstance = this;
        a = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(Sound sound)
    {
        soundInstance.Play(sound);
    }
    
    public void Play(Sound sound)
    {
        switch (sound)
        {
            case Sound.RapidBullet:
                a.PlayOneShot(RapidBullets[Random.Range(0, RapidBullets.Length)], 0.2f);
                break;
            
            case Sound.BurstBullet:
                a.PlayOneShot(BurstBullets[Random.Range(0, BurstBullets.Length)], 0.1f);
                break;
            
            case Sound.Impact:
                a.PlayOneShot(Impacts[Random.Range(0, Impacts.Length)], 0.1f);
                break;
            
            case Sound.Menu:
                a.PlayOneShot(Menu);
                break;
            
            case Sound.SniperBullet:
                a.PlayOneShot(SniperBullet, 0.2f);
                break;
            
            case Sound.Struck:
                a.PlayOneShot(Strucks[Random.Range(0,Strucks.Length)]);
                break;
            
            case Sound.NextLevel:
                a.PlayOneShot(NextLevel, 0.4f);
                break;
            
            case Sound.Death:
                a.PlayOneShot(Death);
                break;
            
            case Sound.Item:
                a.PlayOneShot(Item);
                break;
            
            case Sound.LandmineBullet:
                a.PlayOneShot(LandmineBullet, 0.4f);
                break;
            
            case Sound.Detonation:
                a.PlayOneShot(Detonation, 0.2f);
                break;
            
            case Sound.ACK:
                a.PlayOneShot(ACK);
                break;
            
            case Sound.NAK:
                a.PlayOneShot(NAK);
                break;
            
            case Sound.Reload:
                a.PlayOneShot(Reload);
                break;
        }
    }

}
