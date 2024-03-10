using UnityEngine.Rendering.Universal;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public bool vertical = false;
    public ShadowCaster2D shadow;
    public DoorType type;

    private Vector3 closedPosition;
    private Vector3 openedPosition;
    private float waitTimer = 0;
    private const float waitConst = 0.01f;
    private const float movinConst = 1;
    private const float roomCenter = 5;

    // True     = opened
    // False    = closed
    private bool state;
    private bool wanted;
    private bool setup;
    private bool renamed;

    void Start()
    {
        // name = "Door:Spawn:10x10-U_-2x1"
        string[] typ;
        typ = name.Split(':');

        if (typ.Length > 2)
        {
            SetUp(typ, transform);
        }
        else
        {
            typ = transform.parent.name.Split(':');
            if (typ.Length > 2)
            {
                SetUp(typ, transform.parent);
            }
            else
                setup = false;
        }
    }
    void SetUp(string[] parameters, Transform transform2)
    {
        // typ[0] = "Door"

        // typ[1] = "Spawn"
        switch (parameters[1])
        {
            case "Boss":
                type = DoorType.BossIn;
                break;
            case "Loot":
                type = DoorType.BossOut; break;
            case "Spawn":
                type = DoorType.Spawn;
                break;
            case "Path":
                type = DoorType.Locked; break;
            default:
                Debug.Log($"{parameters[1]} is not an option for door");
                break;
        }

        // typ[2] = "10x10-U_-2x1"
        parameters = parameters[2].Split('-');

        // typ[0] = "10x10"
        float roomX = float.Parse(parameters[0].Split('x')[0]) / 2;
        float roomY = float.Parse(parameters[0].Split("x")[1]) - roomCenter;

        // Position setup
        // typ[1] = "U_"
        switch (parameters[1])
        {
            case "U_":
                transform2.position = new Vector3(transform2.position.x, transform2.position.y + roomY, transform2.position.z);
                break;
            default:
            case "D_":
                transform2.position = new Vector3(transform2.position.x, transform2.position.y - roomCenter, transform2.position.z);
                break;
            case "_R":
                transform2.position = new Vector3(transform2.position.x + roomX, transform2.position.y, transform2.position.z);
                vertical = true; break;
            case "_L":
                transform2.position = new Vector3(transform2.position.x - roomX, transform2.position.y, transform2.position.z);
                vertical = true; break;
        }

        // Verticality
        if (vertical)
            transform2.rotation = Quaternion.Euler(0, 0, 90);

        // Setup acording to Type (Role)
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

        closedPosition = transform2.position;
        wanted = state;

        // Opened position
        // typ[2] = "2x1"
        float doorSizeX = float.Parse(parameters[2].Split('x')[0]);

        if (vertical)
        {
            openedPosition = new Vector3(transform2.position.x, transform2.position.y + doorSizeX, transform2.position.z);
        }
        else
        {
            openedPosition = new Vector3(transform2.position.x + doorSizeX, transform2.position.y, transform2.position.z);
        }

        // Setting Positions
        transform2.position = closedPosition;
        
        if (transform2.childCount > 1)
            transform2.GetChild(1).position = openedPosition;

        if (state)
            transform2.GetChild(0).position = openedPosition;
        else
            transform2.GetChild(0).position = closedPosition;

        GameManager.enviroment.AddDoor(this);
        setup = true;
    }
    void Update()
    {
        if (setup)
        {/*
            if (interCheck)
            {
                Transform player = GameObject.FindGameObjectWithTag("Player").transform;
                if (type.Equals(DoorType.Spawn)) // Spawn room
                {
                    if (player.position.y > transform.position.y)
                    {
                        if (toRight)
                        {
                            if (transform.position.x < player.position.x)
                            {
                                wanted = false;
                            }
                        }
                        else
                        {
                            if (transform.position.x > player.position.x)
                            {
                                wanted = false;
                            }
                        }
                    }
                    else if (!wanted)
                    {
                        wanted = true;
                        Open();
                    }

                }
                else
                {
                    if (vertical)
                    {
                        if (toRight)
                        {
                            if (transform.position.x < player.position.x)
                            {
                                interCheck = false;
                                wanted = false;
                            }
                        }
                        else
                        {
                            if (transform.position.x > player.position.x)
                            {
                                interCheck = false;
                                wanted = false;
                            }
                        }
                    }
                    else
                    {
                        if (player.position.y > transform.position.y)
                        {
                            interCheck = false;
                            wanted = false;
                        }
                    }
                }
            }
        */}
        else if (Time.time > waitTimer && !renamed)
        {
            // room.name = "Spawn=10x10-U_"
            Transform t = transform.parent;
            string[] s;

            s = t.parent.name.Split('=');
            // name = "Door-2x1(clone)"
            // name = name.Split('(')[0];
            // name = "Door-2x1"
            string[] q = t.name.Split('-');

            if (2 <= s.Length && q.Length >= 2)
            {
                t.name = q[0] + ":" + s[0] + ":" + s[1] + "-" + q[1];
                // t.name = "Door:Spawn:10x10-U_-2x1"
                renamed = true;
                Start();
            }
            waitTimer = Time.time + waitConst;
        }

        if (state != wanted)
            if (state && !wanted) // CLOSING
                Close();
            else if (!state && wanted) // OPENING
                Open();
    }
    void Open()
    {
        if (!transform.position.Equals(openedPosition))
        {
            MoveTowards(openedPosition);
        }
        else
        {
            state = wanted;
            Shadow(state);
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
            state = wanted;
            Shadow(state);
        }
    }
    void Shadow(bool en)
    {
        // en = opened
        if (shadow != null)
        {
            shadow.selfShadows = !en;
            //shadow.castsShadows = en;
        }
    }
    void MoveTowards(Vector3 pos)
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, movinConst * Time.deltaTime);
    }
    public void ChangeState(bool newState)
    {
        wanted = newState;
    }
    /*public void SetUpDoor(string size, DoorType newType, bool newVert)
    {
        // NAME: Door_10x10-2x1
        name = "Door_" + size;
        vertical = newVert;
        type = newType;
    }*/
    public Vector2 GetClosedPos()
    {
        return closedPosition;
    }
    private void OnDestroy()
    {
        GameManager.enviroment.RemoveDoor(this);
    }
}
public enum DoorType
{
    BossIn, BossOut, Locked, Spawn
}