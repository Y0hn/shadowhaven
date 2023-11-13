using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.WSA;

public class PlayerStat : CharakterStats
{
    Inventory inv;
    private PlayerCombatScript playerCom;
    private int numE;

    private float nextDamage = 0;
    private const float inviTime = 0.5f;

    private void Start()
    {
        inv = Inventory.instance;
        playerCom = GetComponent<PlayerCombatScript>();
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
        base.TakeDamage(dmg);

        healthBar.SetHealth(curHealth);
        animator.SetTrigger("Hurt");

        // Invincibility
        nextDamage = Time.time + inviTime;
    }
    protected override void Die()
    {
        base.Die();

        rb.simulated = false;
        playerCom.enabled = false;

        animator.SetBool("isAlive", false);
        //Debug.Log("Hrac zomrel");
        GameManager.playerLives = false;
    }
}
