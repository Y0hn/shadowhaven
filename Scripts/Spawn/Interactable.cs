using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        SpriteRenderer sRend = GetComponent<SpriteRenderer>();
        ItemsList itList = ItemsList.instance;
        if (item == null)
        {
            switch (transform.name)
            {
                case "weapon":
                    item = itList.GetRandWeapon();
                    break;
                case "armor":
                    item = itList.GetRandArmor();
                    break;
                case "weapon/armor":
                    int r = Random.Range(0, 2);
                    if (r == 0)
                        item = itList.GetRandWeapon();
                    else
                        item = itList.GetRandArmor();
                    break;
                default:
                    item = itList.GetRandItem();
                    break;
            }
            if (item == null)
                Destroy(gameObject);
            else
            {
                sRend.sprite = item.icon;
                sRend.color = item.color;
            }
        }
        else
        {
            sRend.sprite = item.icon;
            sRend.color = item.color;
        }
    }
    public void AddToInventory()
    {
        if (Inventory.instance.Add(item))
            Destroy(gameObject);
    }
}