using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }

    public GameObject player;
    public GameObject playerUI;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject Level;

    private PlayerScript playerScript;

    public static bool playerLives;
    public static bool isPaused;

    void Start()
    {
        playerScript = player.GetComponent<PlayerScript>();
        pauseMenu.SetActive(false);
        playerLives = true;
        isPaused = false;
        // Debug.Log(Application.persistentDataPath);
    }
    private void Update()
    {
        if (playerLives)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
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
        playerUI.SetActive(false);
        deathMenu.SetActive(true);
    }
    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
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
        SceneManager.LoadScene(0);
    }
    public void PlayerRevive() // Just for testing
    {
        playerScript.Resurect();
        playerLives = true;
        isPaused = false;
        Destroy(GameObject.FindGameObjectWithTag("Level"));
        Instantiate(Level, transform);
        deathMenu.SetActive(false);
        playerUI.SetActive(true);
        Time.timeScale = 1f;
    }
}