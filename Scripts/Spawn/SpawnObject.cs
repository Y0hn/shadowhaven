using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    private GameObject[] objects;

    void Start()
    {
        GameObject spawn = null;

        if (name.Contains("Wall") || name.Contains("Floor"))
        {
            List<GameObject> temp = new List<GameObject>();

            string[] s = name.Split('-');
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
        else
        {
            objects = GameObject.FindGameObjectsWithTag(transform.name);
            int rand = Random.Range(0, objects.Length);
            if (objects.Length != 0)
                spawn = objects[rand];
        }
        if (spawn != null)
        {
            spawn = Instantiate(spawn, transform.position, Quaternion.identity, transform);
            spawn.tag = "Untagged";

            if (name.Contains("Door"))
            {
                DoorBehavior beh = spawn.GetComponent<DoorBehavior>();
                beh.vertical = transform.position.x != transform.parent.position.x;  // Ak dvere niesu priamo pod stredom miestnosti tak su vertikalne
                spawn.name = transform.parent.name + spawn.name;
            }
        }
    }
}
