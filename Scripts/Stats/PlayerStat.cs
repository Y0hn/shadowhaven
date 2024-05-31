using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharakterStats
{
    public LevelBar LvlBar;

    private PlayerCombatScript playerCom;
    private int numXP = 0;
    private int tillnextLvl = 10;
    private const int tillnextLvlConst = 5;

    public float nextDamage = 0;
    private const float inviTime = 0.5f;

    protected override void Start()
    {
        base.Start();
        // Additional References
        playerCom = GetComponent<PlayerCombatScript>();
        GameManager.inventory.onEquipChangeCallback += EquipmentStatsRefresh;

        LvlBar.SetMax(tillnextLvl);
        LvlBar.Set(0);
        //Debug.Log("Player stats started! ");
    }
    private void EquipmentStatsRefresh()
    {/*
        armor.ClearMod();
        damage.ClearMod();

        for (int i = 0; i < 2; i++)
        {
            Armor a = (Armor)inv.Equiped(i);

            if (a != null)
                armor.AddMod(a.armorModifier);
            
            //else Debug.Log("Equipment " + i + " is \"null\"");
        }
        //Debug.Log("Player equipment armor updated");*/
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
        GameManager.instance.playerLives = false;

        playerCom.enabled = false;
        numXP = 0;
        GameManager.inventory.ClearEquipment();
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
        speed.DeModify();
        animator.SetBool("isAlive", true);
    }
    public void SetHealth(int health)
    {
        /// carefull does not change max HP
        if (health > maxHealth)
            maxHealth = health;
        else
            curHealth = health;

        healthBar.Set(health);
    }
    public void AddXp(int xp)
    {
        numXP += xp;
        if (numXP >= tillnextLvl)
            LevelUp();
        LvlBar.Set(numXP);
    }
    public void LevelUp()
    {
        bool changeHP = true;
        level++;
        List<string> stats = new List<string>();
        stats.Add("Max Health\n" + maxHealth);
        numXP = 0;
        tillnextLvl += tillnextLvlConst*level;

        if (changeHP)
        {
            maxHealth += 10;
            stats[0] += " -> " + maxHealth;
            SetHealth(maxHealth);
        }

        // Visual
        LvlBar.SetMax(tillnextLvl);
        LvlBar.SetText("Lvl  " + level);
        healthBar.SetMax(maxHealth);
        healthBar.Set(curHealth);
        GameManager.notifi.LevelUp(stats.ToArray());
        GameManager.audio.Play("level-up");
        LvlBar.Set(numXP);
    }
    public void SetDamage(int dam, bool add = false)
    {
        if (!add)
            damage.ClearMod();
        damage.AddMod(dam);
    }
}
