using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Transform titleScreen;
    public Transform mainMenu;
    public Transform setMenu;

    private RectTransform tScreen;
    private Image mScreen;
    private RectTransform sScreen;

    private readonly static float[] titleZoomParam = { 10f, -260f, 3780f };
    private const float scaleChange = 2f;
    private float[] moveBy = new float[2];
    private bool main;
    private bool sub;

    void Start()
    {
        // References
        Transform vers = GameObject.FindGameObjectWithTag("Finish").transform;
        Transform comp = GameObject.FindGameObjectWithTag("Floor").transform;
        Transform name = GameObject.FindGameObjectWithTag("Tile").transform;
        tScreen = titleScreen.GetComponent<RectTransform>();
        mScreen = mainMenu.GetComponent<Image>();
        sScreen = setMenu.GetComponent<RectTransform>();

        // Set Up
        vers.GetComponent<Text>().text += Application.version;
        name.GetComponent<Text>().text = Application.productName;
        comp.GetComponent<Text>().text = Application.companyName;

        // Starting Values for variables
        main = true;
        sub = false;
        moveBy[0] = 0;
        moveBy[1] = 0;
        setMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(true);
        mainMenu.GetChild(0).GetChild(0).GetComponent<Button>().interactable = SaveCheck();

    }
    void Update()
    {
        if (main)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();
            }
            else if (Input.anyKeyDown)
            {
                main = false;
                sub = false;
            }
        }
        else if (!sub)
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
                sub = true;
            }
        }
        else // sub = true
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
            }
        }
    }
    public void ContinueGame()
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

    private bool SaveCheck()
    {
        return false;
    }
}
