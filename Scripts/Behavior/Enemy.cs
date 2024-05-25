using UnityEngine;
public class EnemyScript : MonoBehaviour
{
    /* REQUIREMENTS:
     *
     *  1.) acurate naming
     *  2.) animator requirements:
     *      a.) bool "isFollowing"
     *
     *  # Skeleton:
     *      animator:
     *          "Shootin"
     *          "runAway"
     *  
     *  # 
     *
     */
    private Animator animator;
    private Transform target;
    private EnemyStats stats;
    private Rigidbody2D rb;

    private float rangeMax;
    private float rangeMin;
    private bool targeted;
    private const float tarTol = 0.2f; // Targeting Tolerancy
    private void OnDrawGizmosSelected()
    {
        stats = GetComponent<EnemyStats>();
        SetStats();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.lookRadius);
        if (transform.name == "Skeleton" || transform.name == "Imp")
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, rangeMax);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangeMin);
        }
    }
    private void Start()
    {
        // References
        animator = GetComponent<Animator>();
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targeted = false;
        SetStats();
    }
    private void SetStats()
    {
        // Only specials
        if (name.Contains("Skeleton"))
        {
            rangeMax = stats.lookRadius * 0.8f;
            rangeMin = stats.lookRadius * 0.4f;
        }
        else if (name.Contains("Imp"))
        {
            rangeMax = stats.lookRadius * 0.8f;
            rangeMin = stats.lookRadius * 0.2f;
        }
    }
    private void Update()
    {
        Behavior();
    }
    private void Behavior()
    {
        float distance = Vector2.Distance(target.position, transform.position);

        switch (name)
        {
            case "Zombie":
            case "Demon":
                if (distance < stats.lookRadius)
                    animator.SetBool("isFollowing", true);
                else if (animator.GetBool("isFollowing"))
                {
                    animator.SetBool("isFollowing", false);
                    rb.velocity = Vector3.zero;
                }
                break;
            case "Imp":
            case "Skeleton":
                if (distance < rangeMin)
                // FLEES
                {
                    animator.SetBool("runAway", true);
                    animator.SetBool("Shootin", false);
                    animator.SetBool("isFollowing", true);
                    targeted = false;
                }
                else if (targeted && distance < rangeMax + rangeMax*tarTol
                            ||
                        !targeted && distance < rangeMax - rangeMax*tarTol)
                // SHOOTS
                {
                    animator.SetBool("isFollowing", false);
                    animator.SetBool("Shootin", true);
                    rb.velocity = Vector3.zero;
                    targeted = true;
                }
                else if ((!targeted && (rangeMax - tarTol*rangeMax < distance)
                            ||
                        (targeted && (rangeMax + tarTol*rangeMax < distance)))
                            &&
                        distance < stats.lookRadius)
                // FOLLOWS
                {
                    animator.SetBool("runAway", false);
                    animator.SetBool("Shootin", false);
                    animator.SetBool("isFollowing", true);
                    targeted = false;
                }
                else
                // DEFAULT POSE => IDLE
                {
                    if (animator.GetBool("Shootin"))
                        animator.SetBool("Shootin", false);
                    if (animator.GetBool("isFollowing"))
                        animator.SetBool("isFollowing", false);
                    rb.velocity = Vector3.zero;
                    targeted = false;
                }
                break;

            case "Slime":
            case "Magma Slime":
                if (distance < stats.lookRadius)
                    animator.SetBool("isFollowing", true);
                else if (animator.GetBool("isFollowing"))
                    animator.SetBool("isFollowing", false);
                break;

            case "Spider":
                if (distance < stats.lookRadius && !animator.GetBool("isFollowing"))
                {
                    int rand = Random.Range(0, 2);

                    if (rand == 0)
                    {
                        animator.SetBool("randomWalk", true);
                    }
                    else // follow
                    {
                        animator.SetBool("isFollowing", true);
                    }
                }
                break;

            default:
                Debug.Log($"Enemy {name} not in case");
                Destroy(gameObject);
                break;
        }
        // pridat pre kazdeho enemaka nejaky nahodny behavior ?
    }
}