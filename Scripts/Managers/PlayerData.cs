using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int health;
    public int maxHealth;
    public Equipment[] equipment;
    public float[] position;

    public PlayerData (PlayerScript player, PlayerStats stats)
    {
        level = stats.level;
        health = stats.curHealth;
        maxHealth = stats.maxHealth;
        equipment = Inventory.instance.GetEquipment();

        Vector2 pos = player.GetPos();
        position = new float[2];
        position[0] = pos.x;
        position[1] = pos.y;
    }
}
