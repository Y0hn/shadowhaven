using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public PlayerScript player;

    public static bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            if (isPaused) 
                ResumeGame();
            else
                PauseGame();
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
        player.rb.position = new Vector2(data.position[0], data.position[1]);
    }
    public void GoToMainMenu()
    { 
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}