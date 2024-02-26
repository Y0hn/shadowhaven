using System.Collections.Generic;
using UnityEngine;

public class EviromentManager : MonoBehaviour
{
    private List<DoorBehavior> doors = new();

    public void AddDoor(DoorBehavior newDoor)
    {
        doors.Add(newDoor);
    }
    public void RemoveDoor(DoorBehavior oldDoor)
    {
        doors.Remove(oldDoor);
    }
    public void OpenDoors(DoorType type, bool open = true)
    {
        foreach (DoorBehavior d in doors)
            if (d.type.Equals(type))
                d.ChangeState(open);
    }
    public DoorBehavior[] GetDoors(DoorType type)
    {
        List<DoorBehavior> dos = new();
        foreach (DoorBehavior d in doors)
            if (d.type == type)
                dos.Add(d);
        return dos.ToArray();
    }
}
