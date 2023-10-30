using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public string ver;

    private Transform mainMenu;
    private Transform setMenu;
    private bool main;

    void Start()
    {
        mainMenu = GameObject.FindGameObjectWithTag("Melee Enemy").GetComponent<Transform>();
        setMenu = GameObject.FindGameObjectWithTag("Ranged Enemy").GetComponent<Transform>();
        GameObject.FindGameObjectWithTag("Finish").transform.GetComponent<Text>().text += ver;

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
