using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int health;
    public float[] position;

    public PlayerData (PlayerScript player)
    {
        level = player.GetLevel();
        health = player.GetHealth();

        position = new float[2];
        position[0] = player.rb.position.x;
        position[1] = player.rb.position.y;
    }
}
