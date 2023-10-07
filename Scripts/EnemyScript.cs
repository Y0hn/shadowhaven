using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int maxHealth;
    public int minHealth;

    private int fullHealth;
    private int health;

    void Start()
    {
        fullHealth = Random.Range(minHealth, maxHealth);
        health = fullHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Play animation
        if (health < 0)
            Die();
    }

    private void Die() 
    { 
        // Die Animation

        Debug.Log("Enemy died!");
        Destroy(this);
    }
}
