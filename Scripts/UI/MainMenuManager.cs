using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Transform titleScreen;
    public Transform mainMenu;
    public Transform setMenu;
    private bool title;
    private bool main;

    void Start()
    {
        // References
        Transform vers = GameObject.FindGameObjectWithTag("Finish").transform;
        Transform name = GameObject.FindGameObjectWithTag("Tile").transform;
        Transform comp = GameObject.FindGameObjectWithTag("Floor").transform;
        // Set Up
        vers.GetComponent<Text>().text += Application.version;
        name.GetComponent<Text>().text = Application.productName;
        comp.GetComponent<Text>().text = Application.companyName;
        // Starting Values for variables
        title = true;
        main = false;
    }
    void Update()
    {
        if (title)
        {
            if (Input.anyKeyDown)
            {
                StartGame();
                //\\ remove this
            }
            // Add som animation
        }
        // Add some more opitions
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
}
