using UnityEngine;

public class SpawnOnEnable : MonoBehaviour
{
    public GameObject spawnPrefab;
    private void OnEnable()
    {
        GameObject g = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
        enabled = false;
    }
}
