using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSoundBehavior : StateMachineBehaviour
{
    public new string name = "";
    public bool random = false;
    public bool onEnter = false;
    public bool onExit = false;
    public float onUpdateTimer = -1f;
    public float delay = 0f;

    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter && delay <= 0f)
            PlaySound();
        timer = 0;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onUpdateTimer > 0 && 0 >= delay)
        {
            if (timer == 0)
                timer = Time.time + onUpdateTimer;
            else if (timer <= Time.time)
            {
                PlaySound();
                timer = Time.time + onUpdateTimer;
            }
        }
        else if (delay > 0)
        {
            if (timer == 0)
                timer = Time.time + delay;
            else if (timer <= Time.time)
            {
                PlaySound();
                delay = 0f;
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit)
            PlaySound();
    }
    private void PlaySound()
    {
        if (random)
            GameManager.audio.PlayAtRandom(name);
        else
            GameManager.audio.Play(name);
    }
}