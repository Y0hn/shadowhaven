
using UnityEngine;


public class EnemyScript : MonoBehaviour
{
    private Animator animator;
    private Transform target;
    private EnemyStats stats;
    private Rigidbody2D rb;

    private string eneName;

    private float rangeMax;
    private float rangeMin;

    private void OnDrawGizmosSelected()
    {
        stats = GetComponent<EnemyStats>();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.lookRadius);
        if (transform.name == "Skeleton")
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stats.lookRadius * 0.8f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.lookRadius * 0.4f);
        }
    }
    private void Start()
    {
        // References
        eneName = transform.name;
        animator = GetComponent<Animator>();
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        SetStats();
    }
    private void SetStats()
    {
        // Only specials
        if (eneName.Contains("Skeleton"))
        {
            rangeMax = stats.lookRadius * 0.8f;
            rangeMin = stats.lookRadius * 0.4f;
        }
    }
    private void Update()
    {
        Behavior();
    }
    private void Behavior()
    {
        float distance = Vector2.Distance(target.position, transform.position);

        if (eneName.Contains("Zombie"))
        {
            if (distance < stats.lookRadius)
                animator.SetBool("isFollowing", true);
            else if (animator.GetBool("isFollowing"))
            {
                animator.SetBool("isFollowing", false);
                rb.velocity = Vector3.zero;
            }

        }
        else if (eneName.Contains("Skeleton"))
        {
            if ((rangeMax < distance || distance < rangeMin) && distance < stats.lookRadius)
            {
                if (distance < rangeMin)
                    animator.SetBool("runAway", true);
                else
                    animator.SetBool("runAway", false);

                animator.SetBool("Shootin", false);
                animator.SetBool("isFollowing", true);
            }
            else if (rangeMax > distance)
            {
                animator.SetBool("isFollowing", false);
                animator.SetBool("Shootin", true);
                rb.velocity = Vector3.zero;
            }
            else
            {
                if (animator.GetBool("Shootin"))
                    animator.SetBool("Shootin", false);
                if (animator.GetBool("isFollowing"))
                    animator.SetBool("isFollowing", false);
                rb.velocity = Vector3.zero;
            } 
        }
        else if (eneName.Contains("Slime"))
        {
            if  (distance < stats.lookRadius)
                animator.SetBool("isFollowing", true);
            else if (animator.GetBool("isFollowing"))
                    animator.SetBool("isFollowing", false);
        }
    }
}