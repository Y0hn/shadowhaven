using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    private int value;

    private List<int> modifiers = new List<int>();
    public int GetValue()
    {
        int finalValue = value;
        modifiers.ForEach(x => finalValue += x);
        return finalValue; 
    }
    public void AddMod(int mod)
    {
        if (mod != 0)
            modifiers.Add(mod);
    }
    public void RemMod(int mod)
    {
        if (mod != 0)
            for (int i = 0; i < modifiers.Count; i++) 
                if (modifiers[i] == mod)
                    modifiers.RemoveAt(i);
    }
    public void ClearMod()
    {
        modifiers.Clear();
    }
}