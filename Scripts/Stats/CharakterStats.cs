using UnityEngine;
public abstract class CharakterStats : MonoBehaviour
{
    public HealthBar healthBar;
    protected Animator animator;
    protected Rigidbody2D rb;
    public int curHealth {  get; protected set; }
    public int maxHealth = 100;
    public Stat damage;
    public Stat armor;
    public int level;

    private const int minDmg = 1;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (healthBar != null )
            healthBar.SetMax(maxHealth);
    }
    private void Awake()
    {
        curHealth = maxHealth;
    }
    public virtual void TakeDamage(int dmg)
    {
        string log = name + " got [" + dmg + "] damage";
        dmg -= armor.GetValue();
        dmg = Mathf.Clamp(dmg, minDmg, int.MaxValue);
        curHealth -= dmg;
        log += " and suffered only: " + dmg;
        Debug.Log(log);

        healthBar.Set(curHealth);

        if (curHealth <= 0)
            Die();
    }
    protected virtual void Die()
    {
        // Metod will be more specified in override
    }
}