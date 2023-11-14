using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.WSA;

public class PlayerStats : CharakterStats
{
    private PlayerCombatScript playerCom;
    private Inventory inv;
    private int numE;

    public float nextDamage = 0;
    private const float inviTime = 0.5f;

    public void SetHealth(int health)
    {
        /// carefull does not change max HP
        curHealth = health;
    }
    protected override void Start()
    {
        base.Start();

        playerCom = GetComponent<PlayerCombatScript>();
        inv = Inventory.instance;

        inv.onEquipChangeCallback += EquipmentStatsRefresh;
        numE = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
    }
    private void EquipmentStatsRefresh()
    {
        armor.ClearMod();
        damage.ClearMod();

        for (int i = 0; i < numE; i++)
        {
            Equipment e = inv.Equiped(i);
            Weapon w;
            Armor a;

            if (e != null)
                switch (i)
                {
                    case 0:
                    case 1:
                        a = (Armor)e;
                        armor.AddMod(a.armorModifier);
                        break;
                    case 2:
                    case 3:
                        w = (Weapon)e;
                        damage.AddMod(w.damageModifier);
                        break;
                    default:
                        break;
                }
        }
    }
    public override void TakeDamage(int dmg)
    {
        if (nextDamage < Time.time)
        {
            //Debug.Log("Player Takain dmg!");

            base.TakeDamage(dmg);

            healthBar.SetHealth(curHealth);
            animator.SetTrigger("Hurt");

            // Invincibility
            nextDamage = Time.time + inviTime;
        }
    }
    protected override void Die()
    {
        rb.simulated = false;
        playerCom.enabled = false;

        animator.SetBool("isAlive", false);
        //Debug.Log("Hrac zomrel");
        GameManager.playerLives = false;
    }
    public void Resurect()
    {
        curHealth = maxHealth;
        healthBar.SetHealth(curHealth);
        //playerCom.enabled = true;
        rb.simulated = true;
        animator.SetBool("isAlive", true);
    }
}
