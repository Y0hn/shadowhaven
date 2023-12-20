using System.Collections.Generic;
using UnityEngine;

public class ChainSpawnBehavior : MonoBehaviour
{
    public float count;
    public float moveAmout;
    public float spawnDelay = 0.1f;
    public GameObject spawnObject;
    public Vector3 target;
    public bool randomizedMovement = false;

    private float timer = 0f;
    private Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        enabled = false;
    }
    void Update()
    {
        if (timer < Time.time)
        {
            Instantiate(spawnObject, transform.position, Quaternion.identity, transform);
            Move();
            timer = Time.time + spawnDelay;
        }
    }
    void Move()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, target, moveAmout);
    }
    private void OnEnable()
    {
        timer = 0f;
    }
    private void OnDisable()
    {
        transform.localPosition = startPos;
    }
}
