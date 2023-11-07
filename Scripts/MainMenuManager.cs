using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private Transform mainMenu;
    private Transform setMenu;
    private bool main;

    void Start()
    {
        // References
        mainMenu = GameObject.FindGameObjectWithTag("Melee Enemy").GetComponent<Transform>();
        setMenu = GameObject.FindGameObjectWithTag("Ranged Enemy").GetComponent<Transform>();
        Transform vers = GameObject.FindGameObjectWithTag("Finish").transform;
        Transform title = GameObject.FindGameObjectWithTag("Tile").transform;
        Transform comp = GameObject.FindGameObjectWithTag("Floor").transform;

        vers.GetComponent<Text>().text += Application.version;
        title.GetComponent<Text>().text += Application.productName;
        comp.GetComponent<Text>().text += Application.companyName;

        main = true;
        mainMenu.gameObject.SetActive(main);
        setMenu.gameObject.SetActive(!main);
    }
    void Update()
    {

    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void EnterExitSettings()
    {
        main = !main;
        mainMenu.gameObject.SetActive(main);
        setMenu.gameObject.SetActive(!main);
    }
    public void Quit()
    {
        Application.Quit();
    }
    /*
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
    */
}
