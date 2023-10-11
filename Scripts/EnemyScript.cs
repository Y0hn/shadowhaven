using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int maxHealth;
    public int minHealth;
    public GameObject enemy;

    private int fullHealth;
    private int health;
    private Animator animator;

    void Start()
    {
        animator = (Animator)enemy.GetComponent("Animator");
        fullHealth = Random.Range(minHealth, maxHealth);
        health = fullHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        animator.SetTrigger("Hurt");

        // Play animation
        if (health < 0)
            Die();
    }

    private void Die() 
    { 
        // Die Animation

        Debug.Log("Enemy died!");
        animator.SetFloat("Horizontal", -1);
        animator.SetBool("IsAlive", false);
        enemy.GetComponent<Collider2D>().enabled = false;
        Destroy(this);
    }
}
