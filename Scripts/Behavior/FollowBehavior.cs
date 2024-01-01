using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FollowBehavior : StateMachineBehaviour
{
    public string targetTag;
    public float speed;
    public float tol = 0.2f;
    public bool toClose = false;
    public bool onlySetTrajectory = false;
    public string triger = "attack";
    public float stopingSpeed;

    private Transform targetTra;
    private Rigidbody2D rb;

    // Start
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        rb = animator.GetComponent<Rigidbody2D>();

        // Finding closet target
        if (targetTra == null)
        {
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

        if (onlySetTrajectory)
        {
            Vector2 pos = animator.transform.position;
            Vector2 playerPos = targetTra.position;
            Vector2 dir;

            // Set Direction 
            if (pos.x - tol > playerPos.x)
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
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onlySetTrajectory)
        {
            Vector2 pos = animator.transform.position;
            Vector2 playerPos = targetTra.position;
            Vector2 dir;

            // Set Direction 
            if (pos.x - tol > playerPos.x)
                dir = Vector2.left;
            else if (pos.x + tol < playerPos.x)
                dir = Vector2.right;
            else if (pos.y > playerPos.y)
                dir = Vector2.down;
            else
                dir = Vector2.up;

            Vector2 moveDir = playerPos - pos;
            moveDir = moveDir.normalized;

            if (toClose)
                if (animator.GetBool("runAway"))
                {
                    moveDir = new Vector2(0 - moveDir.x, 0 - moveDir.y);
                    dir = new Vector2(0 - dir.x, 0 - dir.y);
                }

            rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
            animator.SetFloat("Horizontal", dir.x);
            animator.SetFloat("Vertical", dir.y);
        }
        else if (rb.velocity.x == stopingSpeed)
        {
            animator.SetTrigger(triger);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = Vector2.zero;

        if (onlySetTrajectory)
            animator.SetTrigger(triger);
    }
}