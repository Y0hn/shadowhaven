using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public Transform[] UIs;
    private static readonly Dictionary<string, int> Dic = new()
    {
        {"base", 0},
        {"inv", 1},
        {"pause", 2},
        {"death", 3},
        {"money", 4 },
        {"quick", 5 },
    };
    private itemSlot[] inveSlots;
    private itemSlot[] equiSlots;
    private itemSlot[] quicSlots;
    private Inventory inventory;
    private int equipL;
    private bool[] pastUIs;

    private const string s = " $   ";

    private void Start()
    {
        ResetUI();
        inventory = Inventory.instance;
        inventory.onItemChangeCallback += UpdateInventory;
        inventory.onEquipChangeCallback += UpdateEquipment;
        inventory.onCashChangeCallback += UpdateCash;

        inveSlots = UIs[Dic["inv"]].GetChild(0).GetComponentsInChildren<itemSlot>();

        equipL = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        equipL++;
        equiSlots = UIs[Dic["inv"]].GetChild(1).GetComponentsInChildren<itemSlot>();
        quicSlots = UIs[Dic["quick"]].GetComponentsInChildren<itemSlot>();
    }
    private void UpdateInventory()
    {
        for (int i = 0; i < inveSlots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                inveSlots[i].AddItem(inventory.items[i]);
            }
            else
            {
                inveSlots[i].Clear();
            }
        }
    }
    private void UpdateEquipment()
    {
        for (int i = 0; i < equipL; i++)
        {
            Equipment e = inventory.Equiped(i);
            if (e != null)
                equiSlots[i].AddItem(e);
            else
                equiSlots[i].Clear();
        }
        for (int i = 0; i < quicSlots.Length; i++)
        {
            Item add = null;

            if (i == 0)
                add = inventory.Equiped(2);
            else if (i == 1)
                add = inventory.Equiped(3);
            else if (i > 1)
            {
                // Torch
            }

            if (add != null)
                quicSlots[i].AddItem(add);
            else if (i < 2)
                quicSlots[i].Clear();
        }
    }
    public void UpdateCash()
    {
        EnableUI(Dic["money"]);
        UIs[Dic["money"]].GetComponent<Text>().text = inventory.GetMoney() + s;
    }
    public void ResetUI()
    {
        pastUIs = new bool[UIs.Length];

        pastUIs[0] = UIs[0].gameObject.activeSelf;
        EnableUI(0);

        for (int i = 1; i < UIs.Length; i++)
        {
            if (i != Dic["quick"])
            {
                pastUIs[i] = UIs[i].gameObject.activeSelf;
                DisableUI(i);
            }
        }
    }
    public void RevertUI()
    {
        for (int i = 0; i < pastUIs.Length; i++)
            if (pastUIs[i] && i != Dic["quick"])
                EnableUI(i);
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
