using UnityEngine;
using UnityEngine.UI;
public class NotificationsManager : MonoBehaviour
{
    public RectTransform ui;
    private Animator animator;
    private Notify[] panels;

    private void Start()
    {
        animator = ui.GetComponent<Animator>();
        panels = new Notify[1];
        panels[0] = new Notify(ui.GetChild(0).GetComponent<RectTransform>(), Notify.Type.Level);
    }
    public void LevelUp(string[] stats)
    {
        string details = "";
        foreach (string s in stats) 
            details += s + "\n";

        panels[0].text.text = details;
        animator.SetTrigger("levelUp");
        Debug.Log("levelUp [Notification]");
    }
    private class Notify
    {
        public RectTransform parent;
        public Text title;
        public Text text;
        public Notify(RectTransform parent, Type type) 
        { 
            this.parent = parent;
            switch (type)
            {
                case Type.Level:
                    title = parent.GetChild(0).GetComponent<Text>();
                    text = parent.GetChild(1).GetComponent<Text>();

                    title.text = "Level Up";
                    break;
            }
        }
        public enum Type
        {
            Level, Default
        }
    }
}
