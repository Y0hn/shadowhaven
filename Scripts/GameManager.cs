using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private PlayerScript player;

    public static bool playerLives;
    public static bool isPaused;

    private float afterDeath;

    void Start()
    {
        GameObject playerO = GameObject.FindGameObjectWithTag("Player");
        player = playerO.GetComponent<PlayerScript>();
        pauseMenu.SetActive(false);
        playerLives = true;
        isPaused = false;
        afterDeath = -1;
        Debug.Log(Application.persistentDataPath);
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
            if (afterDeath == -1)
                afterDeath = Time.time + 2;
            else if (afterDeath <= Time.time)
                PauseGame();
        }
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
}