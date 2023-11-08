using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsList : MonoBehaviour
{
    #region Singleton
    public static ItemsList instance;
    private void Awake()
    {
        if (transform.tag == "GameController")
        {
            if (instance != null)
                Debug.LogWarning("More than one Instance of ItemList!");
            else
                instance = this;
            //Debug.Log("Vytvorena instance");
        }
    }
    #endregion

    public List <Item> Items;

    public Item GetRandItem()
    {
        if (Items.Count != 0)
        {
            int R = Random.Range(0, Items.Count);
            Item item = Items[R];
            Items.Remove(item);
            return item;
        }
        else
            return null;
    }
    public List<Item> GetAll()
    {
        return Items;
    }
    public void SetAll(List<Item> list)
    {
        Items.Clear();
        Items = list;
    }
    public void RemoveArray(Item[] it) 
    {
        if (it != null)
            foreach(Item i in it)
                if (i != null)
                    Items.Remove(i);
    }
}
