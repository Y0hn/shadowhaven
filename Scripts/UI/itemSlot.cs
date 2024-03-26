using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour
{
    public Image bg;
    public Image icon;
    public Button removeBtn;
    [SerializeField]
    Item item;

    public static readonly Dictionary<Rarity, Color> rarityC = new()
    {
        { Rarity.Common,    new(140/255f,140/255f,140/255f, 1) },
        { Rarity.Uncommon,  new(150/255f,115/255f, 70/255f, 1) },
        { Rarity.Rare,      new( 35/255f,150/255f,150/255f, 1) },
        { Rarity.SuperRare, new(140/255f, 90/255f,165/255f, 1) },
        { Rarity.Legendary, new(170/255f,  0/255f, 20/255f, 1)}
    };

    public void AddItem(Item newItem)
    {
        item = newItem;
        if (item.icon != null)
        {
            icon.sprite = item.icon;
            icon.color = item.color;
            bg.color = rarityC[item.rarity];
            //Debug.Log($"Setting {item.name}: Rarity[{item.rarity.ToString()}] color: ({bg.color.r},{bg.color.g},{bg.color.b})");
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
        bg.color = rarityC[Rarity.Common];
        //Debug.Log(name + " cleared");
    }
    public void OnRemoveBtn()
    {
        GameManager.inventory.Drop(item);
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
                GameManager.inventory.Unequip((Equipment)item);
                Clear();
            }
            else
            {
                item.Use();
            }
        }
    }
}
