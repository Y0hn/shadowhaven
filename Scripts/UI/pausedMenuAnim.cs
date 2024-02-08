using UnityEngine;

public class pausedMenuAnim : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform aud;

    bool open, animating;
    float openSize = 500;
    float closeSize = 250;
    public float speed = 1f;
    float sizer;

    private void Start()
    {
        open = false;
        animating = false;
        aud.gameObject.SetActive(false);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, closeSize);
    }
    private void Update()
    {
        if (animating)
        {
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizer);
            sizer += Time.deltaTime * speed;

            if ((open && sizer >= openSize) || (!open && sizer <= closeSize))
            {
                animating = false;
                aud.gameObject.SetActive(open);
            }
        }
    }
    public void OpenSettings()
    {
        if (!animating)
        {
            open = !open;
            if (open)
            {
                sizer = openSize;
                speed = - Mathf.Abs(speed);
            }
            else
            {
                sizer = closeSize;
                speed = - Mathf.Abs(speed);
                aud.gameObject.SetActive(false);
            }
            animating = true;
        }
    }
}
