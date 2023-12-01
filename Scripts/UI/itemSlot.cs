using UnityEngine;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour
{
    public Image icon;
    public Button removeBtn;

    Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
        if (item.icon != null)
        {
            icon.sprite = item.icon;
            icon.color = item.color;
        }
        icon.enabled = true;
        if (!(transform.parent.name.Contains("Left") || name.Contains("QuickSlot")))
            removeBtn.interactable = true;
    }
    public void Clear()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeBtn.interactable = false;
        //Debug.Log(name + " cleared");
    }
    public void OnRemoveBtn()
    {
        Inventory.instance.Drop(item);
    }
    public void UseItem()   // On Click event
    {
        if (item != null)
        {
            if (name.Contains("QuickSlot"))
            {
                /*if (Time.time > cooldown)*/
                if (int.TryParse(name.Split(' ')[1], out int i))
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().eqiWeap = i;
            }
            else if (transform.parent.name.Equals("Left"))
            {
                Inventory.instance.Unequip((Equipment)item);
                Clear();
            }
            else
            {
                item.Use();
            }
        }
    }
}
