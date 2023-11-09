using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        if (item != null || transform.name.Contains("item"))
        {
            if (item == null)
                item = ItemsList.instance.GetRandItem();
            if (item != null)
                transform.GetComponent<SpriteRenderer>().sprite = item.icon;
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