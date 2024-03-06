using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    private int value = 0;
    private List<int> modifiers;
    public int GetValue()
    {
        if (modifiers == null)
            modifiers = new();
        int finalValue = value;
        foreach (int mod in modifiers)
            finalValue += mod;
        //Debug.Log("returning: " + finalValue + " number of mods: " + modifiers.Count);
        return finalValue; 
    }
    public void AddMod(int mod)
    {
        if (modifiers == null)
            modifiers = new();
        if (mod != 0)
        {
            modifiers.Add(mod);
            //Debug.Log("Modifier added: " + mod + " total current value: " + GetValue());
        }
        //else Debug.Log("Modifier not added: 0");
    }
    public void RemMod(int mod)
    {
        modifiers.RemoveAt(modifiers.IndexOf(mod));
    }
    public void ClearMod()
    {
        if (modifiers == null)
            modifiers = new();
        modifiers.Clear();
    }
}
