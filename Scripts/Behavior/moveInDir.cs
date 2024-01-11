using UnityEngine;

public class moveRandomDir : StateMachineBehaviour
{
    public float rangeTolerancy = 0.2f;
    public float trajectoryLimit = -1;
    public bool stopOnExit = true;
    public float startDelay = 0;
    public float speed;
    public string exitName; // bool/trigger + string

    private bool inMovement = false;
    private Vector2 startPos;
    private Rigidbody2D rb;
    private float delay;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        rb = animator.GetComponent<Rigidbody2D>();
        inMovement = false;

        // Limit Range
        if (trajectoryLimit != -1)
            startPos = animator.transform.position;
        
        // Set Random Direction
        Vector2 dir;
        dir.x = Random.Range(-1,2);
        dir.y = Random.Range(-1,2);
        dir = dir.normalized;

        rb.velocity = new Vector2(dir.x * speed, dir.y * speed);
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);

        inMovement = true;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (inMovement)
        {
            if      (rangeTolerancy <= rb.velocity.x && rb.velocity.x <= rangeTolerancy
                  && rangeTolerancy <= rb.velocity.y && rb.velocity.y <= rangeTolerancy)
                // zastavil sa o stenu/hraca
                inMovement = false;
            
            else if (trajectoryLimit != -1)
            {
                Vector2 v = new Vector2(animator.transform.position.x - startPos.x, animator.transform.position.y - startPos.y);
                if (v.magnitude >= trajectoryLimit)
                {
                    inMovement = false;
                }
            }
        }
        else
            Exit(animator);
    }
    private void Exit(Animator animator)
    {
        //Debug.Log("Exited");

        if (stopOnExit)
            rb.velocity = Vector2.zero;
        string[] s = exitName.Split(' ');
        switch (s[0].ToLower().Trim())
        {
            case "bool":
                animator.SetBool(s[1], false);
                break;
            case "trigger":
            case "triger":
                animator.SetBool(s[1], false);
                break;
        }
    }
}
