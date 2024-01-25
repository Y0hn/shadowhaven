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

    private Dictionary<string, int> uiIndexer;
    private float yTowards;
    private readonly string titles =
        "Game Desing: Jan Anco\nCharakter desing: Jan Anco\nCharakter Animations: Jan Anco\nBehavior: Jan Anco\nSound desing: Tim";
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
        UIs[uiIndexer["CompanyName"]].GetComponent<Text>().text = Application.companyName;
        UIs[uiIndexer["Titles"]].GetComponent<Text>().text = titles;


        // Titles put down
        RectTransform
            canvas = UIs[uiIndexer["Canvas"]].GetComponent<RectTransform>();
            parent = UIs[uiIndexer["Titles_parent"]].GetComponent<RectTransform>();
        
        parent.position = new(parent.position.x, -(parent.sizeDelta.y / 2));
        yTowards = Math.Abs(parent.position.y) + canvas.sizeDelta.y;
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
        // ENDED UP
        else
        {
            ReturnToMain();
        }
    }
    void ReturnToMain()
    {
        // save that game was finished
        SceneManager.LoadScene(0);
    }
}
