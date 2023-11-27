using UnityEngine;

public class Equipment : Item
{
    public EquipmentSlot equipSlot;
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
    Head, Chest, Weapon
}