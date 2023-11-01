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

    private bool stunable;

    private float rangeMax;
    private float rangeMin;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        if (transform.name == "Skeleton")
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lookRadius * 0.8f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius * 0.25f);
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
        player = GameManager.instance.player;
        target = player.transform;
        playerScript = player.GetComponent<PlayerScript>();
        SetStats();
    }
    private void SetStats()
    {
        // Stats
        stunable = true;
        //fullHealth += 20 * playerScript.GetLevel();
        healthBar.SetMaxHealth(fullHealth);
        health = fullHealth;
        //damage += 5 * playerScript.GetLevel();

        if (eneName.Contains("Zombie"))
        {
        }
        else if (eneName.Contains("Skeleton"))
        {
            rangeMax = lookRadius * 0.8f;
            rangeMin = lookRadius * 0.5f;
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
            if ((rangeMax < distance || distance < rangeMin) && distance < lookRadius)
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
    }
    public int DoDamage()
    {
        return damage;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
        animator.SetTrigger("Hurt");

        if (health <= 0)
            Die();

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
    private void Die() 
    { 
        animator.SetBool("isAlive", false);
        // Deconstruction
        rb.simulated = false;
        Destroy(collid);
        Destroy(transform.gameObject, 5);
    }
}