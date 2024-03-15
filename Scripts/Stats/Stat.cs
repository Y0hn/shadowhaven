using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    public int value = 0;
    private float mod = 1;
    private List<int> modifiers;
    private EffectInArea effArea;
    public int GetValue()
    {
        if (modifiers == null)
            modifiers = new();
        int finalValue = value;
        foreach (int mod in modifiers)
            finalValue += mod;

        finalValue = (int)Mathf.Round(finalValue * mod);
        
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
    public void Modify(EffectInArea effectInArea)
    {
        if (mod == 1)
        {
            effArea = effectInArea;
            mod = effArea.modifier;
        }
        else if (mod > effectInArea.modifier)
        {
            effArea = effectInArea;
            mod = effArea.modifier;
        }
    }
    public bool DeModify(EffectInArea effectInArea)
    {
        if (effArea != null)
            if (effArea == effectInArea)
            {
                effArea = null;
                mod = 1;
            }
            else
                return false;
        return true;
    }
    public void DeModify()
    {
        effArea = null;
        mod = 1;
    }
    public void ClearMod()
    {
        if (modifiers == null)
            modifiers = new();
        modifiers.Clear();
    }
}
