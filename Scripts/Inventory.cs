using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("More than one Instance of Inventory!");
        instance = this;
    }
    #endregion

    public delegate void OnItemChange();
    public OnItemChange onItemChangeCallback;

    [SerializeField]
    private int space = 12;
    public List<Item> items = new();
    Equipment[] equipment;

    private void Start()
    {
        int n = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        equipment = new Equipment[n];
    }

    public bool Add(Item item)
    {
        if (items.Count >= space)
            return false;
            
        items.Add(item);

        onItemChangeCallback?.Invoke();
        return true;
    }
    public void Remove (Item item) 
    { 
        items.Remove(item);

        onItemChangeCallback?.Invoke();
    }
    public void Equip (Equipment newEqu)
    {
        int slotIndex = (int)newEqu.equipSlot;
        if (equipment[slotIndex] != null)
        {
            if (Add(equipment[slotIndex]))
            {
                equipment[slotIndex] = newEqu;
                Remove(newEqu);
            }
        }
        else
        {
            equipment[slotIndex] = newEqu;
            Remove(newEqu);
        }        
    }
    public bool Unequip(Equipment newEqu)
    {
        int slotIndex = (int)newEqu.equipSlot;
        if (equipment[slotIndex] != null)
        {
            if (Add(equipment[slotIndex]))
                equipment[slotIndex] = null;
            else
                return false;
        }
        return true;
    }
    public Equipment Equiped(int i) 
    { 
        return equipment[i];
    }
}