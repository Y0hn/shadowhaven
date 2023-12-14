using UnityEngine;

public class FollowBehavior : StateMachineBehaviour
{
    public string targetTag;
    public float speed;
    public float tol = 0.2f;
    public bool boss = false;
    public bool selfTriger = false;
    public bool toClose = false;
    public string triger = "attack";

    private Transform targetTra;
    private Rigidbody2D rb;
    private string settedTriger;
    private float range;

    // Start
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        rb = animator.GetComponent<Rigidbody2D>();
        range = animator.GetComponent<EnemyStats>().lookRadius;
        //Debug.Log(range);

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
        settedTriger = "";

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

        if (toClose)
            if (animator.GetBool("runAway"))
            {
                moveDir = new Vector2(0 - moveDir.x, 0 - moveDir.y);
                dir = new Vector2(0 - dir.x, 0 - dir.y);
            }

        if (selfTriger)
        {
            float dist = Vector2.Distance(targetTra.position, pos);
            if (dist <= range)
            {
                //Debug.Log("Target in range");
                if (boss)
                {
                    int n = animator.GetComponent<BossStats>().numberOfAttacks + 1;

                    switch (animator.name)
                    {
                        case "ZomBoss":
                            // range = 5
                            if (dist <= range-1)
                                // Smash
                                n = 1;
                            else
                                // Stomp
                                n = 2;
                            break;

                        default:
                            n = Random.Range(1, n);
                            break;
                    }

                    settedTriger = triger + n;
                }
                animator.SetTrigger(settedTriger);
            }
            else
                rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
        }
        else
        {
            rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
        }
        
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = Vector2.zero;

        if (!settedTriger.Equals(""))
            animator.ResetTrigger(settedTriger);
    }
}