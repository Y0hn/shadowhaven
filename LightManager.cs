using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    Dictionary<GameObject, LightType> lights;

    private void Start()
    {
        lights = new();
    }

    void Update()
    {        

    }
}
public enum LightType
{
    Normal, Boss
}
