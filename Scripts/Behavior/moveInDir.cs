using UnityEngine;

public class moveRandomDir : StateMachineBehaviour
{
    public float speed;
    public float rangeTolerancy = 0.2f;
    public bool toClose = false;
    public bool stopOnExit = false;
    public bool onlySetTrajectory = false;
    public float trajectoryLimit = -1;
    public float startDelay = 0;

    private bool startDelayed = false;
    private bool inMovement = false;
    private Transform targetTra;
    private Vector2 startPos;
    private Rigidbody2D rb;
    private float delay;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        rb = animator.GetComponent<Rigidbody2D>();
        startDelayed = startDelay != 0;
        inMovement = false;

        // Limit Range
        if (trajectoryLimit != -1)
        {
            startPos = animator.transform.position;
        }
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    private void SetTrajectory(Animator animator)
    {
        // Setting trajectory
        if (onlySetTrajectory)
        {
            Vector2 pos = animator.transform.position;
            Vector2 playerPos = targetTra.position;
            Vector2 dir;

            // Set Direction 
            if (pos.x - rangeTolerancy > playerPos.x)
                dir = Vector2.left;
            else // (pos.x + rangeTolerancy < playerPos.x)
                dir = Vector2.right;

            Vector2 moveDir = playerPos - pos;
            moveDir = moveDir.normalized;

            rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
            //Debug.Log($"Setting velocity to: [{rb.velocity.x},{rb.velocity.y}]");
            animator.SetFloat("Horizontal", dir.x);
            inMovement = true;
        }
        else
            inMovement = true;
    }
}
