using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;

public class LevelGener : MonoBehaviour
{
    public Transform[] startingPos;

    private Dictionary<string, int> RoomTypes;
    private List<GameObject> rooms;
    private GameObject spawnObj;

    public float minX;
    public float maxX;
    public float maxY;

    public Vector2 moveAmount;
    public string doorSize = "2x1";

    private string[] roomer;

    private int dir = -1;
    private int pastDir = -1;
    private Vector2 pastPos;

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
        roomer = new string[2];
        // Learn Spawn
        spawnObj = Resources.LoadAll<GameObject>("PreFabs/Spawner")[0];

        // Learnin Dictionary Rooms
        switch (transform.parent.name)
        {
            case "Level_01":
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 10x10"));
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 20x20"));
                roomer = new string[] { "10x10", "10x10", "20x20" };
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
                if      (room.name.Contains("_"))
                {
                    s[0] = "Spawn";
                }
                else if (s[1].Equals(roomer[1]))
                {
                    s[0] = "Path";
                }
                else if (s[1].Equals(roomer[2]))
                {
                    s[0] = "Boss";
                }
                    
                s[0] += s[1] + s[2];
            }
            //Debug.Log($"Added record to dictionary ({s[1]},{rooms.IndexOf(room)})");
            RoomTypes.Add(s[0], rooms.IndexOf(room));
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
            GameObject R;
            try
            {
                if (path)
                {
                    // Generate Path Room
                    R = Instantiate(GeneratePath(), transform.position, Quaternion.identity, transform.parent);
                    
                    R.name = "Path=" + NameCrop(R.name);
                    Move();
                }
                else if (startEnd)
                {
                    string s;
                    if (pastDir == -1)
                        s = "Spawn=";
                    else
                        s = "Loot=";

                    R = Instantiate(GenerateSpawn(), transform.position, Quaternion.identity, transform.parent);
                    pastPos = R.transform.position;
                    R.name = s + NameCrop(R.name);
                    Move();

                    GenerateDoor(R.transform);
                }
                else
                {
                    R = GenerateBoss();
                    R.name = "Boss=" + NameCrop(R.name);
                    GenerateDoor(R.transform);
                    SpawnBoss(R);
                }
            }
            catch 
            {
                Debug.LogWarning("Something is wrong with Level Generator D:");
                stop = true;
            }
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
                ret[0] += "-";
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
        string s = "Path" + roomer[1];
        if      (pastDir == 1 || pastDir == 3)      // FROM RIGHT
        {
            if      (dir == 1 || dir == 3)  // to right
                s += "LR";
            else if (dir == 0 || dir == 5)  // to up
                s += "UR";
        }
        else if (pastDir == 2 || pastDir == 4)      // FROM LEFT
        {
            if      (dir == 2 || dir == 4)  // to left
                s += "LR";
            else if (dir == 0 || dir == 5)  // to up
                s += "UL";
        }   
        else if (pastDir == 0 || pastDir == 5)      // FROM BOTTOM
        {
            if      (dir == 1 || dir == 3)  // to left
                s += "DL";
            else if (dir == 2 || dir == 4)  // to right
                s += "DR";
            else if (dir == 0 || dir == 5)  // to up
                s += "UD";
        }

        pastDir = dir;
        return rooms[RoomTypes[s]];
    }
    private GameObject GenerateSpawn()
    {
        string s = "Spawn" + roomer[0];

        if (pastDir != -1)
        {

            switch (pastDir)
            {
                case 1: case 3: s += "_R"; break;   // FROM LEFT
                case 2: case 4: s += "_L"; break;   // FROM RIGHT
                case 0: case 5: s += "D_"; break;   // FROM DOWN
            }
            stop = true;
        }
        else
        {
            switch (dir)
            {
                case 1: case 3: s += "_L"; break;   // Left
                case 2: case 4: s += "_R"; break;   // Right
                case 0: case 5: s += "U_"; break;   // Up
            }
            path = true;
        }

        pastDir = dir;
        startEnd = false;
        return rooms[RoomTypes[s]];
    }
    private GameObject GenerateBoss()
    {
        string s = "Boss" + roomer[2];

        dir = Random.Range(0, 3);
        transform.position = new Vector2(transform.position.x, transform.position.y + moveAmount.y);
        pastPos = transform.position;

        switch (dir)
        {
            case 1: case 3: 
                s += "DR"; 
                transform.position = 
                    new Vector2(transform.position.x - 1.5f * moveAmount.x, transform.position.y + moveAmount.y);
                break;
            case 2: case 4: 
                s += "DL";
                transform.position =
                    new Vector2(transform.position.x + 1.5f * moveAmount.x, transform.position.y + moveAmount.y);
                break;
            case 0: case 5: 
                s += "DU";
                transform.position =
                    new Vector2(transform.position.x, transform.position.y + 2 * moveAmount.y);
                break;
        }

        // Boss Room
        GameObject room = Instantiate(rooms[RoomTypes[s]], pastPos, Quaternion.identity, transform.parent);
        startEnd = true;
        return room;        
    }
    private void SpawnBoss(GameObject room)
    {
        Vector2 pos = room.transform.position;
        switch (roomer[2])
        {
            case "20x20":
                pos = new Vector2(pos.x, pos.y + 10);
                break;
            default:
                break;
        }

        GameObject spawner = Instantiate(spawnObj, pos, Quaternion.identity, room.transform);
        spawner.name = "Boss-" + (maxY + moveAmount.y);
        startEnd = true;
        pastDir = dir;
    }
    private void GenerateDoor(Transform room)
    {
        GameObject spawner;

        spawner = Instantiate(spawnObj, pastPos, Quaternion.identity, room);
        // spawner.name => "Door-2x1"
        spawner.name = "Door-" + doorSize;
    }
}