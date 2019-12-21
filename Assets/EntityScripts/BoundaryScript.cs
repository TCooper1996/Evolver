using System;
using BehaviorScripts;
using UnityEngine;
using static DirectorScript;

public class BoundaryScript : MonoBehaviour
{
    public static BoundaryScript instance;
    public SpriteRenderer rightChild;
    public SpriteRenderer topChild;
    public SpriteRenderer leftChild;
    public SpriteRenderer bottomChild;

    private static SpriteRenderer[] children;

    public void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        children = new[] {rightChild, topChild, leftChild, bottomChild};
        instance = this;
    }


    public static Direction GetDirectionFromWall(GameObject g)
    {
        return (Direction)Array.FindIndex(children, x => x.gameObject == g);
    }
    
}