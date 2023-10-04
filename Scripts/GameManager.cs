using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerScript player;

    public void Save()
    {
        SaveSystem.SavePlayer(player);
    }
    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        player.SetHealth(data.health);
        player.SetLevel(data.level);
        player.rb.position = new Vector2(data.position[0], data.position[1]);
    }
}