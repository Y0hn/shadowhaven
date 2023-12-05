using UnityEngine.UI;
using UnityEngine;

public class EnemyStats : CharakterStats
{
    public Text loot;
    protected Collider2D collid;
    public float lookRadius;
    protected bool stunable = true;

    protected override void Start()
    {
        base.Start();

        collid = GetComponent<Collider2D>();
    }
    public override void TakeDamage(int damage)
    {
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
            Inventory.instance.AddMoney(reward);
            loot.text = reward + loot.text;

            // Reward XP
            reward = Random.Range(level, level * 5);
            GameManager.instance.AddXp(reward);
        }

        // Deconstruction
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
