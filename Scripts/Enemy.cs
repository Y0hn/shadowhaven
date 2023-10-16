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
    private Vector2 attackRange;
    private Vector2 hitBox;
    private int fullHealth;
    private int health;
    private int damage = 10;

    private void Start()
    {
        // References
        animator = GetComponent<Animator>();
        player = PlayerManager.instance.player;
        target = player.transform;
        playerScript = player.GetComponent<PlayerScript>();      
        
        // Set Stats
        /* UnpackData(out int baseHealth);
        int playerLevel = playerScript.GetLevel();
        int minHealth = int.Parse(Math.Round(playerLevel * 1.5 + baseHealth).ToShortString());
        int maxHealth = int.Parse(Math.Round(playerLevel * 1.6 + baseHealth * 1.5).ToShortString());
        fullHealth = UnityEngine.Random.Range(minHealth, maxHealth);
        health = fullHealth;
        */
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
    }/*
    private void UnpackData(out int baseHealth )
    {
        
        string[] data = EnemyData.GetData(enemy.name).Split(' ');
        baseHealth = int.Parse(data[0]);
        attackRange = new Vector2(float.Parse(data[1]), float.Parse(data[2]));
        attackSpeed = float.Parse(data[3]);
        moveSpeed = float.Parse(data[4]);
        
    }*/
    public int DoDamage()
    {
        return damage;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
        animator.SetTrigger("Hurt");

        // Play animation
        if (health < 0)
            Die();
    }
    private void Die() 
    { 
        Debug.Log("Enemy died!");
        animator.SetTrigger("Die");
        Destroy(this);
    }
}