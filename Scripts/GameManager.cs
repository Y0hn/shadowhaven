using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("More than one Instance of GameManager!");
        instance = this;
    }
    #endregion

    public GameObject player;
    public ManagerUI UI;
    public GameObject[] Levels;

    #region References

    private PlayerScript playerScript;
    private PlayerStats playerStats;
    private PlayerCombatScript playerCombat;
    private ItemsList items;
    private Inventory inventory;

    #endregion

    public int level;
    public int eqiWeap;

    public static bool playerLives = true;
    public static bool isPaused = false;
    public static bool inv = false;

    private bool sceneLoaded = false;
    private bool deathScreen = false;

    void Start()
    {
        // References
        // Debug.Log(Application.persistentDataPath);
        playerCombat = player.GetComponent<PlayerCombatScript>();
        playerScript = player.GetComponent<PlayerScript>();
        playerStats = player.GetComponent<PlayerStats>();
        inventory = GetComponent<Inventory>();
        items = GetComponent<ItemsList>();

        LevelLoad();

        sceneLoaded = true;
    }
    private void Update()
    {
        if (!sceneLoaded) 
            ReloadScene();
        if (playerLives)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
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
        }
        else
        {
            if (deathScreen)
            {
                if      (Input.GetKeyDown(KeyCode.Space))
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
    #region Events
    private void PauseGame()
    {
        if (inv)
        {
            UI.DisableUI("inv");
            inv = false;
        }
        else
        {
            UI.EnableUI("pause");
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
    private void OpenCloseInventory()
    {
        if (!isPaused)
        {
            if (inv)
                UI.DisableUI("inv");
            else
                UI.EnableUI("inv");

            inv = !inv;
        }
    }
    private void LevelLoad()
    {
        GameObject oldLvl = GameObject.FindGameObjectWithTag("Level");

        if (oldLvl != null)
            Destroy(oldLvl);

        // Premenovanie Levela xdd
        GameObject levObj = Instantiate(Levels[level], transform.position, Quaternion.identity);
        levObj.name = levObj.name.Split('_')[0] + "_" +levObj.name.Split('_')[1].Split('(')[0];
        items.SetAll(Resources.LoadAll<Item>($"Levels/{levObj.name}"));

        // Odstrani uz vlastnene itemy z item poolu
        items.RemoveArray(Inventory.instance.GetEquipment());
    }
    private void PlayerDeath()
    {
        Inventory.instance.ClearInventory();
        Destroy(GameObject.FindGameObjectWithTag("Level"));
        UI.EnableUI("death");
        UI.DisableUI(0);
        deathScreen = true;
        isPaused = true;
    }
    private void ReloadScene()
    {
        PlayerRevive();
        sceneLoaded = true;
    }

    #endregion

    #region Public Events

    public void ResumeGame()
    {
        UI.DisableUI("inv");
        UI.DisableUI("pause");
        Time.timeScale = 1f;
        isPaused = false;
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
        deathScreen = false;
        playerLives = true;
        isPaused = false;
        LevelLoad();
        UI.ResetUI();
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

    #endregion
}