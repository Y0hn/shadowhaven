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
        GameManager.inventory.onItemChangeCallback.Invoke();
    }
    public void RemoveFromInventory()
    {
        GameManager.inventory.Remove(this);
    }
    public static Rarity GetRarity(string rarity)
    {
        Rarity r = Rarity.Common;

        switch (rarity)
        {
            case "Uncommon": r = Rarity.Uncommon; break;
            case "Rare": r = Rarity.Rare; break;
            case "SuperRare": r = Rarity.SuperRare; break;
            case "Legendary": r = Rarity.Legendary; break;
        }

        return r;
    }
}

public enum Rarity
{
    Common, Uncommon, Rare, SuperRare, Legendary
}