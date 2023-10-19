using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlg
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPos, int walkLencht)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPos);
        var prevPos = startPos;

        for (int i = 0; i < walkLencht; i++)
        {
            var newPos = prevPos + Direction2D.GetRandCardDir();
            path.Add(newPos);
            prevPos = newPos;
        }
        return path;
    }
}
public static class Direction2D
{
    public static List<Vector2Int> cardinalDirList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // UP
        new Vector2Int(1,0), // RIGHT
        new Vector2Int(0,-1),// DOWN
        new Vector2Int(-1,0),// LEFT
    };

    public static Vector2Int GetRandCardDir()
    {
        return cardinalDirList[Random.Range(0, cardinalDirList.Count)];
    }
}
