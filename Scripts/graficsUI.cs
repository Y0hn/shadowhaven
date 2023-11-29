using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class graficsUI : MonoBehaviour
{
    Dictionary<RectTransform, Image> UIs = new Dictionary<RectTransform, Image>();
    List<Vector2> positions = new List<Vector2>();

    private static readonly Dictionary<string, string> NameToStartPos = new Dictionary<string, string>()
    {
        { "DeathScreen", "0 -300_70 -100_-70 -100#0_0 255 12 0_0" },
    };
    
    void Start()
    {
        switch (transform.name)
        {
            case "DeathScreen":
                for (int i = 0; i < 3; i++)
                    UIs.Add(transform.GetChild(i).GetComponent<RectTransform>(), transform.GetChild(i).GetComponent<Image>());
                break;

            default:
                break;
        }

        positions = new List<Vector2>(UIs.Count);

        int t = 0;
        foreach (RectTransform r in UIs.Keys)
        {
            positions[t] = r.position;
            t++;
        }
        enabled = false;
    }
    private void FakeStartPos()
    {
        string[] p = NameToStartPos[transform.name].Split('#');
        string[] s = p[0].Split('_');
        string[] c = p[1].Split("_");
        int i = 0;
        foreach (RectTransform r in UIs.Keys)
        {
            string[] q = s[i].Split(' ');
            float x = float.Parse(q[0]);
            float y = float.Parse(q[1]);
            r.localPosition = new Vector2(x, y);
            Debug.Log($"UI: {r.name} position set to [{x},{y}]");
            UIs[r].tintColor = Color.red;
            i++;
        }        
    }
    private void OnEnable()
    {
        FakeStartPos();
        switch (transform.name) 
        {
            case "deathscreen":


            default:
                break;
        }
        //enabled = false;
    }
    private void OnDisable()
    {

    }
}
