using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharakterStats
{
    private Collider2D collid;
    public float lookRadius { get; private set; }
    private bool stunable;

    void Start()
    {
        animator = GetComponent<Animator>();
        collid = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        healthBar.SetHealth(curHealth);
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
    protected override void Die()
    {
        animator.SetBool("isAlive", false);
        // Deconstruction
        rb.simulated = false;
        Destroy(collid);
        Destroy(transform.gameObject, 5);
    }
}
