using UnityEngine;

public class BossStats : CharakterStats
{
    protected override void Start()
    {
        base.Start();

        // Zistenie typu miestnosti
        switch (RoomType())
        {
            case "20x20":
                transform.position = new Vector2(transform.position.x, transform.position.y + 10);
                break;

            default:
                // 10x10
                break;
        }


    }
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
    }
    protected override void Die()
    {

    }

    private string RoomType()
    {
        string[] s = transform.parent.name.Split(' ');

        if (s.Length > 1)
            return s[1];
        else
            return null;
    }
}
