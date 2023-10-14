using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    private Dictionary<int, string> Inventory = new Dictionary<int, string>
    {
        { 0, "itemName_"   + "itemClass_"   + "damage/armor_"+ "baseAttackRate_" + " attackDamage"},
        { 1, "Iron Sword_" + "melee-weapon_"+ "5_"           + "" +    "10 "},
        { 2, " "},
        { 3, " "},
        { 4, " "}
    };
    public int level;
    public int health;
    public float[] position;

    public PlayerData (PlayerScript player)
    {
        level = player.GetLevel();
        health = player.GetHealth();

        Vector2 pos = player.GetPos();
        position = new float[2];
        position[0] = pos.x;
        position[1] = pos.y;
    }
}
