using UnityEngine.UI;
using UnityEngine;

public class EnemyStats : CharakterStats
{
    /* REQUIREMENTS
     *  1.) Animator:
     *      a.) trigger "Hurt"
     *      b.) float "Horizontal"
     *      c.) float "Vertical"
     *      d.) bool "isAlive"
     */
    public Text loot;
    protected Collider2D collid;
    public float lookRadius;
    public GameObject projectile;
    protected bool stunable = true;

    public float nextDamage = 0;
    private const float inviTime = 0.2f;

    protected override void Start()
    {
        base.Start();

        collid = GetComponent<Collider2D>();
        healthBar.transform.parent.gameObject.SetActive(false);
        //Debug.Log("Enemy: " + name +" start compete");
    }
    public override void TakeDamage(int damage)
    {
        if (nextDamage < Time.time)
        {
            if (!healthBar.transform.parent.gameObject.activeSelf)
            {
                healthBar.transform.parent.gameObject.SetActive(true);
                //Debug.Log("Health Bar activated: " + name);
            }
            base.TakeDamage(damage);

            animator.SetTrigger("Hurt");

            if (stunable)
            {
                rb.velocity = Vector2.zero;
                Vector2 moveDir;

                float hori = animator.GetFloat("Horizontal");
                float vert = animator.GetFloat("Vertical");

                moveDir = new Vector2(-hori, -vert);
                rb.velocity = moveDir;
            }
            if (lookRadius <= 15)
                lookRadius += 5;

            // Invincibility
            nextDamage = Time.time + inviTime;
        }
    }
    public int DoDamage()
    {
        return damage.GetValue();
    }
    protected override void Die()
    {
        if (loot != null)
        {
            // Reward Loot
            int reward;
            reward = Random.Range(level, level * 5);
            GameManager.inventory.AddMoney(reward);
            loot.text = reward + loot.text;

            // Reward XP
            reward = Random.Range(level, level * 5);
            GameManager.instance.AddXp(reward);
        }

        // Deconstruction
        GetComponent<EnemyScript>().enabled = false;
        animator.SetBool("isAlive", false);
        rb.simulated = false;
        Destroy(collid);
        Destroy(transform.gameObject, 5);
    }

    public void SetLookRadius(float nLR)
    {
        lookRadius = nLR;
    }
}
