using UnityEngine;
public class SpawnOnDestroy : MonoBehaviour
{
    public GameObject preFab;
    public float[] pos = new float[3];
    
    /*private void OnDestroy()
    {
        SpawnPreFab();
    }*/
    public bool SpawnPreFab()
    {
        bool ret = preFab != null;

        if (ret)
        {
            if (pos[0] == 0)
                pos[0] = GetPos(0);
            if (pos[1] == 0)
                pos[1] = GetPos(1);

            Instantiate(preFab, new(pos[0], pos[1], pos[2]), Quaternion.identity);
        }

        preFab = null;
        return ret;
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
