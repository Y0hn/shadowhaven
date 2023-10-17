using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyScript : MonoBehaviour
{
    public GameObject enemy;
    public float lookRadius;
    public HealthBar healthBar;

    private Animator animator;
    private GameObject player;
    private Transform target;
    private PlayerScript playerScript;
    private Collider2D collid;

    private Vector2 hitBox;

    private int fullHealth = 100;
    private int health = 100;
    private int damage = 10;

    private float nextDamage = 0;
    private float inviTime = 0.5f;

    private void Start()
    {
        // References
        animator = GetComponent<Animator>();
        collid = GetComponent<Collider2D>();
        player = PlayerManager.instance.player;
        target = player.transform;
        playerScript = player.GetComponent<PlayerScript>();

        // Stats
        healthBar.SetMaxHealth(fullHealth);
        health = fullHealth;

    }
    private void Update()
    {
        float distance = Vector2.Distance(target.position, transform.position);

        if      (distance < lookRadius)
            animator.SetBool("isFollowing", true);
        else if (animator.GetBool("isFollowing"))
            animator.SetBool("isFollowing", false);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
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
        }
    }
    private void Die() 
    { 
        Debug.Log("Enemy died!");
        animator.SetTrigger("Die");
        // Deconstruction
        Destroy(collid);
        Destroy(this);
    }
}