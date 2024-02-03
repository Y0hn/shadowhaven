using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holdStillBehavior : StateMachineBehaviour
{
    public bool holdVelocity = true;
    public bool inUpdate = true;
    public bool holdX = true;
    public bool holdY = true;

    Rigidbody2D rb;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        if (holdVelocity)
            rb.velocity = Vector2.zero;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (inUpdate)
        {
            if (holdVelocity)
                rb.velocity = Vector2.zero;

            // hold XY
        }
    }
}
