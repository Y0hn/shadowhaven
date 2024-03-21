using UnityEngine;
public class LevelEnd : MonoBehaviour
{
    public Vector2 pos = Vector2.zero;
    public float radius = 1f;
    private Transform player;
    private float timer = 0;
    private void Start()
    {
        player = GameManager.instance.player.transform;
    }
    void Update()
    {
        if (timer < Time.time)
        {
            float d = Vector2.Distance(GetPos(), player.position);

            if (d < radius * 5) // Proximity
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(GetPos(), radius);
                foreach (Collider2D col in colliders)
                {
                    //Debug.Log($"{col.name} was overlaped by destroy");
                    if (col.name == "Player")
                    {
                        GameManager.instance.NextLevel();
                        break;
                    }
                }
            }
            else
            {
                timer = Time.time + 1f; //d/10;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetPos(), radius);
    }
    private Vector3 GetPos()
    {
        Vector3 vector3 = transform.position;
        vector3.x += pos.x;
        vector3.y += pos.y;
        return vector3;
    }
}
