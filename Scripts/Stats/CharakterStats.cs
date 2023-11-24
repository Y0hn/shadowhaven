using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharakterStats : MonoBehaviour
{
    public HealthBar healthBar;
    protected Animator animator;
    protected Rigidbody2D rb;

    public int maxHealth = 100;
    public int curHealth {  get; protected set; }

    public Stat damage;
    public Stat armor;
    public int level;

    private const int minDmg = 1;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (healthBar != null )
            healthBar.SetMaxHealth(maxHealth);
    }
    /* Testing 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TakeDamage(50);
    }*/

    private void Awake()
    {
        curHealth = maxHealth;
    }
    public virtual void TakeDamage(int dmg)
    {
        dmg -= armor.GetValue();
        dmg = Mathf.Clamp(dmg, minDmg, int.MaxValue);
        curHealth -= dmg;

        healthBar.SetHealth(curHealth);

        if (curHealth <= 0)
            Die();
    }
    protected virtual void Die()
    {
        // Override for metod will be more specified
    }
}
