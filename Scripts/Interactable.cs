using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        transform.GetComponent<SpriteRenderer>().sprite = item.icon;
    }

    public void AddToInventory()
    {
        //Debug.Log("Picking up " + item.name);
        if (Inventory.instance.Add(item))
        {
            //Debug.Log("Destroyed " + gameObject.name);
            Destroy(gameObject);
        }
    }
}