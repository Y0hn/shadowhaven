using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class BtnManager : MonoBehaviour
{
    public RectTransform[] menus;
    /* * * * *  ORDER * * * * * *\
     *                          *
     *                          *
     * 0    -   Main            *
     * 1    -   Settings        *
     * 2    -   Audio           *
     * 3    -   Video           *
     *                          *
    \* * * * * * *  * * * * * * */

    private Menu[] menu;

    void Awake()
    {
        // REFERENCES
        menu = new Menu[menus.Length];
        for (int i = 0; i < menus.Length; i++)
            menu[i] = new(menus[i], 50f, 25f);

        // DEPEDNANCIES
        for (int i = 0;i < menu.Length; i++)
            switch (menu[i].name)
            {
                case "Audio":
                    menu[1].SetDepend(i);
                    menu[3].AddAntiDependancy(i);
                    break;
                case "Video":
                    menu[1].SetDepend(i);
                    menu[2].AddAntiDependancy(i);
                    break;
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
                else
                    foreach (int i in m.antiDepend)
                        menu[i].MakeMove(!m.enabled);
            }
    }
    public void EnDisMenu(string name, bool enable)
    {
        foreach (Menu m in menu)
            if (m.name == name && m.enabled != enable)
            {
                m.MakeMove();

                if (!m.enabled)
                    foreach (int i in m.depend)
                        menu[i].MakeMove(m.enabled);
                else
                    foreach (int i in m.antiDepend)
                        menu[i].MakeMove(!m.enabled);
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
        public List<int> antiDepend { get; private set; }
        public bool animated { get; set; }
        private float animationSpeed;
        public bool enabled;

        public Menu(RectTransform menu, float speed, float dur, float deltaY = 0)
        {
            // Variables
            depend = new();
            enabled = false;
            animated = false;
            antiDepend = new();
            this.name = menu.name;
            this.transform = menu;
            originalPos = menu.position;
            animationDur = dur;
            animationSpeed = speed;

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
        public void AddAntiDependancy(int index)
        {
            antiDepend.Add(index);
        }
    }
}