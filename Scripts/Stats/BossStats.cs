using UnityEngine;

public class BossStats : EnemyStats
{
    public float activateBorderY = float.MaxValue;

    private Transform target;
    //private int bossType = 0;

    protected override void Start()
    {
        base.Start();
        stunable = false;
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
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if (target.position.y >= activateBorderY)
        {
            animator.enabled = true;
            this.enabled = false;
        }

        /*switch (bossType)
        {
            case 1: // ZOM

                break;
            case 2: // BLOB

                break;
            case 3: // CENTIPIDE

                break;

            default:
                Destroy(gameObject); 
                break;
        }
        */
    }
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
    }
    protected override void Die()
    {
        // Just die
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
