using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsList : MonoBehaviour
{
    #region Singleton
    public static ItemsList instance;
    private void Awake()
    {
        if (transform.CompareTag("GameController"))
        {
            //Debug.Log("Creating ItemList " + this + " under " + transform.name);
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
        //Debug.Log("Item request to " + this + " under " + transform.name + " zvysny pocet itemov: " + Items.Count);

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
    public void GetAllOut(out List<Item> list)
    {
        list = Items;
        Destroy(this);
    }
    public void SetAll(List<Item> list)
    {
        Items = list;
    }
    public void RemoveArray(Item[] it) 
    {
        if (it != null)
            foreach(Item i in it)
                if (i != null)
                    Items.Remove(i);
    }
    private void OnDestroy()
    {
        //Debug.Log("Destroing ItemList " + this + " under " + transform.name);
        Destroy(this);
    }
}
