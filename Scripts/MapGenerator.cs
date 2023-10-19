using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;
    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    public int walkLength = 10;
    [SerializeField]
    public bool startRandEachIteration = true;
    [SerializeField]
    private TileMapVisual tileMapVisual;

    public void RunProcedualGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandWalk();
        tileMapVisual.PaintFloorTitles(floorPositions);
    }
    protected HashSet<Vector2Int> RunRandWalk()
    {
        var currentPos = startPos;
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();
        
        for (int i = 0; i < iterations; i++) 
        {
            var path = ProceduralGenerationAlg.SimpleRandomWalk(currentPos, walkLength);
            floorPos.UnionWith(path);
            if (startRandEachIteration)
                currentPos = floorPos.ElementAt(Random.Range(0,floorPos.Count));
        }
        return floorPos;
    }
}
