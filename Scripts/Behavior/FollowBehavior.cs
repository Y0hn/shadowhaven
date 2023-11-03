using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FollowBehavior : StateMachineBehaviour
{
    public string targetTag;
    public float speed;
    public float tol = 0.2f;

    private Transform targetTra;
    private Rigidbody2D rb;

    // Start
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        // Finding closet target
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        float minDistance = Mathf.Infinity;
        foreach (GameObject target in targets)
        {
            float distance = Vector2.Distance(target.transform.position, animator.transform.position);
            if (distance < minDistance)
            {
                targetTra = target.transform;
                minDistance = distance;
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 dir;
        Vector2 pos = animator.transform.position;
        Vector2 playerPos = targetTra.position;

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

        if (animator.transform.tag == "Ranged Enemy")
            if (animator.GetBool("runAway"))
            {
                moveDir = new Vector2(0 - moveDir.x, 0 - moveDir.y);
                dir = new Vector2(0 - dir.x, 0 - dir.y);
            }
        
        rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);

        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}