using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehavior : StateMachineBehaviour
{
    public float speed;

    private Transform playerPos;
    private float tol = 0.2f;

    // Start
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = PlayerManager.instance.player.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 dir;
        Vector2 pos = animator.transform.position;

        // Move Towards Player
        Vector2 movePos = Vector2.MoveTowards(pos, playerPos.position, speed * Time.deltaTime);

        // Set Direction
        if      (pos.x - tol * Time.deltaTime > movePos.x)
            dir = Vector2.left;
        else if (pos.x + tol * Time.deltaTime < movePos.x)
            dir = Vector2.right;
        else if (pos.y > movePos.y)
            dir = Vector2.down;
        else
            dir = Vector2.up;

        animator.transform.position = movePos;
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
