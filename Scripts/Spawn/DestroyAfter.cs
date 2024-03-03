using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    private const float close = 0.05f;
    private SpawnOnDestroy spawn;
    public float destroyAfter;
    private bool spawnBef;
    private void Start()
    {
        destroyAfter += Time.time;
        spawnBef = TryGetComponent(out spawn);
    }
    void Update()
    {
        if      (spawnBef)
        {
            if (destroyAfter - close <= Time.time)
                spawn.SpawnPreFab();
        }
        else if (destroyAfter <= Time.time)
            Destroy(gameObject);
    }
}
