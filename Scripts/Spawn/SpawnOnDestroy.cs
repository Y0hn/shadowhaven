using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    public GameObject preFab;
    public float[] pos = new float[3];
    private void OnDestroy()
    {
        for (int i = 0; i < pos.Length; i++)
            if (pos[i] == 0)
                pos[i] = GetPos(i);
        Instantiate(preFab, new(pos[0], pos[1], pos[2]), Quaternion.identity);
    }
    private float GetPos(int index)
    {
        switch (index)
        {
            case 0: return transform.position.x;
            case 1: return transform.position.y;
            case 2: return transform.position.z;
        }
        return 0;
    }
}
