using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
public static class SaveSystem
{
    public static void Save()
    {
        BinaryFormatter formatter = new();
        FileStream stream = new(Path("data"), FileMode.Create);

        // Entities DATA
        List<Data.CharakterData> charakterData = new();
        charakterData.Add(GameManager.instance.playerStats.SaveData());
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
            charakterData.Add(e.GetComponent<EnemyStats>().SaveData());
        charakterData.Add(GameManager.instance.boss.SaveData());

        Data data = new(charakterData);
        formatter.Serialize(stream, data);

        stream.Close();
        Debug.Log($"[{Time.time}] Data saved succesfully\nData:" +
                    $"\n\tPlayer [{data.entities[0].position}]" +
                    $"\n\tNumber of Rooms [{data.level.rooms.Length}] on level {data.curLevel}" +
                    $"\n\tNumber of Items [{data.interactables.items.Length}]" +
                    $"\n\tNumber of Enemies [{data.entities.Length - 1}]" +
                    $"\n\tBoss {data.entities.Last().charName} is on pos [{data.entities.Last().position}]");
    }
    public static Data Load()
    {
        if (File.Exists(Path("data")))
        {
            BinaryFormatter formatter = new();
            FileStream steam = new(Path("data"), FileMode.Open);

            Data data = formatter.Deserialize(steam) as Data;
            steam.Close();
            Debug.Log($"[{Time.time}] Data loaded succesfully\nData:" +
                    $"\n\tPlayer [{data.entities[0].position}]" +
                    $"\n\tNumber of Rooms [{data.level.rooms.Length}] on level {data.curLevel}" +
                    $"\n\tNumber of Items [{data.interactables.items.Length}]" +
                    $"\n\tNumber of Enemies [{data.entities.Length - 1}]" +
                    $"\n\tBoss {data.entities.Last().charName} is on pos [{data.entities.Last().position}]");
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