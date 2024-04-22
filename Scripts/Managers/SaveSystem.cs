using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
public static class SaveSystem
{
    public static bool fileDataLoaded = false;
    private static bool debug = false;
    public static bool CheckSaveNeed()
    {
        return CompareData(GetData(), Load());
    }
    public static bool SaveDataExist(string filename = "data")
    {
        Data d = Load();
        return DataCheck(d);
    }
    private static bool DataCheck(Data data)
    {
        bool check = true;
        try
        {
            // Level number
            check &= data != null;
            check &= data.curLevel >= 0;
            check &= data.level.roomsPos.Length == data.level.rooms.Length;


            // Entities => min. 1 {Player}
            check &= data.entities.Length >= 1;
            foreach (Data.CharakterData e in data.entities)
                check &= e.curHealh >= 0 && e.charName != "" && e.position != null;

            // Inventory + Equipment DUPLICATE CHECK
            check &= data.inventory.items.Count >= 0;
            check &= data.inventory.equipment.Length >= 0;
            List<ItemData> list = new();
            for (int i = 0, e = 0; (i < data.inventory.items.Count || data.inventory.equipment.Length > e) && check; i++, e++)
            {
                if (i < data.inventory.items.Count)
                {
                    check &= !list.Contains(data.inventory.items[i]);
                    list.Add(data.inventory.items[i]);
                }
                if (e < data.inventory.equipment.Length)
                {
                    check &= !list.Contains(data.inventory.equipment[e]);
                    list.Add(data.inventory.equipment[e]);
                }
            }
            list.Clear();

            // Interactables
            check &= data.interactables.positions.Length == data.interactables.items.Length;
        }
        catch
        {
            check = false;
        }
        if (check)
            Debug.Log("Passed data check!");
        else
            Debug.Log("Failed data check!");
        return check;
    }
    private static bool CompareData(Data d1, Data d2)
    {
        bool compare = DataCheck(d1) && DataCheck(d2);

        // Cur Level
        compare &= d1.curLevel == d2.curLevel;
        DebugLogForComperators(compare, "Current Level");
        // Inventory
        compare &= d1.inventory.items.Count == d2.inventory.items.Count;
        if (compare)
            for (int i = 0; i < d1.inventory.items.Count && compare; i++)
                compare &= d1.inventory.items[i].name == d2.inventory.items[i].name;
        DebugLogForComperators(compare, "Inventory");
        // Equipment
        compare &= d1.inventory.equipment.Length == d2.inventory.equipment.Length;
        if (compare)
            for (int i = 0; i < d1.inventory.equipment.Length && compare; i++)
                compare &= d1.inventory.equipment[i].name == d2.inventory.equipment[i].name;
        DebugLogForComperators(compare, "Equipment");
        // Level
        compare &= d1.level.rooms.Length == d2.level.rooms.Length;
        if (compare)
            for (int i = 0; i < d1.level.rooms.Length && compare; i++)
                compare &= d1.level.rooms[i] == d2.level.rooms[i];
        DebugLogForComperators(compare, "Level");
        // Interactable
        compare &= d1.interactables.items.Length == d2.interactables.items.Length;
        if (compare)
            for (int i = 0; i < d1.interactables.items.Length && compare; i++)
                compare &= d1.interactables.items[i].name == d2.interactables.items[i].name;
        DebugLogForComperators(compare, "Interactable");
        // Entities
        compare &= d1.entities.Length == d2.entities.Length;
        if (compare)
            foreach (Data.CharakterData entity in d1.entities)
            {
                bool found = false;
                foreach (Data.CharakterData refer in d2.entities)
                    if (entity.position.GetVector() == refer.position.GetVector())
                    {
                        compare &= entity.charName == refer.charName;
                        compare &= entity.curHealh == refer.curHealh;
                        found = true;
                        break;
                    }
                compare &= found;
                if (!compare)
                {
                    Debug.Log($"{entity.charName} not found in save data");
                    break;
                }
            }
        DebugLogForComperators(compare, "Entities");
        DebugLogDataOut(d1, "compare to d2");
        DebugLogDataOut(d2, "compare to d1");

        return compare;
    }
    private static void DebugLogForComperators(bool pass, string message)
    {
        if (debug)
        {
            if (pass)
                Debug.Log(message + " passed!");
            else
                Debug.Log(message + " failed!");
        }
    }
    private static void DebugLogDataOut(Data data, string action)
    {
        if (debug)
            Debug.Log($"[{Time.time}] Data {action} succesfully\nData:" +
                        $"\n\tPlayer [{data.entities[0].position}]" +
                        $"\n\tNumber of Rooms [{data.level.rooms.Length}] on level {data.curLevel}" +
                        $"\n\tNumber of Items [{data.interactables.items.Length}]" +
                        $"\n\tNumber of Enemies [{data.entities.Length - 1}]" +
                        $"\n\tBoss {data.entities.Last().charName} is on pos [{data.entities.Last().position}]");
    }
    public static void Save()
    {
        BinaryFormatter formatter = new();
        FileStream stream = new(Path("data"), FileMode.Create);

        Data data = GetData();
        formatter.Serialize(stream, data);
        stream.Close();
        DebugLogDataOut(data, "saved");
    }
    private static Data GetData()
    {
        // Entities DATA
        List<Data.CharakterData> charakterData = new()
        { GameManager.instance.playerStats.SaveData() };
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
            charakterData.Add(e.GetComponent<EnemyStats>().SaveData());
        if (GameManager.instance.boss != null)
            charakterData.Add(GameManager.instance.boss.SaveData());

        return new(charakterData);
    }
    public static Data Load()
    {
        if (File.Exists(Path("data")))
        {
            BinaryFormatter formatter = new();
            FileStream steam = new(Path("data"), FileMode.Open);

            Data data = formatter.Deserialize(steam) as Data;
            steam.Close();
            DebugLogDataOut(data, "loaded");
            return data;
        }
        else
        {
            Debug.LogError("Savefile not found: " + Path("data"));
            return null;
        }
    }
    private static string Path(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".file";
        return path;
    }

