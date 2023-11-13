using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharakterStats : MonoBehaviour
{
    public HealthBar healthBar;
    protected Animator animator;
    protected Rigidbody2D rb;

    public int maxHealth = 100;
    public int curHealth {  get; private set; }

    public Stat damage;
    public Stat armor;

    private const int minDmg = 0;

    private void Awake()
    {
        curHealth = maxHealth;
    }
    public virtual void TakeDamage(int dmg)
    {
        dmg -= armor.GetValue();
        dmg = Mathf.Clamp(dmg, minDmg, int.MaxValue);
        curHealth -= dmg;
        if (curHealth <= 0)
            Die();
    }
    protected virtual void Die()
    {
        // Override for metod will be more specified
    }
}
