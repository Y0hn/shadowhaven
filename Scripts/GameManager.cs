using System.Collections.Generic;
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

    public GameObject FreeCamera;
    public GameObject player;
    public ManagerUI UI;
    public GameObject[] Levels;

    #region References

    private PlayerCombatScript playerCombat;
    private PlayerScript playerScript;
    private PlayerStats playerStats;
    private GameObject playerCamera;
    private Inventory inventory;
    private ItemsList items;

    #endregion

    public int level;
    public int eqiWeap;

    public static bool ongoingBossFight = false;
    public static bool playerLives = true;
    public static bool ableToMove = true;
    public static bool inv = false;

    public bool cameraFocused = false;

    private const float cameraMove = 1f;
    private float timerCamera;

    private Dictionary<DoorBehavior, DoorType> doors = new();

    private bool movingCamera = false;
    private bool bossDefeaded = false;
    private bool sceneLoaded = false;
    private bool deathScreen = false;
    private bool moveTowards = true;
    private Vector2[] cameraPos = new Vector2[2];

    void Start()
    {
        // References
        // Debug.Log(Application.persistentDataPath);
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerCombat = player.GetComponent<PlayerCombatScript>();
        playerScript = player.GetComponent<PlayerScript>();
        playerStats = player.GetComponent<PlayerStats>();
        inventory = GetComponent<Inventory>();
        items = GetComponent<ItemsList>();

        LevelLoad();

        sceneLoaded = true;
    }
    void Update()
    {
        if (!sceneLoaded) 
            ReloadScene();
        if (playerLives)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ableToMove)
                    PauseGame();
                else
                    ResumeGame();
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

        if (movingCamera)
        {
            if (cameraFocused)
            {
                if (!moveTowards)
                {
                    movingCamera = false;
                    ChangeCamera();
                }
                else if (timerCamera <= Time.time)
                {
                    moveTowards = false;
                    cameraFocused = false;
                }
            }
            else if (moveTowards)
            {
                MoveCamera(3, ref FreeCamera, cameraPos[1]);

                if (FreeCamera.transform.position.x == cameraPos[1].x && FreeCamera.transform.position.y == cameraPos[1].y)
                {
                    cameraFocused = true;
                    timerCamera = Time.time + timerCamera;
                }
            }
            else
            {
                MoveCamera(5, ref FreeCamera, cameraPos[0]);

                if (FreeCamera.transform.position.x == cameraPos[0].x && FreeCamera.transform.position.y == cameraPos[0].y)
                {
                    cameraFocused = true;
                    timerCamera = Time.time + timerCamera;
                }
            }
        }

        if (bossDefeaded) // Loot room opening
        {
            foreach (DoorBehavior d in doors.Keys)
            {
                if (doors[d].Equals(DoorType.BossIn) || doors[d].Equals(DoorType.BossIn))
                {
                    d.ChangeState(true);
                }
            }
        }
    }
    #region Private Events
    void PauseGame()
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
            ableToMove = false;
        }
    }
    void OpenCloseInventory()
    {
        if (ableToMove)
        {
            if (inv)
                UI.DisableUI("inv");
            else
                UI.EnableUI("inv");

            inv = !inv;
        }
    }
    void LevelLoad()
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
    void PlayerDeath()
    {
        Inventory.instance.ClearInventory();
        Destroy(GameObject.FindGameObjectWithTag("Level"));
        UI.EnableUI("death");
        UI.DisableUI(0);
        deathScreen = true;
        ableToMove = false;
    }
    void ChangeCamera()
    {
        if (playerCamera.activeSelf)
        {
            Debug.Log("Camera changed to freeCam");
            FreeCamera.transform.position = new Vector3(cameraPos[0].x, cameraPos[0].y, FreeCamera.transform.position.z);
            playerCamera.SetActive(false);
            ableToMove = false;
            FreeCamera.SetActive(true);
        }
        else
        {
            Debug.Log("Camera changed to playerCam");
            playerCamera.SetActive(true);
            FreeCamera.SetActive(false);
            ableToMove = true;
        }
    }
    void ReloadScene()
    {
        PlayerRevive();
        sceneLoaded = true;
    }
    void MoveCamera(float force, ref GameObject camera, Vector2 pos)
    {
        Vector2 move = Vector2.MoveTowards(camera.transform.position, pos, force * Time.deltaTime);
        camera.transform.position = new Vector3(move.x, move.y, camera.transform.position.z);
    }

    #endregion

    #region Public Events
    public void ResumeGame()
    {
        UI.DisableUI("inv");
        UI.DisableUI("pause");
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
        playerScript.Resurect();
        deathScreen = false;
        playerLives = true;
        ableToMove = true;
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
    public void BossKilled()
    {
        bossDefeaded = true;
    }
    public void AddXp(int xp) 
    {
        playerStats.AddXp(xp);
    }
    public void AddDoor(DoorBehavior newDoor)
    {
        doors.Add(newDoor, newDoor.type);
    }
    public void MoveCameraTo(Vector2 B, float forTime)
    {
        if (!movingCamera)
        {
            cameraPos[0] = playerCamera.transform.position;
            cameraPos[1] = B;
            timerCamera = forTime;
            movingCamera = true;
            ChangeCamera();
        }
    }
    public void MoveCameraTo(Vector2 A, Vector2 B, float forTime)
    {
        if (!movingCamera)
        {
            cameraPos[0] = A;
            cameraPos[1] = B;
            timerCamera = forTime;
            movingCamera = true;
            ChangeCamera();
        }
    }
    #endregion
}