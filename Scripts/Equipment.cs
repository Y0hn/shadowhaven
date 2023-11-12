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

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Armor")]
public class Armor : Equipment
{
    public int armorModifier = 0;
    public Sprite[] texture;
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment
{
    public Type type = Type.Melee;
    public int damageModifier;
    public bool onlySecondary = false;
    public Sprite texture = null;
    public GameObject projectile = null;
}

public enum Type
{
    Melee, Ranged, Magic
}
public enum EquipmentSlot
{
    Head, Chest, Weapon
}