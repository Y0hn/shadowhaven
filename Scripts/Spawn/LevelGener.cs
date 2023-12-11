using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;

public class LevelGener : MonoBehaviour
{
    public Transform[] startingPos;

    private Dictionary<string, int> RoomTypes;
    private List<GameObject> rooms;
    private Dictionary<string, int> DoorTypes;
    private List<GameObject> doors;

    public float minX;
    public float maxX;
    public float maxY;

    public Vector2 moveAmount;
    public string doorSize = "2x1";

    private string[] roomer;

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
        RoomTypes = new();
        rooms = new();
        DoorTypes = new();
        doors = new();
        roomer = new string[2];

        // Learnin Dictionary Rooms
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

        // Learnin Dictionary Doors
        doors.AddRange(Resources.LoadAll<GameObject>("Rooms/Doors/Arangement"));
        foreach (GameObject door in doors)
            DoorTypes.Add(door.name, doors.IndexOf(door));

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
            GameObject R;/*
            try
            {*/
                if (path)
                {
                    // Generate Path Room
                    R = Instantiate(GeneratePath(), transform.position, Quaternion.identity, transform.parent);
                    
                    R.name = "Path=" + NameCrop(R.name);
                    Move();
                }
                else if (startEnd)
                {
                    string s = "";
                    if (pastDir == -1)
                        s = "Spawn=";
                    else
                        s = "Loot=";

                    R = Instantiate(GenerateSpawn(), transform.position, Quaternion.identity, transform.parent);
                    R.name = s + NameCrop(R.name);
                    GenerateDoor(roomer[0], false, R.transform);
                    Move();
                }
                else
                {
                    R = GenerateBoss();
                    R.name = "Boss=" + NameCrop(R.name);
                    GenerateDoor(roomer[1], false, R.transform);
                    GenerateDoor(roomer[1], true, R.transform);
                    SpawnBoss(R);
                }/*
            }
            catch 
            {
                Debug.LogWarning("Something is wrong with Level Generator D:");
                stop = true;
            }*/
        }
        else
        {
            if (deleteAssets)
            {
                Destroy(GameObject.FindGameObjectWithTag("Assets"));
                Destroy(gameObject);
            }
        }
    }
    private string NameCrop(string s, char spliter = ' ')
    {
        s = s.Split('(')[0];
        string[] ret = s.Split(spliter);
        ret[0] = "";
        for (int i = 1; i < ret.Length; i++)
        {
            ret[0] += ret[i];

            if (i < ret.Length - 1)
                ret[0] += "_";
        }
        return ret[0];
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
    private GameObject GenerateSpawn()
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
    private GameObject GenerateBoss()
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
        startEnd = true;
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
    private void GenerateDoor(string size, bool byDir, Transform room)
    {
        GameObject spawner = null;
        //Debug.Log($"Spawnning doors of size {size} with use of dir {byDir} in parantage of {room.name}");

        if (pastDir == -1)
        {
            byDir = true;
        }
        //Debug.Log($"Indexes for doors in DoorTypes: \nL = {size+"L"} index: {DoorTypes[size + "L"]}\nR = {size+"R"} index: {DoorTypes[size + "R"]}\nFromD = {"FromD"} index: {DoorTypes["FromD"]}");
        if (byDir)
            switch (dir)
            {
                case 0: // from DOWN door
                case 5:
                    spawner = doors[DoorTypes[size + "U"]];
                    break;
                case 1: // LEFT  door
                case 3:
                    spawner = doors[DoorTypes[size + "L"]];
                    break;
                case 2: // RIGHT door
                case 4:
                    spawner = doors[DoorTypes[size + "R"]];
                    break;
            }
        else
            switch (pastDir)
            {
                case 0: // from DOWN door
                case 5:
                    spawner = doors[DoorTypes["FromD"]];
                    break;
                case 1: // from LEFT  door
                case 3:
                    spawner = doors[DoorTypes[size + "L"]];
                    break;
                case 2: // RIGHT door
                case 4:
                    spawner = doors[DoorTypes[size + "R"]];
                    break;
            }

        if (spawner != null)
            Instantiate(spawner, transform.position, Quaternion.identity, room);
    }
}