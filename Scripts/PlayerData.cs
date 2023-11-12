using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int health;
    public Equipment[] equipment;
    public float[] position;

    public PlayerData (PlayerScript player)
    {
        level = player.GetLevel();
        health = player.GetHealth();
        equipment = Inventory.instance.GetEquipment();

        Vector2 pos = player.GetPos();
        position = new float[2];
        position[0] = pos.x;
        position[1] = pos.y;
    }
}
