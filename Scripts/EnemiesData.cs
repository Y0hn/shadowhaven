using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyData
{
    private static Dictionary<string, string> Enemies = new Dictionary<string, string>
    {
        {"Name",    "baseHealth attackRange  attackRate moveSpeed attackDamage"},
        {"Zombie",  "100 " +    "1,5 2,5 " + "0,5 " +   "2 " +    "10 "},
        {"Skeleton", " "},
        {"Fly", " "},
        {"file", " "}
    };
    public static string GetData(string key)
    {
        return Enemies[key];
    }
}
