using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private int width, height;
    private bool Full = false;

    void Start()
    {

    }
    void Update()
    {

    }
    public void SetWidth(int newWidth)
    {
        width = newWidth;
    }
    public void SetHeight(int newHeight)
    {
        height = newHeight;
    }
    public void SetFull(bool newFull)
    {
        Full = newFull;
    }
    public void SetRes()
    {
        Screen.SetResolution(width, height, Full);
    }
}
