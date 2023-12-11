using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    private GameObject[] objects;

    void Start()
    {
        bool destroyEvenMyParent = false;
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
        else
        {
            objects = GameObject.FindGameObjectsWithTag(transform.name);
            int rand = Random.Range(0, objects.Length);
            if (objects.Length != 0)
                spawn = objects[rand];
        }
        if (spawn != null)
        {

            if (name.Contains("Door"))
            {
                spawn = Instantiate(spawn, transform.position, Quaternion.identity, transform.parent.parent);

                DoorBehavior beh = spawn.GetComponent<DoorBehavior>();
                // Ak dvere niesu priamo pod stredom miestnosti tak su vertikalne
                switch (transform.parent.name.Split('=')[0])
                {
                    case "Boss": beh.type = DoorType.BossIn; break;
                    case "Loot": beh.type = DoorType.BossOut; break;
                    case "Spawn": beh.type = DoorType.Spawn; break;
                    case "Path": beh.type = DoorType.Locked; break;
                }
                beh.vertical = transform.position.x != transform.parent.position.x;
                destroyEvenMyParent = true;
            }
            else
                spawn = Instantiate(spawn, transform.position, Quaternion.identity, transform.parent);
            spawn.tag = "Untagged";
            spawn.name = spawn.name.Split('(')[0];
        }
        else
            Debug.LogWarning($"{name} is not a tag nor special");

        // Destruction
        if (destroyEvenMyParent)
            Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}
