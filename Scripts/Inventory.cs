using System.Collections.Generic;
using System.Linq;
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

    public GameObject droper;
    public int space = 12;
    
    public List<Item> items = new();
    private Equipment[] equipment;
    public List<int> quickSlots;

    private Transform player;

    private void Start()
    {
        // References
        player = GameObject.FindGameObjectWithTag("Player").transform;
        int n = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        n++;
        equipment = new Equipment[n];
        
        // Load Equipment From Save
    }
    public bool Add(Item item)
    {
        try{ /**/  // FOR BUGS >:|
        
            if (items.Count >= space)
                return false;

            items.Add(item);

            onItemChangeCallback.Invoke();
        }
        catch
        { Debug.LogError("Item destroyed !!!"); }/**/
        return true;
    }
    public void Remove (Item item) 
    { 
        items.Remove(item);
        onItemChangeCallback?.Invoke();
    }
    public void Drop(Item item)
    {
        items.Remove(item);
        droper.GetComponent<Interactable>().item = item;
        player.GetComponent<PlayerScript>().DropedItem();
        Instantiate(droper, player.position, Quaternion.identity);

        onItemChangeCallback?.Invoke();
    }
    public void Equip (Equipment newEqu)
    {
        int slotIndex = (int)newEqu.equipSlot;
        if (equipment[slotIndex] != null)
        {
            if (slotIndex == 2 && !Input.GetKey(KeyCode.LeftShift))
            {
                slotIndex++;
                Equip(newEqu, slotIndex);
            }
            else if (newEqu != equipment[slotIndex])
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

        //Debug.Log("Equipment " + newEqu.name + " typu: " + newEqu.equipSlot + " equiped on slot " + slotIndex);
        onItemChangeCallback.Invoke();
    }
    private void Equip(Equipment newEqu, int slotIndex)
    {
        if (equipment[slotIndex] != null)
        {
            if (newEqu != equipment[slotIndex])
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
        List<Equipment> list = equipment.ToList();
        int slotIndex = list.IndexOf(newEqu);
        if (equipment[slotIndex] != null)
        {
            if (Add(equipment[slotIndex]))
                equipment[slotIndex] = null;
            else
                return false;
        }
        onItemChangeCallback.Invoke();
        return true;
    }
    public void ChangeQuickSlot(Item item, int boundry)
    {
        if (quickSlots.Count < boundry)
        {
            quickSlots.Add(items.IndexOf(item));
        }
        else
        {
            quickSlots.Remove(quickSlots.First());
            quickSlots.Add(items.IndexOf(item));
        }
    }
    public void QuickUse(int i)
    {
        i -= 3;
        if (quickSlots.Count > i)
            items[quickSlots[i]].Use();
        else
            player.GetComponent<PlayerScript>().SetActiveCombat(false);
    }

    public Equipment Equiped(int i) 
    { return equipment[i]; }
    public Equipment[] GetEquipment()
    { return equipment; }
    public void ClearInventory()
    { items = new(); }
}