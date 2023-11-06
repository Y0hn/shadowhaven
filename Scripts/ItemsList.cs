using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsList : MonoBehaviour
{
    #region Singleton
    public static ItemsList instance;
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("More than one Instance of ItemList!");
        instance = this;
    }
    #endregion

    public List <Item> Items;

    public Item GetRandItem()
    {
        int R = Random.Range(0, Items.Count);
        return Items[R];
    }
}
