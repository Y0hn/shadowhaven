using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyScript : MonoBehaviour
{
    public float lookRadius;
    public HealthBar healthBar;

    private GameObject enemy;
    private Animator animator;
    private GameObject player;
    private Transform target;
    private PlayerScript playerScript;
    private Collider2D collid;
    private Rigidbody2D rb;

    private Vector2 hitBox;

    private int fullHealth;
    private int health;
    private int damage;

    private float nextDamage;
    private float inviTime;

    private void Start()
    {
        // References
        enemy = transform.gameObject;
        animator = GetComponent<Animator>();
        collid = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        player = PlayerManager.instance.player;
        target = player.transform;
        playerScript = player.GetComponent<PlayerScript>();

        // Stats
        fullHealth = 100;
        health = fullHealth;
        healthBar.SetMaxHealth(fullHealth);
        damage = 10;
        nextDamage = 0;
        inviTime = 0.5f;
    }
    private void Update()
    {
        float distance = Vector2.Distance(target.position, transform.position);

        if (distance < lookRadius)
            animator.SetBool("isFollowing", true);
        else if (animator.GetBool("isFollowing"))
        {
            animator.SetBool("isFollowing", false);
            rb.velocity = Vector3.zero;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        if (transform.name == "Skeleton")
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lookRadius/9*7);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius/3);
        }
    }
    public int DoDamage()
    {
        return damage;
    }
    public void TakeDamage(int damage)
    {
        if (Time.time > nextDamage)
        {
            health -= damage;
            healthBar.SetHealth(health);
            animator.SetTrigger("Hurt");

            if (health <= 0)
                Die();

            nextDamage = Time.time + inviTime;

            if (lookRadius <= 15)
                lookRadius += 5;
        }
    }
    private void Die() 
    { 
        //Debug.Log("Enemy died!");
        animator.SetTrigger("Die");
        // Deconstruction
        rb.simulated = false;
        Destroy(collid);
        Destroy(this);
    }
}