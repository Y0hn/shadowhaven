using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public List<Item> GetRarity(Rarity rar)
    {
        List<Item> list = new List<Item>();

        foreach (Item i in Items) 
            if (i.rarity == rar)
                list.Add(i);

        return list;
    }
    public List<Item> GetMultiRarity(Rarity[] rar)
    {
        List<Item> list = new List<Item>();

        foreach (Item i in Items)
            foreach (Rarity r in rar)
                if (i.rarity == r)
                    list.Add(i);

        return list;
    }
    public void SetAll(Item[] ar)
    {
        Items = ar.ToList();
    }
    public void RemoveArray(Item[] it) 
    {
        if (it != null)
            foreach(Item i in it)
                if (i != null)
                    Items.Remove(i);
    }
    public void AddRange(List<Item> list)
    {
        if (list != null)
            foreach (Item i in list)
                if (!Items.Contains(i))
                    Items.Add(i);
    }
    public Item GetRandWeapon()
    {
        List<Item> weapons = new List<Item>();
        foreach (Item i in Items)
        {
            if (i is Weapon)
                weapons.Add(i);
        }
        if (weapons.Count > 0)
        {
            int R = Random.Range(0, weapons.Count);
            Item w = weapons[R];
            Items.Remove(w);
            return w;
        }
        else
            return null;
    }
    public Item GetRandArmor()
    {
        List<Item> armors = new List<Item>();
        foreach (Item i in Items)
        {
            if (i is Armor)
                armors.Add(i);
        }
        if (armors.Count != 0)
        {
            int R = Random.Range(0, armors.Count);
            Item a = armors[R];
            Items.Remove(a);
            return a;
        }
        else
            return null;
    }
}
