using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    public GameObject body;
    private double temp = 0;
    void Start()
    {
        HealWounds(body);
        RandomDamage();
    }
    private void Update()
    {
        if (Time.time > temp) 
        { 
            temp = Time.time + 1;
            HealWounds(body);
            RandomDamage();
        }
    }
    private void RandomDamage()
    {


        int damaged = Random.Range(0, 6);
        List<int> used = new List<int>();

        for (int i = 0; i < damaged; i++)
        {
            int hurt;
            do hurt = Random.Range(0, 6);
            while (used.Contains(hurt));
            used.Add(hurt);

            hurt = hurt * 10 + Random.Range(1, 3);

            FindChildByName(body, NumberToName(hurt)).SetActive(true);
        }
    }
    private string NumberToName(int i)
    {
        string name = "";

        switch (i / 10)
        {
            case 0: name = "head";   break;
            case 1: name = "torso";  break;
            case 2: name = "armL";   break;
            case 3: name = "armR";   break;
            case 4: name = "legL";   break;
            case 5: name = "legR";   break;
        }

        switch (i % 10)
        {
            case 1: name = name + "-damage1"; break;
            case 2: name = name + "-damage2"; break;
            default: break;
        }

        return name;
    }
    private GameObject FindChildByName(GameObject parent, string childName)
    {
        for (int i = 0; i < parent.transform.childCount; i++) 
        { 
            if (parent.transform.GetChild(i).name == childName)
                return parent.transform.GetChild(i).gameObject;

            GameObject tmp = FindChildByName(parent.transform.GetChild(i).gameObject, childName);

            if (tmp != null)
                return tmp;
        }

        return null;
    }
    private void HealWounds(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            int j = i * 10;
            DisableWounds(ref parent, NumberToName(j + 1));
            DisableWounds(ref parent, NumberToName(j + 2));
        }
    }
    private void DisableWounds(ref GameObject parent, string name)
    {
        FindChildByName(parent, name).SetActive(false);
    }
}
