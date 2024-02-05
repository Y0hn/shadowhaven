using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BtnManager : MonoBehaviour
{
    public RectTransform[] menus;

    private Dictionary<string, RectTransform> buttonsRect;
    private Dictionary<string, Button> buttons;
    private Dictionary<string, int> menusNames;
    private Dictionary<int, Vector2> menusPos;
    private Dictionary<string, int> menuBtns;

    // Animation
    private const float animationDur = 50f;
    public float animationSpeed = 50f;
    private RectTransform animated;
    private Vector2 targetPos;
    private AlfaShade shader;
    private bool deactivate;
    private bool animate;
    private bool moveX;

    void Start()
    {
        // References
        buttons = new();
        menuBtns = new();
        menusPos = new();
        menusNames = new();
        buttonsRect = new();
        for (int i = 0; i < menus.Length; i++)
        {
            RectTransform child = menus[i];
            List<Transform> grandChildren = new();
            for (int j = 0; j < child.childCount; j++)
                grandChildren.Add(child.GetChild(j));
            foreach (Transform grandChild in grandChildren)
            {
                try
                {
                    if (grandChild.name.Contains("B"))
                        grandChild.name = grandChild.name.Split('B')[0];
                    buttonsRect.Add(grandChild.name, grandChild.GetComponent<RectTransform>());
                    buttons.Add(grandChild.name, grandChild.GetComponent<Button>());
                    menuBtns.Add(grandChild.name, i);
                } catch { Debug.LogWarning("GrandChild: " + grandChild.name + " is not a button or is not compatible"); }
            }
            menusNames.Add(child.name, i);
            menusPos.Add(i, child.position);

            // Btn Setup
            child.position = new(- child.position.x - child.sizeDelta.x, child.position.y);
            child.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (animate)
        {
            if (
                ((animated.position.x < targetPos.x || animated.position.y < targetPos.y) && 0 < animationSpeed)
                    ||
                ((animated.position.x > targetPos.x || animated.position.y > targetPos.y) && 0 > animationSpeed)
                )
            {
                if (moveX)
                    animated.position = new(animated.position.x + animationSpeed * Time.deltaTime, animated.position.y);
                else
                    animated.position = new(animated.position.x, animated.position.y + animationSpeed * Time.deltaTime);

                // Shading
                animated.GetComponent<AlfaShade>().AddTransparency(1/(animationDur/(animationSpeed * Time.deltaTime)));
            }
            else
            {
                animated.gameObject.SetActive(!deactivate);
                animate = false;
            }
        }
    }
    public void DisableBtn(string name)
    {
        try
        {
            buttons[name].interactable = false;
            Debug.Log("Disabled btn: " + name);
        } 
        catch 
        {
            string btns = "";

            string[] temp = { "" };
            if (buttonsRect != null)
                temp = buttonsRect.Keys.ToArray();
            if (!temp[0].Equals(string.Empty))

                foreach (string key in temp)
                {
                    btns += "\n" + key;
                }
            else
                btns = "[ is empty ]";

            Debug.LogWarning("Button " + name + " is not in Dictionaries (yet)\nList of Buttons:" + btns);
        }
    }
    public void EnDisMenu(string name)
    {
        int index = menusNames[name];
        animated = menus[index];
        shader = animated.GetComponent<AlfaShade>();
        moveX = true;

        if (animated.gameObject.activeSelf)
        {
            animationSpeed = -Mathf.Abs(animationSpeed);
            if (moveX)
                targetPos = new(animated.position.x - animationDur, animated.position.y);
            else
                targetPos = new(animated.position.x, animated.position.y - animationDur);

            Debug.Log("Disambling " + name);
            shader.SetTransparency(1f);
            deactivate = true;
        }
        else
        {
            animationSpeed = Mathf.Abs(animationSpeed);
            targetPos = menusPos[index];

            if (moveX)
                animated.position = new(targetPos.x - animationDur, animated.position.y);
            else
                animated.position = new(targetPos.x, animated.position.y - animationDur);
            animated.gameObject.SetActive(true);

            Debug.Log("Enabling " + name);
            shader.SetTransparency(0f);
            deactivate = false;
        }

        animate = true;
    }
    public void EnDisMenu(string name, bool enable)
    {
        int index = menusNames[name];
        animated = menus[index];
        shader = animated.GetComponent<AlfaShade>();
        moveX = true;

        if      (!enable && animated.gameObject.activeSelf)
        {
            animationSpeed = -Mathf.Abs(animationSpeed);
            if (moveX)
                targetPos = new(animated.position.x - animationDur, animated.position.y);
            else
                targetPos = new(animated.position.x, animated.position.y - animationDur);

            Debug.Log("Disambling " + name);
            shader.SetTransparency(1f);
            deactivate = true;
        }
        else if (enable && !animated.gameObject.activeSelf)
        {
            animationSpeed = Mathf.Abs(animationSpeed);
            targetPos = menusPos[index];

            if (moveX)
                animated.position = new(targetPos.x - animationDur, animated.position.y);
            else
                animated.position = new(targetPos.x, animated.position.y - animationDur);
            animated.gameObject.SetActive(true);

            Debug.Log("Enabling " + name);
            shader.SetTransparency(0f);
            deactivate = false;
        }
        else
            return;

        animate = true;
    }
}