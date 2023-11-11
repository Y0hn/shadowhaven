using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        if (item != null || transform.name.Contains("item"))
        {
            SpriteRenderer sRend = GetComponent<SpriteRenderer>();

            if (item == null)
                item = ItemsList.instance.GetRandItem();

            if (item != null)
            {
                sRend.sprite = item.icon;
                sRend.color = item.color;
            }
            else
                Destroy(gameObject);
        }
    }
    public void AddToInventory()
    {
        if (Inventory.instance.Add(item))
            Destroy(gameObject);
    }
}