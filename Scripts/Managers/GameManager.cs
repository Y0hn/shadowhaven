using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static EviromentManager enviroment;
    public new static CameraManager camera;
    public new static AudioManager audio;
    public static GameManager instance;
    public static LightManager lights;
    public static Inventory inventory;
    public static ManagerUI ui;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Instance of GameManager!");
        }
        instance = this;
        enviroment = GetComponent<EviromentManager>();
        camera = GetComponent<CameraManager>();
        inventory = GetComponent<Inventory>();
        lights = GetComponent<LightManager>();
        audio = GetComponent<AudioManager>();
        ui = GetComponent<ManagerUI>();
    }
    #endregion

    #region References

    public GameObject[] Levels;
    public GameObject player;

    private PlayerScript playerScript;
    private PlayerStats playerStats;
    private ItemsList items;

    #endregion

    public int eqiWeap;
    public int level;
    public bool ableToMove { get; set; }
    public bool playerLives = true;
    public bool generated = false;
    public bool inv = false;

    // Lists
    public List<BossStats> bosses = new();

    private bool sceneLoaded = false;
    private bool deathScreen = false;

    void Start()
    {
        //// REFERENCES \\\\
        
        // Player References
        if (player == null) // if player reference not set manualy
        {
            player = GameObject.FindWithTag("Player");
            Start();
            return;
        }
        playerScript = player.GetComponent<PlayerScript>();
        playerStats = player.GetComponent<PlayerStats>();
        inventory = GetComponent<Inventory>();
        items = GetComponent<ItemsList>();

        // Level SetUp
        LevelLoad();
        sceneLoaded = true;
    }
    void Update()
    {
        if (!sceneLoaded)
            ReloadScene();
        if (playerLives)
        {
            // INPUT \\
            if      (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ableToMove)
                    PauseGame();
                else
                    ResumeGame();

                foreach (BossStats b in bosses)
                    if (b.Active())
                        b.ShowBar(ableToMove);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                OpenCloseInventory();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || eqiWeap == 1)
            {
                // Equip Primary
                if (inventory.Equiped(2) != null)
                    playerScript.ChangeWeapon(true, true);
                else
                    playerScript.SetActiveCombat(false);

                eqiWeap = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || eqiWeap == 2)
            {
                // Equip Secondary
                if (inventory.Equiped(3) != null)
                    playerScript.ChangeWeapon(false, true);
                else
                    playerScript.SetActiveCombat(false);

                eqiWeap = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || eqiWeap == 3)
            {
                inventory.QuickUse(3);
                eqiWeap = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (camera.IsCameraFocused("free"))
                {
                    camera.SkipCurrentSequence();
                }
            }
        }
        else
        {
            if (deathScreen)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerRevive();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GoToMainMenu();
                }
            }
            else
                PlayerDeath();
        }
    }

    #region Private Events
    void PauseGame()
    {
        if (inv)
        {
            ui.DisableUI("inv");
            inv = false;
        }
        else
        {
            ui.DisableUI(0);
            ui.EnableUI("pause");
            audio.PauseTheme();
            Time.timeScale = 0f;
            ableToMove = false;
        }
    }
    void LevelLoad()
    {
        GameObject[] oldLvls = GameObject.FindGameObjectsWithTag("Level");

        foreach (GameObject lvl in oldLvls)
        {
            Debug.Log("Destroyed level: " + lvl.name);
            Destroy(lvl);
        }

        // Premenovanie Levela xdd
        GameObject levObj = Instantiate(Levels[level], transform.position, Quaternion.identity);
        levObj.name = levObj.name.Split('_')[0] + "_" + levObj.name.Split('_')[1].Split('-')[1].Split('(')[0];
        items.SetAll(Resources.LoadAll<Item>($"Items/{levObj.name}"));
        ableToMove = true;
        // Odstrani uz vlastnene itemy z item poolu
        items.RemoveArray(Inventory.instance.GetEquipment());
    }
    void PlayerDeath()
    {
        Inventory.instance.ClearInventory();
        Destroy(GameObject.FindGameObjectWithTag("Level"));
        audio.PlayTheme("stop");
        ui.EnableUI("death");
        ui.DisableUI(0);
        deathScreen = true;
        ableToMove = false;
    }
    void ReloadScene()
    {
        PlayerRevive();
        lights.Start();
        sceneLoaded = true;
    }
    void OpenCloseInventory()
    {
        if (ableToMove)
        {
            if (inv)
                ui.DisableUI("inv");
            else
                ui.EnableUI("inv");

            inv = !inv;
        }
    }

    #endregion

    #region Game Events
    public void ResumeGame()
    {
        ui.EnableUI(0);
        ui.DisableUI("inv");
        ui.DisableUI("pause");
        audio.PauseTheme();
        Time.timeScale = 1f;
        ableToMove = true;
    }
    public void GoToMainMenu()
    { 
        Time.timeScale = 1f;
        sceneLoaded = false;
        SceneManager.LoadScene(0);
    }
    public void PlayerRevive()
    {
        audio.PlayTheme("theme");
        playerScript.Resurect();
        deathScreen = false;
        playerLives = true;
        ableToMove = true;
        LevelLoad();
        ui.ResetUI();
        Time.timeScale = 1f;
    }
    public void Save()
    {
        SaveSystem.SavePlayer(playerScript, playerStats);
    }
    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        playerStats.SetHealth(data.health);
        playerStats.level = data.level;
        playerScript.SetPos(new Vector2(data.position[0], data.position[1]));
    }
    public void AddXp(int xp) 
    {
        playerStats.AddXp(xp);
    }
    public void AddBoss(BossStats boss)
    {
        if (generated)
            bosses.Add(boss);
    }
    public void BossKilled(BossStats boss, bool onDestroy = false)
    {
        if (generated && bosses.Contains(boss))
        {
            if (!onDestroy)
            {
                enviroment.OpenDoors(DoorType.BossOut);
                enviroment.OpenDoors(DoorType.BossIn);
            }
            bosses.Remove(boss);
        }
    }
    public void NextLevel()
    {
        level++;
        if (level < Levels.Length)
        {
            LevelLoad();
        }
        else
            EndTheGame();
    }
    private void EndTheGame()
    {
        Time.timeScale = 1f;
        sceneLoaded = false;
        SceneManager.LoadScene(2);
    }

    #endregion
}