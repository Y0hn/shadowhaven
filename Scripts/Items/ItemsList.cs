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

    public List<Item> Items;

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
    private List<Item> GetRarity(Rarity rar)
    {
        List<Item> list = new List<Item>();

        foreach (Item i in Items)
            if (i.rarity == rar)
                list.Add(i);

        return list;
    }
    private List<Item> GetMultiRarity(Rarity[] rar)
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
            foreach (Item i in it)
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
    public Item GetRandWeapon(Rarity rarity)
    {
        List<Item> weapons = new List<Item>();
        foreach (Item i in GetRarity(rarity))
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
    public Item GetRandArmor(Rarity rarity)
    {
        List<Item> armors = new List<Item>();
        foreach (Item i in GetRarity(rarity))
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
    public Item GetWeaponOfRarityAndBelow(Rarity rarity)
    {
        List<Item> weapons = new List<Item>();
        foreach (Item i in GetMultiRarity(RaritiesBelow(rarity)))
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
    public Item GetArmorOfRarityAndBelow(Rarity rarity)
    {
        List<Item> armors = new List<Item>();
        foreach (Item i in GetMultiRarity(RaritiesBelow(rarity)))
        {
            if (i is Armor)
                armors.Add(i);
        }
        if (armors.Count > 0)
        {
            int R = Random.Range(0, armors.Count);
            Item w = armors[R];
            Items.Remove(w);
            return w;
        }
        else
            return null;
    }
    public Item GetItemOfRarityAndBelow(Rarity rarity)
    {
        List<Item> items = new List<Item>();
        foreach (Item i in GetMultiRarity(RaritiesBelow(rarity)))
        {
            items.Add(i);
        }
        if (items.Count > 0)
        {
            int R = Random.Range(0, items.Count);
            Item w = items[R];
            Items.Remove(w);
            return w;
        }
        else
            return null;
    }
    public void EraseItemsOfRarity(Rarity rarity)
    {
        foreach (Item i in GetMultiRarity(RaritiesBelow(rarity)))
            Items.Remove(i);
    }
    private Rarity[] RaritiesBelow(Rarity rarity)
    {
        Rarity[] r = new Rarity[(int)rarity + 1];
        for (int i = 0; i <= (int)rarity; i++)
        {
            r[i] = (Rarity)i;
        }
        return r;
    }
}