using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnListRand : MonoBehaviour
{
    public GameObject[] spawnList;

    void Start()
    {
        if (spawnList.Length != 0)
        {
            int rand = Random.Range(0, spawnList.Length);
            Instantiate(spawnList[rand], transform);
        }
    }
}
