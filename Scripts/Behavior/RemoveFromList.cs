using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RemoveFromList : MonoBehaviour
{
    public string ListName;
    private void OnDestroy()
    {
        switch (ListName)
        {
            case "light":
                GameManager.lights.Remove(GetComponent<Light2D>());
                break;

            default: 
                break;
        }
    }
}
