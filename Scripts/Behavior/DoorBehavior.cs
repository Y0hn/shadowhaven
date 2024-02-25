using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DoorBehavior : MonoBehaviour
{
    public bool vertical = false;
    public ShadowCaster2D shadow;
    public DoorType type;

    private Vector3 closedPosition;
    private Vector3 openedPosition;
    private const float movinConst = 1;
    private float waitTimer = 0;
    private const float waitConst = 0.01f;
    private const float roomCenter = 5;

    // True - opened
    // False - closed
    private bool state;
    private bool wanted;

    private bool setup;
    private bool renamed;
    private bool interCheck = false;
    private bool toRight = false;

    void Start()
    {
        #region SmartSetup
        // name = "Door:Spawn:10x10-U_-2x1"
        string[] typ;
        typ = name.Split(':');

        if (typ.Length > 2)
        {
            // typ[0] = "Door"

            // typ[1] = "Spawn"
            switch (typ[1])
            {
                case "Boss": 
                    type = DoorType.BossIn;
                    interCheck = false; break;
                case "Loot": 
                    type = DoorType.BossOut; break;
                case "Spawn":
                    type = DoorType.Spawn; 
                    interCheck = true; break;
                case "Path": 
                    type = DoorType.Locked; break;
                default:
                    Debug.Log($"{typ[1]} is not an option for door");
                    break;
            }

            // typ[2] = "10x10-U_-2x1"
            typ = typ[2].Split('-');

            // typ[0] = "10x10"
            float roomX = float.Parse(typ[0].Split('x')[0])/2;
            float roomY = float.Parse(typ[0].Split("x")[1]) - roomCenter;

            // Position setup
            // typ[1] = "U_"
            switch (typ[1])
            {
                case "U_": transform.position = new Vector3(transform.position.x, transform.position.y + roomY, transform.position.z);
                    break;
                default:
                case "D_": transform.position = new Vector3(transform.position.x, transform.position.y - roomCenter, transform.position.z);
                    break;
                case "_R": transform.position = new Vector3(transform.position.x + roomX, transform.position.y, transform.position.z); 
                    vertical = true; toRight = true; break;
                case "_L": transform.position = new Vector3(transform.position.x - roomX, transform.position.y, transform.position.z); 
                    vertical = true; break;
            }

            // Verticality
            if (vertical)
                transform.rotation = Quaternion.Euler(0,0,90);

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

            closedPosition = transform.position;
            wanted = state;

            // Opened position
            // typ[2] = "2x1"
            float doorSizeX = float.Parse(typ[2].Split('x')[0]);
            
            if (vertical)
            {
                openedPosition = new Vector3(transform.position.x, transform.position.y + doorSizeX, transform.position.z);
            }
            else
            {
                openedPosition = new Vector3(transform.position.x + doorSizeX, transform.position.y, transform.position.z);
            }
            if (state)
                transform.position = openedPosition;
            #endregion

            GameManager.instance.AddDoor(this);
            setup = true;
        }
        else
            setup = false;
    }
    void Update()
    {
        if (setup)
        {
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
        }
        else if (Time.time > waitTimer)
        {
            // parent.name = "Spawn=10x10-U_"
            string[] s = transform.parent.name.Split('=');
            if (s.Length > 1)
            {
                if (!renamed)
                {
                    // name = "Door-2x1(clone)"
                    // name = name.Split('(')[1];
                    // name = "Door-2x1"
                    string[] q = name.Split('-');
                    name = q[0] + ":" + s[0] + ":" + s[1] + "-" + q[1];
                    // name = "Door:Spawn:10x10-U_-2x1"
                    renamed = true;
                }
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
    public void SetUpDoor(string size, DoorType newType, bool newVert)
    {
        // NAME: Door_10x10-2x1
        name = "Door_" + size;
        vertical = newVert;
        type = newType;
    }
    public Vector2 GetClosedPos()
    {
        return closedPosition;
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