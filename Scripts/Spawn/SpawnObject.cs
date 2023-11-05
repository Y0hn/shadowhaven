using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    private GameObject[] objects;

    void Start()
    {
        GameObject spawn = null;

        if (transform.name.Contains("Wall") || transform.name.Contains("Floor"))
        {
            List<GameObject> temp = new List<GameObject>();

            string[] s = transform.name.Split('-');
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
            Instantiate(spawn, transform);
    }
}
