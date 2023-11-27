using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public Rarity rarity = Rarity.Common;
    new public string name = "New item";
    public string description = string.Empty;
    public Sprite icon = null;
    public Color color = Color.white;

    public virtual void Use()
    {
        // Use item
        Inventory.instance.onItemChangeCallback.Invoke();
    }
    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}

public enum Rarity
{
    Common, Uncommon, Rare, SuperRare, Legendary
}