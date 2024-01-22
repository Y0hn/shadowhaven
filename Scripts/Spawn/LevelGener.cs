using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;

public class LevelGener : MonoBehaviour
{
    public Transform[] startingPos;

    private Dictionary<string, GameObject> roomContent;
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
        roomer = new string[2];
        roomContent = new();
        RoomTypes = new();
        rooms = new();
        // Learn Spawn
        spawnObj = Resources.LoadAll<GameObject>("PreFabs/Spawner")[0];

        // Learnin Dictionary Rooms
        switch (transform.parent.name)
        {
            case "Level_01":
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 10x10"));
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 20x20"));
                roomer = new string[] { "10x10", "10x10", "20x20" };
                //                       SPAWN    PATH     BOSS 
                break;

            case "Level_02":
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 10x10"));
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 20x20"));
                roomer = new string[] { "10x10", "10x10", "20x20" };
                Destroy(gameObject);
                break;
            default:
                Debug.LogWarning("LevelGenerator destroied!");
                Destroy(gameObject);
                break;
        }

        // Gettin PreFabs for Rooms into Dictionary
        foreach (GameObject room in rooms)
        {
            string[] s = room.name.Split(' ');
            if (s.Length > 2)
            {
                if      (room.name.Contains("_"))
                {
                    s[0] = "Spawn"; // or END
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
                // exmple: s[0] = "Boss20x20DL"
            }

            //Debug.Log($"Added record to dictionary ({s[0]},{rooms.IndexOf(room)})");
            RoomTypes.Add(s[0], rooms.IndexOf(room));
        }
        // Learnin Room Contents
        foreach (GameObject loot in Resources.LoadAll<GameObject>($"Rooms/Content/Items/{roomer[0]}"))
        {
            roomContent.Add("Loot"+loot.name, loot);
        }
        foreach (GameObject enemies in Resources.LoadAll<GameObject>($"Rooms/Content/Enemies/{roomer[1]}"))
        {
            roomContent.Add("Enemy"+enemies.name, enemies);
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
            GameObject R;   // Room
            GameObject C;   // Contents
            //try
            {
                if (path)
                {
                    // Generate Path Room
                    string s = "Path=";
                    R = Instantiate(GeneratePath(), transform.position, Quaternion.identity, transform.parent);
                    R.name = s + NameCrop(R.name);

                    C = Instantiate(GenerateContent(s + "Enemy"), R.transform.position, Quaternion.identity, R.transform);
                    C.name = NameCrop(C.name, true);
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
                    R.name = NameCrop(R.name);
                    GenerateDoor(R.transform);
                    R.name = s + R.name;

                    C = Instantiate(GenerateContent(s+"-"), R.transform.position, Quaternion.identity, R.transform);
                    C.name = NameCrop(C.name, true);
                    Move();
                }
                else
                {
                    R = GenerateBoss();
                    R.name = "Boss=" + NameCrop(R.name);
                    GenerateDoor(R.transform);
                    SpawnBoss(R);
                }
            }/*
            catch 
            {
                Debug.LogWarning("Something is wrong with Level Generator D:");
                Debug.LogAssertion("Available Generator KEYS:" + GetOutAllDictionaries());
                stop = true;
            }*/
        }
        else
        {
            if (deleteAssets)
            {
                Destroy(GameObject.FindGameObjectWithTag("Assets"));
                Destroy(gameObject);
                return;
            }
        }
    }
    private string NameCrop(string s,bool simle = false, char spliter = ' ')
    {
        s = s.Split('(')[0];
        if (!simle)
        {
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
        return s;
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
    private GameObject GenerateContent(string content)
    {
        string[] temp = content.Split('=');
        switch (temp[0])
        {
            case "Spawn":
                temp[0] = "Loot";
                temp[1] = "Starter";
                break;
            case "Loot":
                temp[1] = "Chest";
                break;
            case "Path":
            //case "Enemy":
                temp[0] = "Enemy";
                switch (Random.Range(0,3))
                {
                    case 0:
                        temp[1] = "MM";
                        break;
                    case 1:
                        temp[1] = "RR";
                        break;
                    case 2:
                        temp[1] = "MR";
                        break;
                }
                break;
        }            

        return roomContent[temp[0]+temp[1]];
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
    private string GetOutAllDictionaries()
    {
        string ret = "\n";

        ret += "\nRoom Contents Dictionary:\n{";
        foreach (string key in roomContent.Keys)
        {
            ret += key + "\t";
        }
        ret += "}\n";

        ret += "\nRoom Types Dictionary\n{";
        foreach (string key in RoomTypes.Keys)
        {
            ret += key + "\t";
        }
        ret += "}\n";

        return ret;
    }
}