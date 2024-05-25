using UnityEngine;
public class DeleteOnContact : MonoBehaviour
{
    public Vector2 pos = Vector2.zero;
    public float destroyRadius = 1f;
    public string destroyedName = "";
    public bool contains = true;

    void Start()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GetPos(), destroyRadius);
        foreach (Collider2D col in colliders)
        {
            //Debug.Log($"{col.name} was overlaped by destroy");
            if      (col.name == destroyedName)
                Destroy(col.gameObject);
            else if (contains && col.name.Contains(destroyedName))
                Destroy(col.gameObject);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetPos(), destroyRadius);
    }
    private Vector3 GetPos()
    {
        Vector3 vector3 = transform.position;
        vector3.x += pos.x;
        vector3.y += pos.y;
        return vector3;
    }
}
