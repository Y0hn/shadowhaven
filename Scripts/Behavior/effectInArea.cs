using UnityEngine;
public class EffectInArea : MonoBehaviour
{
    public LayerMask mask;
    public bool scalable = true;
    public float size = 1f;
    float Size
    {
        get 
        {
            if (!scalable)
                return size;
            else
                return size * transform.localScale.x;
        }
        set
        {
            transform.localScale = new(value, value, 1);
        }
    }
    void Update()
    {
        if (scalable) 
        { 
            
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
    }
    public enum Effect
    {
        MaxHealth, Speed, Armor
    }
}
