using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment
{
    public Type type = Type.Melee;
    public int damageModifier;
    public bool onlySecondary = false;
    public Sprite texture = null;
    public GameObject projectile = null;
}