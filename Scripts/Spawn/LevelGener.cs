using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelGener : MonoBehaviour
{
    #region References
    public Transform[] startingPos;

    private Dictionary<string, GameObject> roomContent;
    private Dictionary<string, int> RoomTypes;
    private List<GameObject> rooms;
    private GameObject spawnObj;

    public float minX;
    public float maxX;
    public float maxY;
    #endregion
    public float globalLightIntensity;
    public SaveSystem.Data data;
    public Vector2 moveAmount;
    public string doorSize;
    private string[] roomer;

    private int dir = -1;
    private int pastDir = -1;
    private Vector2 pastPos;

    private bool stop = false;
    private bool path;
    private bool startEnd;

    void Start()
    {
        if (!stop)
        {
            // References
            Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            LoadAssets();
            // Set Up
            int randStartPos = Random.Range(0, startingPos.Length);
            transform.position = startingPos[randStartPos].position;
            GameManager.instance.player.transform.position = startingPos[randStartPos].position;
            GameManager.instance.SetGlobalLight(globalLightIntensity);
            startEnd = true;
            //stop = false;
            path = false;
            // Inicializatin
            dir = Random.Range(0, 6);
            if (transform.position.x >= maxX)
                dir = Random.Range(0, 2);
            else if (transform.position.x <= minX)
                dir = Random.Range(4, 6);
        }
    }
    void Update()
    {
        if (!stop)
        {
            GameObject R;   // Room
            GameObject C;   // Contents

            if (path)
            {
                // Generate Path Room
                string s = "Path=";
                R = Instantiate(GeneratePath(), transform.position, Quaternion.identity, GetRoomSpawnPath());
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

                R = Instantiate(GenerateSpawn(), transform.position, Quaternion.identity, GetRoomSpawnPath());
                pastPos = R.transform.position;
                R.name = NameCrop(R.name);
                if (s.Equals("Loot="))
                {
                    GenerateDoor(R.transform);
                    GenerateGate(R.transform);
                }
                R.name = s + R.name;

                C = Instantiate(GenerateContent(s + "-"), R.transform.position, Quaternion.identity, R.transform);
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
        }
        else
        {
            End();
        }
    }
    void LoadAssets()
    {
        roomer = new string[2];
        roomContent = new();
        RoomTypes = new();
        rooms = new();
        // Learn Spawn
        spawnObj = Resources.LoadAll<GameObject>("Objects/Spawner")[0];
        // Learnin Dictionary Rooms
        switch (transform.parent.name)
        {
            case "Level_1":
            case "Level_var-01":
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 10x10"));
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 20x20"));
                roomer = new string[] { "10x10", "10x10", "20x20" };
                //                       SPAWN    PATH     BOSS 
                doorSize = "2x1";
                break;

            case "Level_2":
            case "Level_var-02":
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 10x10 E"));
                rooms.AddRange(Resources.LoadAll<GameObject>("Rooms/Templates/Tem 20x20 E"));
                roomer = new string[] { "10x10", "10x10", "20x20" };
                doorSize = "2x1-E";
                break;
            default:
                //Debug.LogWarning($"Level Generator of {transform.parent.name} was destroied!");
                Destroy(gameObject);
                break;
        }
        // Gettin PreFabs for Rooms into Dictionary
        foreach (GameObject room in rooms)
        {
            string[] s = room.name.Split(' ');
            if (s.Length > 2)
            {
                if (room.name.Contains("_"))
                {
                    s[0] = "Spawn="; // or END
                }
                else if (s[1].Equals(roomer[1]))
                {
                    s[0] = "Path=";
                }
                else if (s[1].Equals(roomer[2]))
                {
                    s[0] = "Boss=";
                }

                s[0] += s[1] + "-" + s[2];
                // exmple: s[0] = "Boss=20x20-DL"
            }

            //Debug.Log($"Added record to dictionary ({s[0]},{rooms.IndexOf(room)})");
            RoomTypes.Add(s[0], rooms.IndexOf(room));
        }
        // Learnin Room Contents
        foreach (GameObject loot in Resources.LoadAll<GameObject>($"Rooms/Content/Items/{roomer[0]}"))
        {
            roomContent.Add("Loot" + loot.name, loot);
        }
        foreach (GameObject enemies in Resources.LoadAll<GameObject>($"Rooms/Content/Enemies/{roomer[1]}"))
        {
            roomContent.Add("Enemy" + enemies.name, enemies);
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
                temp[1] = "Start";
                break;
            case "Loot":
                temp[1] = "Finish";
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
        string s = "Path=" + roomer[1];
        if      (pastDir == 1 || pastDir == 3)      // FROM RIGHT
        {
            if      (dir == 1 || dir == 3)  // to right
                s += "-LR";
            else if (dir == 0 || dir == 5)  // to up
                s += "-UR";
        }
        else if (pastDir == 2 || pastDir == 4)      // FROM LEFT
        {
            if      (dir == 2 || dir == 4)  // to left
                s += "-LR";
            else if (dir == 0 || dir == 5)  // to up
                s += "-UL";
        }   
        else if (pastDir == 0 || pastDir == 5)      // FROM BOTTOM
        {
            if      (dir == 1 || dir == 3)  // to left
                s += "-DL";
            else if (dir == 2 || dir == 4)  // to right
                s += "-DR";
            else if (dir == 0 || dir == 5)  // to up
                s += "-UD";
        }

        pastDir = dir;
        return rooms[RoomTypes[s]];
    }
    private GameObject GenerateSpawn()
    {
        string s = "Spawn=" + roomer[0];

        if (pastDir != -1)
        {

            switch (pastDir)
            {
                case 1: case 3: s += "-_R"; break;   // FROM LEFT
                case 2: case 4: s += "-_L"; break;   // FROM RIGHT
                case 0: case 5: s += "-D_"; break;   // FROM DOWN
            }
            stop = true;
        }
        else
        {
            switch (dir)
            {
                case 1: case 3: s += "-_L"; break;   // Left
                case 2: case 4: s += "-_R"; break;   // Right
                case 0: case 5: s += "-U_"; break;   // Up
            }
            path = true;
        }

        pastDir = dir;
        startEnd = false;
        return rooms[RoomTypes[s]];
    }
    private GameObject GenerateBoss()
    {
        string s = "Boss=" + roomer[2].Split('_')[0];

        dir = Random.Range(0, 3);
        transform.position = new Vector2(transform.position.x, transform.position.y + moveAmount.y);
        pastPos = transform.position;

        switch (dir)
        {
            case 1: case 3: 
                s += "-DR"; 
                transform.position = 
                    new Vector2(transform.position.x - 1.5f * moveAmount.x, transform.position.y + moveAmount.y);
                break;
            case 2: case 4: 
                s += "-DL";
                transform.position =
                    new Vector2(transform.position.x + 1.5f * moveAmount.x, transform.position.y + moveAmount.y);
                break;
            case 0: case 5: 
                s += "-DU";
                transform.position =
                    new Vector2(transform.position.x, transform.position.y + 2 * moveAmount.y);
                break;
        }

        // Boss Room
        GameObject room = Instantiate(rooms[RoomTypes[s]], pastPos, Quaternion.identity, GetRoomSpawnPath());
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
    private void GenerateGate(Transform room)
    {
        GameObject spawner;
        Vector3 pos = new(room.position.x, room.position.y + 5.5f, -1);
        spawner = Instantiate(spawnObj, pos, Quaternion.identity, room);
        // spawner.name => "Door-2x1"
        spawner.name = "Wall-GATE";
    }
    private void End()
    {
        GameManager.audio.PlayTheme("theme" + GameManager.instance.level);
        GameObject.FindGameObjectWithTag("Assets").SetActive(false);
        enabled = false;
    }
    private Transform GetRoomSpawnPath()
    {
        return transform.parent;
    }
    public void LoadFromData(SaveSystem.Data data)
    {
        enabled = false;
        GameObject inter = Resources.Load<GameObject>("Objects/item");
        GameObject.FindGameObjectWithTag("Assets").SetActive(true);
        GameManager.instance.SetGlobalLight(globalLightIntensity);
        Transform spawned = transform;
        spawned.name = "spawned";
        // Building Rooms
        SaveSystem.Data.LevelData levelData = data.level;
        LoadAssets();
        for (int i = 0; i < levelData.rooms.Length; i++) 
        {
            if (levelData.rooms[i].Contains("Loot"))
                levelData.rooms[i] = "Spawn" + "=" + levelData.rooms[i].Split('=')[1];
            GameObject r = Instantiate(rooms[RoomTypes[levelData.rooms[i]]], levelData.roomsPos[i].GetVector(), Quaternion.identity, GetRoomSpawnPath());
            pastPos = r.transform.position;
            r.name = levelData.rooms[i];

            if (r.name.Contains("Spawn") && i != 0)
            {
                r.name = "Loot=" + r.name.Split('=')[1];
                GenerateGate(r.transform);
                GenerateDoor(r.transform);
                GameObject C = Instantiate(GenerateContent(r.name), r.transform.position, Quaternion.identity, r.transform);
                C.name = NameCrop(C.name, true);
            }
            else if (r.name.Contains("Boss"))
            {
                GenerateDoor(r.transform);
            }
            //Debug.Log($"Loaded room \"{r.name}\" on position [{levelData.roomsPos[i]}]");
        }
        // Insantiatie enemies
        GameObject[] enemies = Resources.LoadAll<GameObject>("Objects/Enemies");
        GameObject[] bosses = Resources.LoadAll<GameObject>("Objects/Bosses");
        for (int i = 1; i < data.entities.Length; i++)
        {
            if (i < data.entities.Length - 1)
            {
                int index = -1;
                for (int j = 0; j < enemies.Length; j++)
                    if (data.entities[i].charName == enemies[j].name)
                    {
                        index = j;
                        break;
                    }
                if (index != -1)
                {
                    GameObject ee = Instantiate(enemies[index], data.entities[i].position.GetVector(), Quaternion.identity, spawned);
                    ee.GetComponent<EnemyStats>().LoadHealth(data.entities[i].curHealth, data.entities[i].maxHealth);
                    ee.name = ee.name.Split('(')[0];
                    ee.tag = "Enemy";
                    //Debug.Log($"Enemy {e.charName} spawned");
                }
                else
                    Debug.Log("Indestingusable enemy saved name: " + data.entities[i].charName);
            }
            else // Boss
            {
                foreach (GameObject b in bosses)
                {
                    if (data.entities[i].charName == b.name)
                    {
                        GameObject bo = Instantiate(b, data.entities[i].position.GetVector(), Quaternion.identity, spawned);
                        bo.name = bo.name.Split('(')[0];
                        BossStats bs = bo.GetComponent<BossStats>();
                        bs.LoadHealth(data.entities[i].curHealth, data.entities[i].maxHealth);
                        bs.SetY(bs.transform.position.y - 10);
                        bs.enabled = true;
                        
                        //Debug.Log("Boss spawned " + bo.name);
                    }
                }
            }
        }
        // Loading interactable items
        for (int i = 0; i < data.interactables.items.Length; i++)
        {
            GameObject interact = Instantiate(inter, data.interactables.positions[i].GetVector(), Quaternion.identity, GetRoomSpawnPath());
            interact.GetComponent<Interactable>().item = data.interactables.items[i].GetItem();
        }
        GameManager.instance.player.transform.position = data.entities[0].position.GetVector();
        GameManager.instance.generated = true;
        transform.parent.name += "_Loaded";
        enabled = true;
        stop = true;
    }
}