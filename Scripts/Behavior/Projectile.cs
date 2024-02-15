using UnityEngine;
public class ProjectileScript : MonoBehaviour
{
    public int damage;
    public float size;
    public float force;
    public float timeToDie;
    public LayerMask targetLayers;
    public bool ableToMove = true;

    private Vector3 targetPos;
    private Vector2 velocity;
    private Rigidbody2D rb;
    private int targets;

    void Start()
    {
        //Debug.Log("Fired projectile name: " + name);

        // References
        string[] temp = name.Split('-');
        if (temp.Length > 1)
        {
            string s = temp[1];
            if (s.Equals("BossProjectile"))
            {
                Debug.Log("Boss projectile");
                targetLayers = LayerMask.NameToLayer("Player");
                gameObject.layer = LayerMask.NameToLayer(s);
            }
            else
                targetLayers &= ~(1 << LayerMask.NameToLayer(s));
            rb = GetComponent<Rigidbody2D>();

            if (s == "Player")
            {
                targetPos = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(0).GetChild(1).position;
                targets = 1;
            }
            else // if (transform.name.Contains("Enemy")) 
            {
                Transform playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                targetPos = playerTrans.position;
                targets = 0;
            }
            // Variables
            if (ableToMove)
            {
                Vector3 direction = targetPos - transform.position;
                Vector3 rotation = transform.position - targetPos;
                velocity = new Vector2(direction.x, direction.y).normalized * force;
                rb.velocity = velocity;
                float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, rot + 90);
            }
            else
            {
                velocity = Vector2.zero;
            }

            timeToDie = Time.time + timeToDie;
        }
        else
        {
            Debug.LogWarning("Projectile has no sender");
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Time.time >= timeToDie)
        {
            Destroy(transform.gameObject);
            Destroy(this);
        }
        else
        {
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, size);

            foreach (Collider2D hit in hitTargets)
            { 
                bool succsess;
                CharakterStats hitted;

                if (targets == 0)
                {
                    succsess = hit.TryGetComponent(out PlayerStats ps);
                    hitted = ps;
                }
                else
                {
                    succsess = hit.TryGetComponent(out EnemyStats es);
                    hitted = es;
                }

                if (succsess)
                {
                    hitted.TakeDamage(damage);
                    Destroy(gameObject);
                }
            }
            if (!rb.velocity.Equals(velocity) && ableToMove)
                Destroy(gameObject);
        }
    }
    public int DoDamage() { return damage; }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, size);
    }
}
