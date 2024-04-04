using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static EviromentManager enviroment;
    public static NotificationsManager notifi;
    public new static CameraManager camera;
    public new static AudioManager audio;
    public static GameManager instance;
    public static LightManager lights;
    public static Inventory inventory;
    public static ManagerUI ui;
    private static Light2D globalLight;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Instance of GameManager!");
        }
        instance = this;
        enviroment = GetComponent<EviromentManager>();
        notifi = GetComponent<NotificationsManager>();
        camera = GetComponent<CameraManager>();
        inventory = GetComponent<Inventory>();
        lights = GetComponent<LightManager>();
        audio = GetComponent<AudioManager>();
        ui = GetComponent<ManagerUI>();
        // PRIVATE
        globalLight = GetComponent<Light2D>();
    }
    #endregion

    #region References

    private PlayerScript playerScript;
    private PlayerStats playerStats;
    public GameObject[] Levels;
    public GameObject player;
    private ItemsList items;

    #endregion

    public int eqiWeap;
    public int level;
    public bool ableToMove { get; set; }
    public bool playerLives = true;
    public bool generated = false;
    private bool qchange = true;
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
            if (Input.GetButtonDown("Pause"))
            {
                if (ableToMove)
                    PauseGame();
                else
                    ResumeGame();

                foreach (BossStats b in bosses)
                    if (b.Active())
                        b.ShowBar(ableToMove);
            }
            else if (Input.GetButtonDown("Inventory"))
            {
                OpenCloseInventory();
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetAxis("Qslots") < 0 || eqiWeap == 1) && qchange)
            {
                // Equip Primary
                if (inventory.Equiped(2) != null)
                    playerScript.ChangeWeapon(true, true);
                else
                    playerScript.SetActiveCombat(false);
                //Debug.Log("Primary equip");
                qchange = false;
                eqiWeap = 0;
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetAxis("Qslots") > 0 || eqiWeap == 2) && qchange)
            {
                // Equip Secondary
                if (inventory.Equiped(3) != null)
                    playerScript.ChangeWeapon(false, true);
                else
                    playerScript.SetActiveCombat(false);
                //Debug.Log("Secondary equip");
                qchange = false;
                eqiWeap = 0;
            }
            else if (!qchange && Input.GetAxis("Qslots") == 0)
            {
                qchange = true;
                //Debug.Log("Qchange reset");
            }
            else if (inv && Input.GetButtonDown("Submit"))
            {
                if (inventory.TryGetItem(0, out Item item))
                    item.Use();
                //Debug.Log("Cont E = " + Input.GetAxis("Cont E"));
            }
            else if (inv && Input.GetButtonDown("Cancel"))
            {
                CloseInventory();
            }
            /*else if (Input.GetKeyDown(KeyCode.Alpha3) || eqiWeap == 3)
            {
                inventory.QuickUse(3);
                eqiWeap = 0;
            }*/
            else if (Input.GetButtonDown("Jump"))
            {
                if (camera.IsCameraFocused("free"))
                {
                    camera.SkipCurrentSequence();
                }
            }
            else if (Input.GetKeyDown(KeyCode.P)) // DEBUG !!!!!
            {
                //playerStats.TakeDamage(int.MaxValue);
                playerStats.LevelUp();
            }
        }
        else
        {
            if (deathScreen)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
                {
                    PlayerRevive();
                }
                else if (Input.GetButtonDown("Pause") || Input.GetButtonDown("Cancel"))
                {
                    GoToMainMenu();
                }
            }
            else
                PlayerDeath();
        }
    }

    #region Private Events
    void LevelLoad()
    {
        GameObject[] oldLvls = GameObject.FindGameObjectsWithTag("Level");

        foreach (GameObject lvl in oldLvls)
        {
            //Debug.Log("Destroyed level: " + lvl.name);
            Destroy(lvl);
        }

        // Premenovanie Levela xdd
        GameObject levObj = Instantiate(Levels[level], transform.position, Quaternion.identity);
        levObj.name = levObj.name.Split('_')[0] + "_" + (level+1);
        for (int i = 0; i <= level; i++)
            items.SetAll(Resources.LoadAll<Item>($"Items/{levObj.name.Split('_')[0]}_{i+1}"));
        //Debug.Log($"Pridany pocet itemov [{items.Items.Count}]");
        if (level > 0)
        {
            items.EraseItemsOfRarity((Rarity)level);
            //Debug.Log("Min item Rarity set to " + (Rarity)level);
        }
        ableToMove = true;

        // Odstrani uz vlastnene itemy z item poolu
        items.RemoveArray(inventory.GetEquipment());
        items.RemoveArray(inventory.items.ToArray());
    }
    void PlayerDeath()
    {
        //GameManager.inventory.ClearInventory();
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

    #endregion

    #region Game Events
    public void PauseGame()
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
    public void OpenCloseInventory()
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
    public void OpenInventory()
    {
        ui.EnableUI("inv");
        inv = true;
    }
    public void SetGlobalLight(float f)
    {
        if (f > 0)
        {
            globalLight.intensity = f;
            globalLight.enabled = true;
        }
        else if (f == 0)
            globalLight.enabled = false;
        else // if (f < 0)
            globalLight.enabled = true;
    }
    public void CloseInventory() 
    {
        ui.DisableUI("inv");
        inv = false;
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