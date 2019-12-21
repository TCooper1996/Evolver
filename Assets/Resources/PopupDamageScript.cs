using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Code from GameGrind on youtube.com
//https://www.youtube.com/watch?v=fbUOG7f3jq8
public class PopupDamageScript : MonoBehaviour
{
    public Animator animator;
    private Text damageText;
    private float lifespan;
    private float clipLength;

    private void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        clipLength = clipInfo[0].clip.length - 0.1f;
        //Destroy( gameObject, clipInfo[0].clip.length - 0.1f);

        damageText = animator.GetComponent<Text>();
    }

    private void Update()
    {
        lifespan += Time.deltaTime;
        if (lifespan >= clipLength)
        {
            gameObject.SetActive(false);
            lifespan = 0;
        }
    }


    public void SetText(String text, bool heal = false)
    {
        try
        {
            damageText.text = text;
            damageText.fontSize = 38;
            if (heal)
            {
                //Can't turn the text green, likely because the animation overrides it. May have to make a separate animation
                damageText.color = Color.green;
            }
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine(e);
        }
    }
}
