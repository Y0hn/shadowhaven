using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Ezquipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;
    public int armorModifier;
    public int damageModifier;

    public override void Use()
    {
        base.Use();
        Inventory.instance.Equip(this);
        RemoveFromInventory();
    }
    public void UnEquip()
    {
        Inventory.instance.Unequip(this);
    }
}

public enum EquipmentSlot
{
    Head, Chest, Legs, Weapon, SecWeapon
}