using UnityEngine;

public class PlayerStats : CharakterStats
{
    public LevelBar LvlBar;

    private PlayerCombatScript playerCom;
    private Inventory inv;
    private int numE;
    private int numXP = 0;
    private int tillnextLvl = 10;
    private const int tillnextLvlConst = 10;

    public float nextDamage = 0;
    private const float inviTime = 0.5f;

    protected override void Start()
    {
        base.Start();
        // Additional References
        playerCom = GetComponent<PlayerCombatScript>();
        inv = Inventory.instance;
        inv.onEquipChangeCallback += EquipmentStatsRefresh;

        LvlBar.SetMax(tillnextLvl);
        LvlBar.Set(0);
        //Debug.Log("Player stats started! ");
    }
    private void EquipmentStatsRefresh()
    {
        armor.ClearMod();
        damage.ClearMod();

        for (int i = 0; i < 2; i++)
        {
            Armor a = (Armor)inv.Equiped(i);

            if (a != null)
                armor.AddMod(a.armorModifier);
            
            //else Debug.Log("Equipment " + i + " is \"null\"");
        }
        //Debug.Log("Player equipment armor updated");
    }
    public override void TakeDamage(int dmg)
    {
        if (nextDamage < Time.time)
        {
            //Debug.Log("Player Takain dmg!");

            base.TakeDamage(dmg);

            healthBar.Set(curHealth);
            animator.SetTrigger("Hurt");

            // Invincibility
            nextDamage = Time.time + inviTime;

            //Debug.Log("Taken damage Armor is " + armor.GetValue() + " and current Damage is " + damage.GetValue());
        }
    }
    protected override void Die()
    {
        GameManager.playerLives = false;

        playerCom.enabled = false;

        level -= 5;
        numXP = 0;

        if (level < 0)
        {
            Inventory.instance.ClearEquipment();
            level = 0;
        }

        animator.SetBool("isAlive", false);
        transform.position = Vector2.zero;
        //Debug.Log("Hrac zomrel");
    }
    public void Resurect()
    {
        curHealth = maxHealth;
        healthBar.Set(curHealth);
        //playerCom.enabled = true;
        rb.simulated = true;
        animator.SetBool("isAlive", true);
    }
    public void SetHealth(int health)
    {
        /// carefull does not change max HP
        if (health > maxHealth)
            health = maxHealth;
        else
            curHealth = health;
    }
    public void AddXp(int xp)
    {
        numXP += xp;
        if (numXP >= tillnextLvl)
        {
            numXP = 0;
            level++;
            tillnextLvl = tillnextLvlConst * level;
            maxHealth += 10;
            SetHealth((int)Mathf.Floor(maxHealth*0.2f + curHealth));

            // Visual
            LvlBar.SetMax(tillnextLvl);
            LvlBar.SetText("Lvl  " + level);
            healthBar.SetMax(maxHealth);
            healthBar.Set(curHealth);
        }
        LvlBar.Set(numXP);
    }
    public void SetDamage(int dam, bool add = false)
    {
        if (!add)
            damage.ClearMod();
        damage.AddMod(dam);
    }
}
