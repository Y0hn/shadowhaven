using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float destroyAfter;
    private void Start()
    {
        destroyAfter += Time.time;
    }
    void Update()
    {
        if (destroyAfter <= Time.time)
            Destroy(gameObject);
    }
}
