using Unity.VisualScripting;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public bool vertical;
    public DoorType type;

    private Vector2 closedPosition;
    private Vector2 openedPosition;
    private const float movinConst = 1;
    private const float piecieOut = 0.05f;

    // True - opened
    // False - closed
    private bool state;
    private bool wanted;

    void Start()
    {
        // name = Door_Spawn_10x10-1x2
        string[] typ = new string[0];
        typ = name.Split('_');

        if (typ.Length > 1)
        {
            switch (type)
            {
                case DoorType.Spawn:
                case DoorType.BossIn:
                    state = true;
                    break;

                case DoorType.Locked:
                case DoorType.BossOut:
                    state = false;
                    break;

                default:
                    Destroy(gameObject);
                    break;
            }

            // otoci o 90 stupnov
            if (vertical)
                transform.rotation = Quaternion.Euler(0, 0, 90);
            // typ[1] = 10x10-R-1x2
            typ = typ[1].Split('-');

            wanted = state;
            closedPosition = transform.position;

            // Opened position
            float[] doorSize = new float[2];
            typ = typ[2].Split('x');
            doorSize[0] = float.Parse(typ[0]) - piecieOut;
            doorSize[1] = float.Parse(typ[1]) - piecieOut;
            if (vertical)
            {
                openedPosition = new Vector2(transform.position.x, transform.position.y + doorSize[1]);
            }
            else
            {
                openedPosition = new Vector2(transform.position.x + doorSize[0], transform.position.y);
            }
            GameManager.instance.AddDoor(this, type);
        }
        else 
            enabled = false;
    }
    void Update()
    {
        if      (state && !wanted) // CLOSING
        {
            Close();
        }
        else if (!state && wanted) // OPENING
        { 
            Open();
        }
    }
    void Open()
    {
        if (!transform.position.Equals(openedPosition))
        {
            MoveTowards(openedPosition);
        }
        else
        {
            state = true;
        }
    }
    void Close()
    {
        if (!transform.position.Equals(closedPosition))
        {
            MoveTowards(closedPosition);
        }
        else
        {
            state = false;
        }
    }
    void MoveTowards(Vector2 pos)
    {
        transform.position = Vector2.MoveTowards(transform.position, pos, movinConst * Time.deltaTime);
    }
    public void ChangeState(bool newState)
    {
        wanted = newState;
    }
    public void SetUpDoor(string size, DoorType newType, bool newVert)
    {
        // NAME: Door_10x10-2x1
        name = "Door_" + size;
        vertical = newVert;
        type = newType;
    }
    private void OnDestroy()
    {
        GameManager.instance.RemoveDoor(this);
    }
}
public enum DoorType
{
    BossIn, BossOut, Locked, Spawn
}