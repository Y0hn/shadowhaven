using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    private GameObject[] objects;

    void Start()
    {
        GameObject spawn = null;
        string[] s;

        if (name.Contains("Wall") || name.Contains("Floor") || name.Contains("Door"))
        {
            List<GameObject> temp = new();

            s = name.Split('-');
            objects = GameObject.FindGameObjectsWithTag(s[0]);

            foreach (GameObject obj in objects)
                if (obj.name.Contains(s[1]))
                {
                    spawn = obj;
                    temp.Add(obj);
                }

            int rand = Random.Range(0, temp.Count);
            spawn = temp[rand];
        }
        else if (name.Contains("Boss"))
        {
            s = name.Split('-');
            objects = GameObject.FindGameObjectsWithTag(s[0]);
            int rand = Random.Range(0, objects.Length);
            if (objects.Length != 0)
                spawn = objects[rand];
        }
        else if (name.Contains("item"))
        {
            GameObject[] temp = Resources.LoadAll<GameObject>("PreFabs");
            foreach (GameObject obj in temp)
            {
                if (obj.name == "item")
                {
                    spawn = obj;
                    break;
                }
            }
            s = name.Split('|');
            spawn.name = s[0];
            spawn.GetComponent<Interactable>().maxRarity = Item.GetRarity(s[1]);
        }
        else
        {
            try
            {
                objects = GameObject.FindGameObjectsWithTag(name);
                int rand = Random.Range(0, objects.Length);
                if (objects.Length != 0)
                    spawn = objects[rand];
            }
            catch 
            {
                Debug.LogWarning("GameObject Tag: " + name + " does not exits or sht like that ");
                Destroy(gameObject);
                return;
            }
            s = null;
        }

        // Actual spawning
        if (spawn != null)
        {
            spawn = Instantiate(spawn, transform.position, Quaternion.identity, transform.parent);

            spawn.tag = "Untagged";
            spawn.name = spawn.name.Split('(')[0];

            // Additional settings
            if      (name.Contains("Boss"))
                spawn.GetComponent<BossStats>().SetY(float.Parse(s[1]));
            else if (name.Contains("Door"))
                spawn.transform.position = new Vector3(transform.position.x, transform.position.y, 0.05f);
        }
        else
            Debug.LogWarning($"{name} is not a tag nor special");

        // Destruction
        //Debug.Log("SpawnObject script spawned new " + spawn.name + " with tag: " + name + " into the Scene" );
        Destroy(gameObject);
    }
}
