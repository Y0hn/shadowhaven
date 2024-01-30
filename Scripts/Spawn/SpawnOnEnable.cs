using UnityEngine;
public class SpawnOnEnable : MonoBehaviour
{
    public GameObject spawnPrefab;
    public string newObjectName;
    private void OnEnable()
    {
        GameObject g = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
        g.name = newObjectName;
        enabled = false;
    }
}
