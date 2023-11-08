using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public ManagerUI UI;
    public GameObject[] Levels;

    private PlayerScript playerScript;
    private ItemsList items;

    public int level;

    public static bool playerLives = true;
    public static bool isPaused = false;
    public static bool inventory = false;

    private bool sceneLoaded = false;
    //private bool bossBeaten = false;

    private const float refreshInvTime = 0.1f;

    void Start()
    {
        // References
        // Debug.Log(Application.persistentDataPath);
        playerScript = player.GetComponent<PlayerScript>();
        items = GetComponent<ItemsList>();

        LevelLoad(level);

        sceneLoaded = true;
    }
    private void Update()
    {
        if (Time.time > refreshInvTime)
            Inventory.instance.onItemChangeCallback.Invoke();
        if (!sceneLoaded) 
            ReloadScene();
        if (playerLives)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            else if (Input.GetKeyUp(KeyCode.E))
                OpenCloseInventory();
        }
        else
        {
            PlayerDeath();
        }
    }
    private void PlayerDeath()
    {
        Inventory.instance.ClearInventory();
        isPaused = true;
        Time.timeScale = 0f;
        UI.DisableUI(0);
        UI.EnableUI("death");
    }
    private void PauseGame()
    {
        if (inventory)
        {
            UI.DisableUI("inv");
            inventory = false;
        }
        else
        {
            UI.EnableUI("pause");
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
    private void ReloadScene()
    {
        PlayerRevive();
        sceneLoaded = true;
    }
    private void OpenCloseInventory()
    {
        if (!isPaused)
        {
            if (inventory)
                UI.DisableUI("inv");
            else
                UI.EnableUI("inv");

            inventory = !inventory;
        }
    }
    private void LevelLoad(int l)
    {
        GameObject oldLvl = GameObject.FindGameObjectWithTag("Level");

        if (oldLvl != null)
            Destroy(oldLvl);


        Instantiate(Levels[l], transform.position, Quaternion.identity);
        items.SetAll(GameObject.FindGameObjectWithTag("Level").GetComponent<ItemsList>().GetAll());
        // Odstrani uz vlastnene itemy z item poolu
        items.RemoveArray(Inventory.instance.GetEquipment());
    }


    public void ResumeGame()
    {
        UI.DisableUI("inv");
        UI.DisableUI("pause");
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void Save()
    {
        SaveSystem.SavePlayer(playerScript);
    }
    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        playerScript.SetHealth(data.health);
        playerScript.SetLevel(data.level);
        playerScript.SetPos(new Vector2(data.position[0], data.position[1]));
    }
    public void GoToMainMenu()
    { 
        Time.timeScale = 1f;
        sceneLoaded = false;
        SceneManager.LoadScene(0);
    }
    public void PlayerRevive()
    {
        playerScript.Resurect();
        playerLives = true;
        isPaused = false;
        LevelLoad(level);
        UI.ResetUI();
        Time.timeScale = 1f;
    }
}