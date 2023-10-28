using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FollowBehavior : StateMachineBehaviour
{
    public float speed;
    public float tol = 0.2f;

    private Transform playerTra;
    private Rigidbody2D rb;

    // Start
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTra = GameManager.instance.player.transform;
        rb = animator.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 dir;
        Vector2 pos = animator.transform.position;
        Vector2 playerPos = playerTra.position;

        // Set Direction 
        if      (pos.x - tol > playerPos.x)
            dir = Vector2.left;
        else if (pos.x + tol < playerPos.x)
            dir = Vector2.right;
        else if (pos.y > playerPos.y)
            dir = Vector2.down;
        else
            dir = Vector2.up;
        
        Vector2 moveDir = playerPos - pos;
        moveDir = moveDir.normalized;

        rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);

        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
