using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New item";
    public Sprite icon = null;
    public Sprite texture = null;
    public string colorModifier = "255 255 255 1";

    public virtual void Use()
    {
        // Use item
    }
    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}