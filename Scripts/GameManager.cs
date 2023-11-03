using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public ManagerUI UI;
    public GameObject[] Levels;

    private PlayerScript playerScript;

    public static bool playerLives;
    public static bool isPaused;

    private bool inventory = false;
    private bool SceneLoaded = false;

    void Start()
    {
        // Debug.Log(Application.persistentDataPath);
        playerScript = player.GetComponent<PlayerScript>();
        playerLives = true;
        isPaused = false;
        SceneLoaded = true;
    }
    private void Update()
    {
        if (!SceneLoaded) 
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
        isPaused = true;
        Time.timeScale = 0f;
        UI.DisableUI(0);
        UI.EnableUI("death");
    }
    private void PauseGame()
    {
        UI.EnableUI("pause");
        Time.timeScale = 0f;
        isPaused = true;
    }
    private void ReloadScene()
    {
        PlayerRevive();
        SceneLoaded = true;
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
        SceneLoaded = false;
        SceneManager.LoadScene(0);
    }
    public void PlayerRevive()
    {
        playerScript.Resurect();
        playerLives = true;
        isPaused = false;
        Destroy(GameObject.FindGameObjectWithTag("Level"));
        Instantiate(Levels[0], transform);
        UI.ResetUI();
        Time.timeScale = 1f;
    }
}