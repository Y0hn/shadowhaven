using UnityEngine;
using UnityEngine.UI;
public class LevelBar : HealthBar
{
    public Text text;

    public void SetText(string s)
    {
        text.text = s;
    }
}