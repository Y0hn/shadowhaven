using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class TitlesManager : MonoBehaviour
{
    public List<Transform> UIs;
    public float move = 100;
    RectTransform parent;
    private const string autor = "Jan Anco";
    private Dictionary<string, int> uiIndexer;
    private float yTowards;
    private float timer = 0;
    private static readonly string ytLinks =
        "\nhttps://www.youtube.com/watch?v=OeU8UYJgxZg" +
        "\nhttps://www.youtube.com/watch?v=DOz9vQYKHBg" +
        "\nhttps://www.youtube.com/watch?v=gfBIRQRdzu0" +
        "\nhttps://www.youtube.com/watch?v=JFrGDCc8Je0" +
        "\nhttps://www.youtube.com/watch?v=qkDByoPKvmU";
    private static readonly string titles =
        $"Game Desing: {autor}\n\nCharakter design: {autor}\n\nCharakter Animations: {autor}\n\nAI Behavior: {autor}\n\nMenu Sound design: Tim Anco\n\n" +
        $"Game Sound Effects\nZAPSPLAT\n\n" +
        $"InGame Sound Track Sources\n--< YouTube >--{ytLinks}";
    private static readonly string thx =
        "Thank you for playing";
    private bool tit;
    private Text afterText, comText;
    private float cmpT = 2, aftT = 2;
    void Start()
    {
        // List add UIs
        for (int i = 0; i < UIs[0].childCount; i++)
            UIs.Add(UIs[0].GetChild(i));

        // References
        uiIndexer = new();
        for (int i = 0; i < UIs.Count; i++) 
            uiIndexer.Add(UIs[i].name.Trim(), i);

        // Set Up
        UIs[uiIndexer["Version"]].GetComponent<Text>().text = Application.version;
        UIs[uiIndexer["Title"]].GetComponent<Text>().text = Application.productName;
        UIs[uiIndexer["Titles"]].GetComponent<Text>().text = titles;
        afterText = UIs[uiIndexer["AfterText"]].GetComponent<Text>();
        comText = UIs[uiIndexer["CompanyName"]].GetComponent<Text>();
        comText.text = Application.companyName;

        // Titles put down
        RectTransform
            canvas = UIs[uiIndexer["Canvas"]].GetComponent<RectTransform>();
            parent = UIs[uiIndexer["Titles_parent"]].GetComponent<RectTransform>();
        
        parent.position = new(parent.position.x, -(parent.sizeDelta.y / 2));
        yTowards = Math.Abs(parent.position.y) + canvas.sizeDelta.y - 700;
        afterText.color = new Color(255, 255, 255, 0);
        tit = false;
        timer = 0;
    }
    void Update()
    {
        // MOVING TITLES UP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToMain();
        }
        else if (parent.position.y < yTowards)
        {
            parent.position = new Vector2(parent.position.x, parent.position.y + move * Time.deltaTime);
        }
        else if (timer == 0)
            timer = Time.time + cmpT;
        // ENDED UP
        else if (!tit && timer < Time.time)
        {
            UIs[uiIndexer["AfterText"]].position = UIs[uiIndexer["CompanyName"]].position;
            timer = Time.time + aftT;
            afterText.text = thx;
            tit = true;
        }
        else if (timer < Time.time) // tit = true
        {
            ReturnToMain();
        }
        else if (tit)
        {
            float a = Mathf.Abs(afterText.color.a + Time.deltaTime / cmpT);
            afterText.color = new Color(255, 255, 255, a);
            //comText.color = new Color(255, 255, 255, 1 - a);
            comText.gameObject.SetActive(false);
            if (a >= 1)
            {
                timer = Time.time + aftT;
            }
        }
    }
    void ReturnToMain()
    {
        // save that game was finished
        SceneManager.LoadScene(0);
    }
}
