using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGener : MonoBehaviour
{
    public Transform[] startingPos;
    public GameObject[] rooms;
    /*  ROOM TYPES
     * 0 - DL
     * 1 - DR
     * 2 - LR
     * 3 - UD
     * 4 - UL
     * 5 - UR
     * 6 - LRD
     * 7 - LRU
     * 8 - LRBU
     */
    //private float nextRoom;
    //private float intervalRoom;

    public float minX;
    public float maxX;
    public float maxY;

    public Vector2 moveAmount;

    private int dir = -1;
    private int pastDir = -1;

    private bool stop;
    private bool done;

    void Start()
    {
        // References
        Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        // Set Up
        int randStartPos = Random.Range(0, startingPos.Length);
        transform.position = startingPos[randStartPos].position;
        player.position = startingPos[randStartPos].position;
        stop = false;
        done = false;
        //nextRoom = 0; intervalRoom = 0.5f;
        //Debug.Log(startingPos[randStartPos].position);

        // Inicializatin
        dir = Random.Range(0, 6);
        if      (transform.position.x >= maxX)
            dir = Random.Range(0, 2);
        else if (transform.position.x <= minX)
            dir = Random.Range(4, 6);
    }
    void Update()
    {
        if (!stop /*&& Time.time >= nextRoom*/)
        {
            if (!done)
            {
                // Do the thing :D
                SpawnRoom();
                pastDir = dir;
                Move();
            }
            else
            {
                // Generate Boss Room
                transform.position = new Vector2(transform.position.x, transform.position.y + moveAmount.y);
                Instantiate(rooms[9], transform.position, Quaternion.identity, transform.parent);
                stop = true;
            }

            // Slowness
            //nextRoom = Time.time + intervalRoom;
        }
    }
    private void Move()
    {
        Vector2 newPos = transform.position;

        switch (dir)
        {
            case 1:     // LEFT
            case 3:
                newPos = new Vector2(transform.position.x - moveAmount.x, transform.position.y);

                dir = Random.Range(0, 6);
                if (dir == 2)
                    dir = 1;
                else if (dir == 4)
                    dir = 3;

                if (newPos.x <= minX)
                    dir = 0;

                break;
            case 2:     // RIGHT
            case 4:
                newPos = new Vector2(transform.position.x + moveAmount.x, transform.position.y);

                dir = Random.Range(0, 6);
                if (dir == 1)
                    dir = 2;
                else if (dir == 3)
                    dir = 4;

                if (newPos.x >= maxX)
                    dir = 5;
                break;
            case 0:     // UP
            case 5:
                if (transform.position.y < maxY)
                    newPos = new Vector2(transform.position.x, transform.position.y + moveAmount.y);
                else
                    done = true;

                dir = Random.Range(0, 6);
                if      (newPos.x >= maxX)
                    dir = Random.Range(0, 2);
                else if (newPos.x <= minX)
                    dir = Random.Range(4, 6);

                break;
        }

        transform.position = newPos;
    }
    private void SpawnRoom()
    {
        int type = 8;

        if (pastDir == -1)
        {
            switch (dir)
            {
                case 1:
                case 3:
                    type = 11;
                    break;
                case 2: 
                case 4:
                    type = 12;
                    break;
                case 0:
                case 5:
                    type = 13;
                    break;
            }
        }
        else if (pastDir == 1 || pastDir == 3)      // FROM RIGHT
        {
            if      (dir == 1 || dir == 3)
                type = 2;
            else if (dir == 0 || dir == 5)
                type = 5;
        }
        else if (pastDir == 2 || pastDir == 4)      // FROM LEFT
        {
            if      (dir == 2 || dir == 4)
                type = 2;
            else if (dir == 0 || dir == 5)
                type = 4;
        }   
        else if (pastDir == 0 || pastDir == 5)                      // FROM BOTTOM
        {
            if      (dir == 1 || dir == 3)
                type = 0;
            else if (dir == 2 || dir == 4)
                type = 1;
            else if (dir == 0 || dir == 5)
                type = 3;
        }

        Instantiate(rooms[type], transform.position, Quaternion.identity, transform.parent);
    }
    private void OnDrawGizmosSelected()
    {
        float a, b;
        Vector2 size, pos;
        Gizmos.color = Color.yellow;

        a = (maxX - minX) + moveAmount.x;
        b = (maxY - startingPos[0].position.y) + moveAmount.y;
        size = new Vector2(a,b);

        Gizmos.DrawWireCube(transform.position, size);

        Gizmos.color = Color.red;

        a = transform.position.x;
        b = startingPos[0].position.y + moveAmount.x * startingPos.Length + moveAmount.y * 0.5f;
        pos = new Vector2 (a,b);
        a = startingPos.Last().position.x - startingPos[0].position.x + 3 * moveAmount.x;
        b = 2 * moveAmount.y;
        size = new Vector2(a, b);

        Gizmos.DrawWireCube(pos, size);
    }
}
