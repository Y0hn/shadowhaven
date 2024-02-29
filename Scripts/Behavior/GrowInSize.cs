using UnityEngine;

public class Grow : MonoBehaviour
{
    public float rate;
    public float size;

    void Start()
    {
        transform.localScale = Vector3.zero;
    }
    void Update()
    {
        if (transform.localScale.x < size)
        {
            float f = transform.localScale.x + rate * Time.deltaTime;
            transform.localScale = new Vector3(f, f, 1);
        }
        else
        {
            Destroy(this);
            enabled = false;
        }
    }
}
