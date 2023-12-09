using System.Collections.Generic;
using UnityEngine;

public class LevelGener : MonoBehaviour
{
    public Transform[] startingPos;

    private Dictionary<string, int> RoomTypes = new Dictionary<string, int>();
    private List<GameObject> rooms;

    public float minX;
    public float maxX;
    public float maxY;

    public Vector2 moveAmount;

    private int dir = -1;
    private int pastDir = -1;

    public bool deleteAssets = true;

    private bool stop;
    private bool path;
    private bool startEnd;

    void Start()
    {
        // References
        Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rooms = new List<GameObject>();
        string[] roomer = new string[0];

        switch (transform.parent.name)
        {
            case "Level_01":
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 10x10"));
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 20x20"));
                roomer = new string[] { "10x10", "20x20" };
                break;

            default:
                Debug.LogWarning("LevelGenerator destroied!");
                Destroy(gameObject);
                break;
        }

        foreach (GameObject room in rooms)
        {
            string[] s = room.name.Split(' ');
            if (s.Length > 2)
            {
                if      (s[1].Equals(roomer[0]))
                {
                    s[1] = "Path";
                }
                else if (s[1].Equals(roomer[1]))
                {
                    s[1] = "Boss";
                }
                    
                s[1] += s[2];
            }
            //Debug.Log($"Added record to dictionary ({s[1]},{rooms.IndexOf(room)})");
            RoomTypes.Add(s[1], rooms.IndexOf(room));
        }

        // Set Up
        int randStartPos = Random.Range(0, startingPos.Length);
        transform.position = startingPos[randStartPos].position;
        player.position = startingPos[randStartPos].position;
        startEnd = true;
        stop = false;
        path = false;

        // Inicializatin
        dir = Random.Range(0, 6);
        if      (transform.position.x >= maxX)
            dir = Random.Range(0, 2);
        else if (transform.position.x <= minX)
            dir = Random.Range(4, 6);
    }
    void Update()
    {
        if (!stop)
        {
            GameObject R = null;
            try
            {
                if (path)
                {
                    // Generate Path Room
                    R = Instantiate(GeneratePath(), transform.position, Quaternion.identity, transform.parent);
                    Move();
                }
                else if (startEnd)
                {
                    R = Instantiate(GenerateSpawnRoom(), transform.position, Quaternion.identity, transform.parent);
                    Move();
                }
                else
                {
                    R = GenerateBossRoom();
                    SpawnBoss(R);
                }

                if (R != null)
                {
                    string[] n = R.name.Split('(')[0].Split(' ');
                    n[0] = "";
                    for (int i = 1; i < n.Length; i++) 
                    {
                        n[0] += n[i] + " ";
                    }
                    R.name = n[0];
                }
            }
            catch 
            {
                Debug.LogWarning("Something is wrong with Level Gener :d");
                stop = true;
            }
        }
        else
        {
            if (deleteAssets)
                Destroy(GameObject.FindGameObjectWithTag("Assets"));
            Destroy(gameObject);
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
                    path = false;

                dir = Random.Range(0, 6);
                if      (newPos.x >= maxX)
                    dir = Random.Range(0, 2);
                else if (newPos.x <= minX)
                    dir = Random.Range(4, 6);

                break;
        }

        transform.position = newPos;
    }
    private GameObject GeneratePath()
    {
        int type = 0;

        if      (pastDir == 1 || pastDir == 3)      // FROM RIGHT
        {
            if      (dir == 1 || dir == 3)  // to right
                type = RoomTypes["PathLR"];
            else if (dir == 0 || dir == 5)  // to up
                type = RoomTypes["PathUR"];
        }
        else if (pastDir == 2 || pastDir == 4)      // FROM LEFT
        {
            if      (dir == 2 || dir == 4)  // to left
                type = RoomTypes["PathLR"];
            else if (dir == 0 || dir == 5)  // to up
                type = RoomTypes["PathUL"];
        }   
        else if (pastDir == 0 || pastDir == 5)      // FROM BOTTOM
        {
            if      (dir == 1 || dir == 3)  // to left
                type = RoomTypes["PathDL"];
            else if (dir == 2 || dir == 4)  // to right
                type = RoomTypes["PathDR"];
            else if (dir == 0 || dir == 5)  // to up
                type = RoomTypes["PathUD"];
        }

        pastDir = dir;
        return rooms[type];
    }
    private GameObject GenerateSpawnRoom()
    {
        int type = 0;

        if (pastDir != -1)
        {

            switch (pastDir)
            {
                case 1: case 3: type = RoomTypes["SR"]; break;   // FROM LEFT
                case 2: case 4: type = RoomTypes["SL"]; break;   // FROM RIGHT
                case 0: case 5: type = RoomTypes["SD"]; break;   // FROM DOWN
            }
            stop = true;
        }
        else
        {
            switch (dir)
            {
                case 1: case 3: type = RoomTypes["SL"]; break;   // Left
                case 2: case 4: type = RoomTypes["SR"]; break;   // Right
                case 0: case 5: type = RoomTypes["SU"]; break;   // Up
            }
            path = true;
        }

        pastDir = dir;
        startEnd = false;
        return rooms[type];
    }
    private GameObject GenerateBossRoom()
    {
        dir = Random.Range(0, 3);
        int type = 0;
        transform.position = new Vector2(transform.position.x, transform.position.y + moveAmount.y);
        Vector2 pos = transform.position;

        switch (dir)
        {
            case 1: case 3: 
                type = RoomTypes["BossR"]; 
                transform.position = 
                    new Vector2(transform.position.x - 1.5f * moveAmount.x, transform.position.y + moveAmount.y);
                break;
            case 2: case 4: 
                type = RoomTypes["BossL"];
                transform.position =
                    new Vector2(transform.position.x + 1.5f * moveAmount.x, transform.position.y + moveAmount.y);
                break;
            case 0: case 5: 
                type = RoomTypes["BossU"];
                transform.position =
                    new Vector2(transform.position.x, transform.position.y + 2 * moveAmount.y);
                break;
        }


        // Boss Room
        GameObject room = Instantiate(rooms[type], pos, Quaternion.identity, transform.parent);
        return room;        
    }
    private void SpawnBoss(GameObject room)
    {
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        GameObject boss = Instantiate(bosses[Random.Range(0, bosses.Length)], room.transform.position, Quaternion.identity, room.transform);
        boss.GetComponent<BossStats>().SetY(maxY + moveAmount.y);

        startEnd = true;
        pastDir = dir;
    }
}