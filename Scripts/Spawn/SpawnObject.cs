using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : MonoBehaviour
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
        else
        {
            objects = GameObject.FindGameObjectsWithTag(transform.name);
            int rand = Random.Range(0, objects.Length);
            if (objects.Length != 0)
                spawn = objects[rand];

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
        Destroy(gameObject);
    }
}
