using UnityEngine;
public class FollowBehavior : StateMachineBehaviour
{
    public string targetTag = "Player";
    public float speed;
    public float rangeTolerancy = 0.2f;
    public bool toClose = false;
    public bool stopOnExit = false;
    public bool onlySetTrajectory = false;
    public float trajectoryLimit = -1;
    public float startDelay = 0;
    public float velocityStopTreshold = 0;
    public string outRawParam = "";

    private bool startDelayed = false;
    private bool inMovement = false;
    private Transform targetTra;
    private Vector2 startPos;
    private Rigidbody2D rb;
    private float delay;
    private const float upperTimeLimit = 5f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        rb = animator.GetComponent<Rigidbody2D>();
        startDelayed = startDelay != 0;
        inMovement = false;

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

        // Limit Range
        if (trajectoryLimit != -1)
        {
            startPos = animator.transform.position;
        }

        if (!startDelayed)
            SetTrajectory(animator);
        else
            delay = Time.time + startDelay;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // MOVEMENT start
        if (inMovement)
        {
            // CONTINUAL follow
            if (!onlySetTrajectory)
            {
                Vector2 pos = animator.transform.position;
                Vector2 playerPos = targetTra.position;
                Vector2 dir;

                // Set Direction 
                if (pos.x - rangeTolerancy > playerPos.x)
                    dir = Vector2.left;
                else if (pos.x + rangeTolerancy < playerPos.x)
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
                if (outRawParam != "")
                {
                    string[] temp = outRawParam.Split(',');
                    animator.SetFloat(temp[0], playerPos.x - pos.x);
                    animator.SetFloat(temp[1], playerPos.y - pos.y);
                }
            }
            // TRAJECTORY set
            else if (trajectoryLimit != -1)
            {
                Vector2 v = new Vector2(animator.transform.position.x - startPos.x, animator.transform.position.y - startPos.y);
                if (v.magnitude >= trajectoryLimit)
                {
                    //animator.SetTrigger("attack1");
                    if (stopOnExit)
                        rb.velocity = Vector2.zero;
                    inMovement = false;
                    animator.SetBool("isFollowing", false);
                }
            }
            // STOP condition
            else if (rb.velocity.x == 0 || (velocityStopTreshold != 0 && (rb.velocity.x <= velocityStopTreshold)))
            {
                //Debug.Log($"rb.velocity = 0");
                animator.SetTrigger("attack1");
                inMovement = false;
                if (stopOnExit)
                    rb.velocity = Vector2.zero;
            }
            else if (Time.time < upperTimeLimit)
            {
                rb.velocity = Vector2.zero;
            }
        }
        // ANIMATION delay
        else if (startDelayed && delay < Time.time)
        {
            SetTrajectory(animator);
            startDelayed = false;
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stopOnExit)
            rb.velocity = Vector2.zero;
        //Debug.Log("Exiting Behavior Follow");
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
        }
        inMovement = true;
    }
}