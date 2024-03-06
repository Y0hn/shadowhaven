using UnityEngine;
using UnityEngine.UI;

public class AlfaShade : MonoBehaviour
{
    public Text[] texts;
    public Image[] images;
    float transparency;

    public void SetTransparency(float alfa)
    {
        foreach (Image img in images)
            img.color = new(img.color.r, img.color.g, img.color.b, alfa);
        foreach (Text txt in texts)
            txt.color = new(txt.color.r, txt.color.g, txt.color.b, alfa);
        //Debug.Log("Transparency of " + name + " was set to " + alfa);
        transparency = alfa;
    }
    public void AddTransparency(float alfa)
    {
        foreach (Image img in images)
            img.color = new(img.color.r, img.color.g, img.color.b, img.color.a + alfa);
        foreach (Text txt in texts)
            txt.color = new(txt.color.r, txt.color.g, txt.color.b, txt.color.a + alfa);
        //Debug.Log("Transparency of " + name + " was set to " + (alfa + transparency));
        transparency += alfa;
    }
}