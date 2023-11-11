using System.Collections.Generic;
using UnityEngine;

public class ManagerUI : MonoBehaviour
{
    public Transform[] UIs;
    static readonly Dictionary<string, int> Dic = new()
    {
        {"base", 0},
        {"inv", 1},
        {"pause", 2},
        {"death", 3},
    };

    private Inventory inventory;
    private itemSlot[] itemSlots;
    private itemSlot[] equiSlots;
    private itemSlot[] quickSlots;
    private int equipL;

    private void Start()
    {
        ResetUI();
        inventory = Inventory.instance;
        inventory.onItemChangeCallback += UpdateUI;

        itemSlots = UIs[1].GetChild(0).GetComponentsInChildren<itemSlot>();

        equipL = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        equipL++;
        equiSlots = UIs[1].GetChild(1).GetComponentsInChildren<itemSlot>();
        quickSlots = UIs[0].GetComponentsInChildren<itemSlot>();
    }
    private void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++) 
        {
            if (i < inventory.items.Count)
            {
                itemSlots[i].AddItem(inventory.items[i]);
            }
            else
            {
                itemSlots[i].Clear();
            }
        }
        for (int i = 0; i < equipL; i++)
        {
            Equipment e = inventory.Equiped(i);
            if (e != null)
                equiSlots[i].AddItem(e);
        }
        for (int i = 0; i < quickSlots.Length; i++)
        {
            Item add = null;

            if      (i == 0)
                add = inventory.Equiped(2);
            else if (i == 1)
                add = inventory.Equiped(3);
            else if (i > 1)
            {
                if (i - 2 < inventory.quickSlots.Count)
                    add = inventory.items[inventory.quickSlots[i-2]];
            }

            if (add != null)
                quickSlots[i].AddItem(add);
            else
                quickSlots[i].Clear();
        }
    }


    public void ResetUI()
    {
        EnableUI(0);
        DisableUI(1);
        DisableUI(2);
        DisableUI(3);
    }
    public void DisableUI(int n)
    {
        UIs[n].gameObject.SetActive(false);
    }
    public void DisableUI(string s)
    {
        if (Dic.TryGetValue(s, out int i))
            UIs[i].gameObject.SetActive(false);
    }
    public void EnableUI(int n)
    {
        UIs[n].gameObject.SetActive(true);
    }
    public void EnableUI(string s)
    {
        if (Dic.TryGetValue(s, out int i))
            UIs[i].gameObject.SetActive(true);
    }
}
