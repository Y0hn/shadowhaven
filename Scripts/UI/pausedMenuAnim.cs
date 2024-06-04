using UnityEngine.UI;
using UnityEngine;

public class pausedMenuAnim : MonoBehaviour
{
    public RectTransform mainPanel;
    public RectTransform settPanel;
    public RectTransform savePanel;
    public RectTransform loadPanel;
    public RectTransform quitPanel;
    private Button[] buttons; 
    private void Start()
    {
        buttons = mainPanel.GetComponentsInChildren<Button>();
        settPanel.gameObject.SetActive(false);
        savePanel.gameObject.SetActive(false);
        loadPanel.gameObject.SetActive(false);
        quitPanel.gameObject.SetActive(false);
        mainPanel.SetSiblingIndex(2);
    }

    private void SetMain(bool active = true)
    {
        buttons ??= mainPanel.GetComponentsInChildren<Button>();
        foreach (Button button in buttons) 
            button.interactable = active;

        if (active)
            mainPanel.SetSiblingIndex(2);
        else
            mainPanel.SetSiblingIndex(1);
    }
    #region ButtonsEvents
    public void SettingsBtn()
    {
        SetMain(false);
        settPanel.gameObject.SetActive(true);
    }
    public void LoadBtn()
    {
        SetMain(false);
        loadPanel.gameObject.SetActive(true);
    }
    public void SaveBtn()
    {
        SetMain(false);
        savePanel.gameObject.SetActive(true);
    }
    public void QuitBtn()
    {
        SetMain(false);
        quitPanel.gameObject.SetActive(true);
    }
    public void DisableSubPanel()
    {
        SetMain(true);
        settPanel.gameObject.SetActive(false);
        savePanel.gameObject.SetActive(false);
        loadPanel.gameObject.SetActive(false);
        quitPanel.gameObject.SetActive(false);
    }
    #endregion
}
