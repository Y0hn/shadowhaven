using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnListRand : MonoBehaviour
{
    public GameObject[] spawnList;

    void Start()
    {
        int rand = Random.Range(0, spawnList.Length);
        Instantiate(spawnList[rand], transform.position, Quaternion.identity);
    }
}
