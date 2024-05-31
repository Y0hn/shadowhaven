using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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
    public PlayerStats playerStats;
    public GameObject player;
    private ItemsList items;
    // Lists
    public GameObject[] Levels;
    public BossStats boss;

    #endregion

    public int eqiWeap;
    public int level;
    private float uptoLevelLoad;
    public bool ableToMove { get; set; }
    public bool playerLives = true;
    public bool generated = false;
    private bool qchange = true;
    public bool inv = false;

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
        uptoLevelLoad = -0.5f;
        if (SaveSystem.fileDataLoaded)
        {
            Debug.Log("File data load!");
            uptoLevelLoad = Time.time + Mathf.Abs(uptoLevelLoad);
            sceneLoaded = false;

            // ENABLE LOADING SCREEN
        }
        else // Level SetUp
        {
            Debug.Log("New level creation!");
            LevelLoad();
            sceneLoaded = true;
        }

    }
    void Update()
    {
        if (!sceneLoaded)
        {
            if (uptoLevelLoad < 0)
                ReloadScene();
            else if (uptoLevelLoad < Time.time)
            {
                Load();
                sceneLoaded = true;
            }
        }
        if (playerLives)
        {
            // INPUT \\
            if (Input.GetButtonDown("Pause"))
            {
                if (ableToMove)
                    PauseGame();
                else
                    ResumeGame();

                if (boss != null)
                    if (boss.Active())
                        boss.ShowBar(ableToMove);
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
            // DEBUG !!!!!
            else if (Input.GetKeyDown(KeyCode.P))
            {
                //playerStats.TakeDamage(int.MaxValue);
                //playerStats.LevelUp();
                Save();
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                LevelLoad();
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log(SaveSystem.CheckSaveNeed());
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                boss.Uninstall();
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
    Transform LevelLoad()
    {
        LevelClear();
        // Premenovanie Levela xdd
        GameObject levObj = Instantiate(Levels[level], transform.position, Quaternion.identity);
        levObj.name = levObj.name.Split('_')[0] + "_" + (level+1);
        for (int i = 0; i <= level; i++)
            items.SetAll(Resources.LoadAll<Item>($"Items/{levObj.name.Split('_')[0]}_{i+1}"));
        // Debug.Log($"Pridany pocet itemov [{items.Items.Count}]");
        if (level > 0)
        {
            items.EraseItemsOfRarity((Rarity)level);
            //Debug.Log("Min item Rarity set to " + (Rarity)level);
        }
        ableToMove = true;

        // Odstrani uz vlastnene itemy z item poolu
        items.RemoveArray(inventory.GetEquipment());
        items.RemoveArray(inventory.items.ToArray());
        audio.PlayTheme("stop");
        return levObj.transform;
    }
    void LevelClear()
    {
        GameObject[] oldLvls = GameObject.FindGameObjectsWithTag("Level");
        foreach (GameObject lvl in oldLvls)
        { //Debug.Log("Destroyed level: " + lvl.name);
            Destroy(lvl);
        }
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
        audio.PlayTheme("theme" + level);
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
        SaveSystem.Save();
    }
    public void Load()
    {
        SaveSystem.Data data = SaveSystem.Load();
        inventory.Load(data.inventory);
        level = data.curLevel;
        playerStats.LoadHealth(data.entities[0].curHealth, data.entities[0].maxHealth);
        //player.transform.position = data.entities[0].position.GetVector();
        LevelLoad().GetComponentInChildren<LevelGener>().LoadFromData(data);
        SaveSystem.fileDataLoaded = false;
        inventory.onEquipChangeCallback();
        inventory.onItemChangeCallback();
        audio.PlayTheme("stop");
        audio.PlayTheme("theme" + level);
    }
    public void AddXp(int xp) 
    {
        playerStats.AddXp(xp);
    }
    public void SetBoss(BossStats boss)
    {
        this.boss = boss;
    }
    public BossStats GetBoss()
    {
        return boss;
    }
    public void BossKilled(bool onDestroy = false)
    {
        if (boss != null)
        {
            if (!onDestroy)
            {
                enviroment.OpenDoors(DoorType.BossOut);
                enviroment.OpenDoors(DoorType.BossIn);
                Debug.Log("Boss destroyed and Gates opening");
            }
            else
            {
                Debug.Log("Boss destroyed");
            }
            boss = null;
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
    public void DisEnableAllMobs(bool enable)
    {
        GameObject[] ene = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in ene) 
        {
            e.GetComponent<EnemyStats>().ResumePause(enable);
        }
    }

    #endregion
}