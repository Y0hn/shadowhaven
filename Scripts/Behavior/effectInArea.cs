using System.Collections.Generic;
using UnityEngine;
public class EffectInArea : MonoBehaviour
{
    public Vector2 pos = Vector2.zero;
    public bool scalable = true;
    public float modifier = 1f;
    public float radius = 1f;
    public LayerMask mask;
    public Effect effect;
    private CharakterStats stats;
    private static readonly Dictionary<Effect, Color> effectColor = new()
    {
        { Effect.MaxHealth, Color.red },
        { Effect.Speed, Color.cyan },
        { Effect.Armor, Color.gray },
    };
    float Size
    {
        get 
        {
            if (!scalable)
                return radius;
            else
                return radius * transform.localScale.x;
        }
        set
        {
            transform.localScale = new(value, value, 1);
        }
    }
    void Update()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(GetPos(), radius, mask);

        if (colls.Length > 0)
            foreach (Collider2D col in colls)
            {
                stats = col.GetComponent<CharakterStats>();
                switch (effect)
                {
                    case Effect.MaxHealth:
                        //col.GetComponent<CharakterStats>().maxHealth.Modify(modifier);
                        Debug.Log("MaxHealth has changed");
                        break;
                    case Effect.Speed:
                        stats.ChangeSpeed(this);
                        break;
                    case Effect.Armor:
                        stats.ChangeSpeed(this);
                        break;
                }
            }
        else if (stats != null)
        {
            stats.ChangeSpeed(this, true);
            stats = null;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = effectColor[effect];
        Gizmos.DrawWireSphere(GetPos(), Size);
    }
    private Vector3 GetPos()
    {
        Vector3 vector3 = transform.position;
        vector3.x += pos.x;
        vector3.y += pos.y;
        return vector3;
    }
    public enum Effect
    {
        MaxHealth, Speed, Armor
    }
    private void OnDestroy()
    {
        if (stats != null)
            stats.ChangeSpeed(this, true);
    }
}
