using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void OnItemChange();
    public OnItemChange onItemChangeCallback;

    public delegate void OnEquipChange();
    public OnItemChange onEquipChangeCallback;

    public delegate void OnCashChange();
    public OnItemChange onCashChangeCallback;

    public GameObject droper;
    public int space = 12;
    
    public List<Item> items = new();
    private Equipment[] equipment;
    public List<int> quickSlots;
    private int money = 0;
    public int equipmentCount;

    private Transform player;

    private void Start()
    {
        // References
        player = GameObject.FindGameObjectWithTag("Player").transform;
        equipmentCount = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        equipmentCount++;
        equipment = new Equipment[equipmentCount];
        
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

        GameManager.audio.Play("equip");
        onEquipChangeCallback.Invoke();
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
        onEquipChangeCallback.Invoke();
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

    // Get Equiped
    public Equipment Equiped(int i) 
    { return equipment[i]; }
    public int GetIndexEquiped(Equipment e)
    {
        return equipment.ToList().IndexOf(e);
    }
    public Equipment[] GetEquipment()
    { return equipment; }
    public void ClearInventory()
    { 
        items = new();
        onItemChangeCallback.Invoke();
    }
    public void ClearEquipment()
    {
        for(int i = 0; i < equipment.Length; i++)
            equipment[i] = null;
        onEquipChangeCallback.Invoke();
    }

    public int GetMoney()
    {
        return money;
    }
    public void AddMoney(int add)
    {
        money += add;
        onCashChangeCallback.Invoke();
    }
    public bool PayMoney(int pay) 
    { 
        if (money > pay)
            money -= pay;
        else
            return false;
        onCashChangeCallback.Invoke();
        return true;
    }
    public bool TryGetItem(int index, out Item item)
    {
        item = null;
        if (items.Count > index)
        {
            item= items[index];
            return true;
        }
        return false;
    }
}