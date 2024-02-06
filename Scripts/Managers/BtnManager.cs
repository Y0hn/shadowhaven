using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BtnManager : MonoBehaviour
{
    public RectTransform[] menus;

    private Menu[] menu;

    void Awake()
    {
        // References
        menu = new Menu[menus.Length];
        for (int i = 0; i < menus.Length; i++)
        {
            menu[i] = new(menus[i], 50f);
        }
    }

    void Update()
    {
        foreach (Menu m in menu)
        {
            if (m.animated)
            {
                if (m.IsOnMove())
                    m.MoveTowards();
                else
                {
                    m.enabled = !m.enabled;
                    m.animated = false;
                }
                Debug.Log("Animating " + name);
            }
        }
    }
    public void SetActiveBtn(string name, bool enable)
    {
        try
        {
            foreach (Menu m in menu)
            {
                foreach (Button b in m.buttons)
                {
                    if (b.name == name)
                    {
                        b.enabled = enable;
                        Debug.Log("Disabled btn: " + name);
                        break;
                    }
                }
            }
        } 
        catch 
        {
            Debug.LogWarning(name + " button does make me hurt! ");
        }
    }
    public void EnDisMenu(string name)
    {
        foreach (Menu m in menu)
        {
            if (m.name == name)
            {
                m.MakeMove();
            }
        }
    }
    public void EnDisMenu(string name, bool enable)
    {
        foreach (Menu m in menu)
        {
            if (m.name == name)
            {
                if (m.enabled != enable)
                    m.MakeMove();
            }
        }
    }
    private class Menu
    {
        public  readonly string name;
        private readonly RectTransform transform;
        public  readonly Button[] buttons;
        private readonly AlfaShade shader;
        private readonly Vector2 originalPos;
        private readonly Vector2 targetedPos;
        private readonly float animationDur;
        public bool animated { get; set; }
        private float animationSpeed;
        public bool enabled;

        public Menu(RectTransform menu, float speedA, float deltaY = 0)
        {
            this.name = menu.name;
            this.transform = menu;
            originalPos = menu.position;
            animationDur = speedA;
            animationSpeed = animationDur;

            // Btn Setup
            List<Button> butList = new();
            List<Transform> children = new();
            for (int j = 0; j < menu.childCount; j++)
                children.Add(menu.GetChild(j));
            foreach (Transform child in children)
                if (child.TryGetComponent(out Button tempB))
                    butList.Add(tempB);
             // else Debug.Log(child.name + " is not a Button! ");
            buttons = butList.ToArray();
            targetedPos = new(- menu.position.x - menu.sizeDelta.x, menu.position.y + deltaY);
            transform.position = targetedPos;

            shader = menu.GetComponent<AlfaShade>();

            // Hidding menu
            enabled = false;
            animated = false;
            menu.gameObject.SetActive(false);

            // Debug 
            Debug.Log($"Created Menu: [{name}] with:\norgPos = {originalPos}\ntarPos = {targetedPos}\nIs enabled = {enabled}");
        }
        public bool IsOnMove()
        {
            bool onMoveToRight, onMoveToLeft;

            if (enabled)
            {
                onMoveToRight = (transform.position.x > targetedPos.x || transform.position.y > targetedPos.y) && 0 < animationSpeed;
                onMoveToLeft  = (transform.position.x < targetedPos.x || transform.position.y < targetedPos.y) && 0 > animationSpeed;
            }
            else
            {
                onMoveToRight = (transform.position.x < targetedPos.x || transform.position.y < targetedPos.y) && 0 < animationSpeed;
                onMoveToLeft  = (transform.position.x > targetedPos.x || transform.position.y > targetedPos.y) && 0 > animationSpeed;
            }

            if (!(onMoveToRight || onMoveToLeft))
            {
                animated = false;
                enabled = !enabled;
            }

            return true; // onMoveToRight || onMoveToLeft;
        }
        public void MakeMove()
        {
            animated = true;
            if (enabled)
                animationSpeed = - Mathf.Abs(animationSpeed);
            else 
                animationSpeed =   Mathf.Abs(animationSpeed);
        }
        public void MoveTowards() 
        {
            if (enabled)
                transform.position = Vector2.MoveTowards(transform.position, originalPos, animationSpeed * Time.deltaTime);
            else
                transform.position = Vector2.MoveTowards(transform.position, targetedPos, animationSpeed * Time.deltaTime);

            shader.AddTransparency(1 / (animationDur / (animationSpeed * Time.deltaTime)));
        }
    }
}