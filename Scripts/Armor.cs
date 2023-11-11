using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Armor")]
public class Armor : Equipment
{
    public int armorModifier = 0;
    public Sprite[] texture;
}
