using System;
using System.Collections;
using BehaviorScripts;
using ManagerScripts;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer xSprite;

    [SerializeField] private GameObject chevron;

    private bool playingXAnimation;
    private float xAnimationDuration = 1;

    // Update is called once per frame
    private void Awake()
    {
        if (DirectorScript.IsMacGuffinObtained)
        {
            StartCoroutine(nameof(ChevronCoroutine));
        }
        else
        {
            chevron.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.transform.parent.GetComponent<MonoBehaviour>() is PlayerScript && !DirectorScript.IsMacGuffinObtained)
        {
            SoundScript.PlaySound(SoundScript.Sound.NAK);
            if (!playingXAnimation)
            {
                StartCoroutine(nameof(XCoroutine));
            }
        }
    }

    private IEnumerator XCoroutine()
    {
        playingXAnimation = true;
        while (xAnimationDuration > 0)
        {
            xSprite.color = new Color(1, 0, 0, xAnimationDuration);
            xAnimationDuration -= Time.deltaTime;
            yield return null;

        }

        xAnimationDuration = 1;
        playingXAnimation = false;
    }

    private IEnumerator ChevronCoroutine()
    {
        while (true)
        {
            chevron.transform.position = new Vector3(0, 20 + (float)Math.Cos(Time.time)*10, -5);
            yield return null;
        }
    }
}
