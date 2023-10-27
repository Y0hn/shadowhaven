using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject Level;
    public GameObject playerBluePrint;
    private PlayerScript player;

    public static bool playerLives;
    public static bool isPaused;

    void Start()
    {
        GameObject playerO = GameObject.FindGameObjectWithTag("Player");
        player = playerO.GetComponent<PlayerScript>();
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
        SaveSystem.SavePlayer(player);
    }
    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        player.SetHealth(data.health);
        player.SetLevel(data.level);
        player.SetPos(new Vector2(data.position[0], data.position[1]));
    }
    public void GoToMainMenu()
    { 
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void PlayerRevive() // Just for testing
    {
        player.Resurect();
        playerLives = true;
        isPaused = false;
        Destroy(GameObject.FindGameObjectWithTag("Level"));
        Instantiate(Level, transform);
        deathMenu.SetActive(false);
        playerUI.SetActive(true);
        Time.timeScale = 1f;
    }
}