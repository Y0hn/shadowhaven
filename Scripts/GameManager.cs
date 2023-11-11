using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public ManagerUI UI;
    public GameObject[] Levels;

    #region References

    private PlayerScript playerScript;
    private ItemsList items;
    private Inventory inventory;

    #endregion

    public int level;
    public int eqiWeap;

    public static bool playerLives = true;
    public static bool isPaused = false;
    public static bool inv = false;

    private bool sceneLoaded = false;
    //private bool bossBeaten = false;

    private const float refreshInvTime = 0.1f;

    void Start()
    {
        // References
        // Debug.Log(Application.persistentDataPath);
        playerScript = player.GetComponent<PlayerScript>();
        inventory = GetComponent<Inventory>();
        //inventory.onItemChangeCallback += Hlasatel;
        items = GetComponent<ItemsList>();

        LevelLoad();

        sceneLoaded = true;
    }
    private void Update()
    {
        /*  Neviem naco to tu je xd 
         *  toto vobec nebol uneficient workaround :D
        if (Time.time > refreshInvTime)
            Inventory.instance.onItemChangeCallback.Invoke();
        /**/
        if (!sceneLoaded) 
            ReloadScene();
        if (playerLives)
        {
            if      (Input.GetKeyUp(KeyCode.Escape))
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
            else if (Input.GetKeyUp(KeyCode.E))
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
            PlayerDeath();
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
            inv = false;
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

        Instantiate(Levels[level], transform.position, Quaternion.identity);
        ItemsList levelItems = GameObject.FindGameObjectWithTag("Level").GetComponent<ItemsList>();

        items.SetAll(levelItems.GetAll());
        Destroy(levelItems);
        // Odstrani uz vlastnene itemy z item poolu
        items.RemoveArray(Inventory.instance.GetEquipment());
    }

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
        playerLives = true;
        isPaused = false;
        LevelLoad();
        UI.ResetUI();
        Time.timeScale = 1f;
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

    #endregion
}