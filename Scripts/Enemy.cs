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

    private Animator animator;
    private Transform target;
    private PlayerScript playerScript;
    private Collider2D collid;
    private Rigidbody2D rb;

    private string eneName;

    public int fullHealth;
    public int damage;

    private int health;

    private float nextDamage;
    private float inviTime;
    private float range;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        if (transform.name == "Skeleton")
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lookRadius * 0.8f);
        }
    }
    private void Start()
    {
        // References
        GameObject player;
        eneName = transform.name;
        animator = GetComponent<Animator>();
        collid = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        player = PlayerManager.instance.player;
        target = player.transform;
        playerScript = player.GetComponent<PlayerScript>();
        SetStats();
    }
    private void SetStats()
    {
        // Stats
        fullHealth += 20 * playerScript.GetLevel();
        healthBar.SetMaxHealth(fullHealth);
        health = fullHealth;
        damage += 10 * playerScript.GetLevel();
        nextDamage = 0;
        inviTime = 0.5f;
        range = lookRadius * 0.8f;
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
            if (distance < lookRadius)
                animator.SetBool("isFollowing", true);
            else if (animator.GetBool("isFollowing"))
            {
                animator.SetBool("isFollowing", false);
                rb.velocity = Vector3.zero;
            }

        }
        else if (eneName.Contains("Skeleton"))
        {
            if (!(range > distance) && distance < lookRadius)
            {
                animator.SetBool("Shootin", false);
                animator.SetBool("isFollowing", true);
            }
            else if (range > distance)
            {
                animator.SetBool("isFollowing", false);
                rb.velocity = Vector3.zero;
                animator.SetBool("Shootin", true);
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