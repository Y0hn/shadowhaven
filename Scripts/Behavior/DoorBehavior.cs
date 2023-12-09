using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public DoorType type;
    public bool vertical;

    private Vector2 closedPosition;
    private Vector2 openedPosition;
    private const float movinConst = 1;

    // True - opened
    // False - closed
    private bool state;
    private bool wanted;

    void Start()
    {
        closedPosition = transform.position;
        GameManager.instance.AddDoor(this);


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
        wanted = state;

        if (vertical)
            transform.rotation = Quaternion.Euler(0,0,90);

        if (float.TryParse(name.Split('-')[1].Split('(')[0].Trim(), out float newPos))
        {
            if (vertical)
            {
                closedPosition = new Vector2 (transform.position.x, newPos);
            }
            else
            {
                closedPosition = new Vector2 (newPos, transform.position.y);
            }
        }
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
}
public enum DoorType
{
    BossIn, BossOut, Locked, Spawn
}