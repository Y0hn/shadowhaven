using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
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
            if (menu[i].name == "Audio")
            {
                menu[i - 1].SetDepend(i);
            }
        }
    }
    private void Start()
    {
        if (menu.Length < 1)
            Awake();
    }
    void Update()
    {
        foreach (Menu m in menu)
        {
            if (m.animated)
                m.MoveTowards();
        }
    }
    public void SetActiveBtn(string name, bool enable)
    {
        try
        {
            foreach (Menu m in menu)
                foreach (Button b in m.buttons)
                    if (b.name == name)
                    {
                        b.interactable = enable;
                        Debug.Log("Disabled btn: " + name);
                        break;
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
            if (m.name == name)
            {
                m.MakeMove();

                if (!m.enabled)
                    foreach (int i in m.depend)
                        menu[i].MakeMove(m.enabled);
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
        public List<int> depend { get; private set; }
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
                {
                    tempB.name = tempB.name.Split('B')[0];
                    butList.Add(tempB);
                }
             // else Debug.Log(child.name + " is not a Button! ");
            buttons = butList.ToArray();
            targetedPos = new(menu.position.x - animationDur, menu.position.y + deltaY);
            transform.position = targetedPos;

            shader = menu.GetComponent<AlfaShade>();

            // Hidding menu
            depend = new();
            enabled = false;
            animated = false;
            menu.gameObject.SetActive(false);
            shader.SetTransparency(0);

            // Debug 
            //Debug.Log($"Created Menu: [{name}] with:\norgPos = {originalPos}\ntarPos = {targetedPos}\nIs enabled = {enabled}");
        }
        public void MakeMove()
        {
            transform.gameObject.SetActive(true);
            enabled = !enabled;
            animated = true;

            if (enabled)
                animationSpeed =   Mathf.Abs(animationSpeed);
            else 
                animationSpeed = - Mathf.Abs(animationSpeed);
        }
        public void MakeMove(bool b)
        {
            if (enabled != b)
            {
                transform.gameObject.SetActive(true);
                enabled = !enabled;
                animated = true;

                if (enabled)
                    animationSpeed = Mathf.Abs(animationSpeed);
                else
                    animationSpeed = -Mathf.Abs(animationSpeed);
            }
        }
        public void MoveTowards() 
        {
            float speed = Mathf.Abs(animationSpeed * Time.deltaTime);

            if (!enabled)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetedPos, speed);
                animated = !(transform.position.x == targetedPos.x && transform.position.y == targetedPos.y);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, originalPos, speed);
                animated = !(transform.position.x == originalPos.x && transform.position.y == originalPos.y);
            }

            shader.AddTransparency(1 / (animationDur / (animationSpeed * Time.deltaTime)));

            if (!animated && !enabled)
                transform.gameObject.SetActive(false);
        }
        public void SetDepend(int index)
        {
            depend.Add(index);
        }
    }
}