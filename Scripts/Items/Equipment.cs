using UnityEngine;

public class Equipment : Item
{
    public EquipmentSlot equipSlot;
    public override void Use()
    {
        base.Use();
        GameManager.inventory.Equip(this);
        RemoveFromInventory();
    }
    public void UnEquip()
    {
        GameManager.inventory.Unequip(this);
    }
}
public enum EquipmentSlot
{
    Head, Chest, Weapon
}