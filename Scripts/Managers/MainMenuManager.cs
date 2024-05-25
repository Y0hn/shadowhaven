using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Transform titleScreen;
    public Transform mainMenu;
    private RectTransform tScreen;
    private BtnManager btnManager;
    private Image mScreen;

    private readonly static float[] titleZoomParam = { 10f, -260f, 3780f };
    private const float scaleChange = 2f;
    private float[] moveBy = new float[2];
    private bool inMenu;
    private bool main;
    private bool sub;

    void Start()
    {
        // References
        Transform vers = GameObject.FindGameObjectWithTag("Finish").transform;
        Transform comp = GameObject.FindGameObjectWithTag("Floor").transform;
        Transform name = GameObject.FindGameObjectWithTag("Tile").transform;
        tScreen = titleScreen.GetComponent<RectTransform>();
        btnManager = mainMenu.GetComponent<BtnManager>();
        mScreen = mainMenu.GetComponent<Image>();

        // Set Up
        vers.GetComponent<Text>().text += Application.version;
        name.GetComponent<Text>().text = Application.productName;
        comp.GetComponent<Text>().text = Application.companyName;

        // Starting Values for variables
        sub = true;
        main = true;
        moveBy[0] = 0;
        moveBy[1] = 0;
        SaveSystem.fileDataLoaded = false;
        mainMenu.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(true);
    }
    void Update()
    {
        if (inMenu)
        {
            // Tranzitions
            if (main)
            {

            }
            else // if (sub)
            {

            }
        }
        else // Try to focus Menu
        {
            if (main)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    Quit();
                else if (Input.anyKeyDown)
                    main = false;
            }
            else if (sub)
            {
                if (moveBy[0] == 0)
                {
                    int counter = (int)Math.Ceiling((titleZoomParam[0] - 1) / scaleChange);
                    moveBy[0] = titleZoomParam[1] / counter;
                    moveBy[1] = titleZoomParam[2] / counter;
                    //Debug.Log($"Counter setted to {counter}");
                }
                else if (tScreen.localScale.x <= titleZoomParam[0])//if (Time.time <= timer) //if (tScreen.localScale.x < titleZoomParam[0])
                {
                    float x = tScreen.position.x + moveBy[0] * Time.deltaTime;
                    float y = tScreen.position.y + moveBy[1] * Time.deltaTime;
                    tScreen.position = new Vector3(x, y, 0);

                    x = tScreen.localScale.x + scaleChange * Time.deltaTime;
                    y = tScreen.localScale.y + scaleChange * Time.deltaTime;
                    tScreen.localScale = new Vector3(x, y, 1);
                }
                else
                {
                    //Debug.Log("Zoomed");
                    mScreen.color = new Color(mScreen.color.r, mScreen.color.g, mScreen.color.b, 0);
                    mainMenu.gameObject.SetActive(true);
                    sub = false;
                }
            }
            else
            {
                if (mScreen.color.a < 1)
                {
                    float newA = mScreen.color.a + Time.deltaTime;
                    if (newA > 1)
                        newA = 1;

                    //Debug.Log($"Color a set to {newA}");
                    mScreen.color = new Color(mScreen.color.r, mScreen.color.g, mScreen.color.b, newA);
                }
                else
                {
                    titleScreen.gameObject.SetActive(false);
                    tScreen.localScale = new Vector3(1, 1, 1);
                    tScreen.position = Vector3.zero;
                    inMenu = true;
                    main = true;
                    if (!SaveSystem.SaveDataExist())
                        btnManager.SetActiveBtn("Continue", false);
                    btnManager.EnDisMenu("Main");
                }
            }
        }
    }
    public void ContinueGame()
    {
        foreach (Transform t in transform.GetComponentInChildren<Transform>())
            t.gameObject.SetActive(false);
        SaveSystem.fileDataLoaded = true;
        SceneManager.LoadScene(1);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void EnterExitSettings()
    {
        btnManager.EnDisMenu("Settings");

    }
    public void EnterExitAudio()
    {
        btnManager.EnDisMenu("Audio");
    }
    public void EnterExitVideo()
    {
        btnManager.EnDisMenu("Video");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
