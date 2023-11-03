using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour
{
    public Image icon;
    public Button removeBtn;

    Item item;
    Equipment equipment;

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeBtn.interactable = true;
    }
    public void AddItem(Equipment newItem)
    {

        equipment = newItem;

        icon.sprite = equipment.icon;
        icon.enabled = true;
    }
    public void Clear()
    {
        item = null;
        equipment = null;
        icon.sprite = null;
        icon.enabled = false;
        removeBtn.interactable = false;
    }
    public void OnRemoveBtn()
    {
        Inventory.instance.Remove(item);
    }
    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
        if (transform.parent.name == "Left")
        {
            if (equipment != null)
            {
                Inventory.instance.Unequip(equipment);
                Clear();
            }
        }
    }
}
