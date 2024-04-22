using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    private GameObject[] objects;

    void Start()
    {
        GameObject spawn = null;
        string[] s = null;

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
            if (spawn != null)
                spawn.GetComponent<BossStats>().enabled = true;
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
        else if (name.Contains("Light"))
        {
            s = name.Split('-');
            objects = GameObject.FindGameObjectsWithTag(s[0]);
            List<GameObject> temp = new List<GameObject>();
            foreach (GameObject obj in objects)
                if (obj.name.Contains(s[1]))
                    temp.Add(obj);

            if (temp.Count != 0)
                spawn = temp[Random.Range(0, temp.Count)];
            else
                Debug.LogWarning("Light with name: " + s[1] + " was not found");
        }
        else
        {
            try
            {
                objects = GameObject.FindGameObjectsWithTag(name);
                int rand = Random.Range(0, objects.Length);
                if (objects.Length != 0)
                {
                    spawn = objects[rand];
                }
                else
                {
                    Debug.Log("Found none enemies of " + name);
                    Destroy(gameObject);
                    return;
                }
            }
            catch 
            {
                Debug.LogWarning("GameObject Tag: " + name + " does not exits or smth like that ");
                Destroy(gameObject);
                return;
            }
        }

        // Actual spawning
        if (spawn != null)
        {
            spawn = Instantiate(spawn, transform.position, Quaternion.identity, transform.parent);
            if (spawn.tag.Contains("Enemy"))
                spawn.tag = "Enemy";
            else
                spawn.tag = "Untagged";
            spawn.name = spawn.name.Split('(')[0];

            // Additional settings
            if      (name.Contains("Boss"))
                spawn.GetComponent<BossStats>().SetY(float.Parse(s[1]));
            else if (name.Contains("Door"))
            {
                if (s.Length == 3)
                    spawn.name += "-" + s[2];
                spawn.transform.position = new Vector3(transform.position.x, transform.position.y, 0.05f);
                //Debug.Log($"{name} spawned door {spawn.name}");
            }
            else if (name.Contains("Light"))
            {
                GameManager.lights.Register(spawn, s[2]);
            }
        }
        else
            Debug.LogWarning($"{name} is not a tag nor special");

        // Destruction
        //Debug.Log("SpawnObject script spawned new " + spawn.name + " with tag: " + name + " into the Scene" );
        Destroy(gameObject);
    }
}