    [System.Serializable]
    public class Data
    {
        public InteractableData interactables;
        public CharakterData[] entities;
        public InventoryData inventory;
        public LevelData level;
        public int curLevel;
        public Data()
        {
            interactables = null;
            inventory = null;
            entities = null;
            level = null;
            curLevel = -1;
        }
        public Data(List<CharakterData> chars)
        {
            entities = chars.ToArray();
            inventory = GameManager.inventory.Save();
            level = new (GameObject.FindGameObjectWithTag("Level").transform);
            curLevel = GameManager.instance.level;

            // Items
            List<Interactable> inter = new List<Interactable>();
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Item"))
                inter.Add(g.GetComponent<Interactable>());
            interactables = new(inter.ToArray());
        }
        [System.Serializable]
        public class CharakterData
        {
            public int curHealh;
            public string charName;
            public Position position;
            public CharakterData(string charName, int curHealh, Vector3 position)
            {
                this.curHealh = curHealh;
                this.charName = charName;
                this.position = new Position(position);
            }
        }
        [System.Serializable]
        public class InventoryData
        {
            public List<ItemData> items;
            public ItemData[] equipment;
            public InventoryData(List<Item> items, Equipment[] equipment)
            {
                this.items = new();
                foreach (Item i in items)
                    this.items.Add(new ItemData(i));

                this.equipment = new ItemData[equipment.Length];
                for (int i = 0; i < equipment.Length; i++)
                    this.equipment[i] = new ItemData(equipment[i]);
            }
        }
        [System.Serializable]
        public class LevelData
        {
            public string[] rooms;
            public Position[] roomsPos;
            public LevelData(Transform level)
            {
                Transform[] t = level.GetComponentsInChildren<Transform>();
                List<string> s = new();
                List<Position> pos = new();
                foreach (Transform n in t) 
                { 
                    if (n.name.Contains("="))
                    {
                        // is ROOM
                        s.Add(n.name);
                        pos.Add(new Position(n.position));
                    }
                }
                rooms = s.ToArray();
                roomsPos = pos.ToArray();
            }
        }
        [System.Serializable]
        public class InteractableData
        {
            public ItemData[] items;
            public Position[] positions;
            public InteractableData(Interactable[] items)
            {
                this.items = new ItemData[items.Length];
                positions = new Position[items.Length];
                for (int i = 0;i < items.Length;i++)
                {
                    this.items[i] = new ItemData(items[i].item);
                    positions[i] = new Position(items[i].transform.position);
                }                
            }
        }
        
        [System.Serializable]
        public class Position
        {
            public float x, y, z;
            public Position(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public Position(Vector3 v3)
            {
                this.x = v3.x;
                this.y = v3.y;
                this.z = v3.z;
            }
            public Vector3 GetVector()
            {
                return new Vector3(x, y, z);
            }
            public override string ToString()
            {
                return $"{x},{y},{z}";
            }
        }
    }
}