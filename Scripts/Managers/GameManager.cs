using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;
    public static LightManager lights;
    public static Inventory inventory;
    public new static AudioManager audio;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Instance of GameManager!");
        }
        instance = this;
        lights = GetComponent<LightManager>();
        inventory = GetComponent<Inventory>();
        audio = GetComponent<AudioManager>();
    }
    #endregion

    #region References

    public GameObject[] Levels;
    public GameObject player;
    public Transform freeCam;
    public ManagerUI UI;

    private PlayerCombatScript playerCombat;
    private PlayerScript playerScript;
    private PlayerStats playerStats;
    private ItemsList items;

    #endregion

    public int eqiWeap;
    public int level;

    public static bool playerLives = true;
    public static bool ableToMove = true;
    public static bool generated = false;
    public static bool inv = false;

    // Lists
    private Kamera[] cameras;
    private int cameraSeqFollower;
    private List<BossStats> bosses = new();
    private List<DoorBehavior> doors = new();
    private List<string> cameraSequence = new();
    private static readonly Dictionary<string, float> camModer = new()
    {
        // Modes
        { "toPlayer",   0 },
        { "toBoss",     1 },
        { "toDoor",     2 },

        // Parameters 
        { "speed 0", 3f },
        { "haltTime 0", 0f },

        { "speed 1", 2f },
        { "haltTime 1", 2f},

        { "speed 2", 1f},
        { "haltTime 2", 1.5f},
    };

    private bool sceneLoaded = false;
    private bool deathScreen = false;
    private bool proceedInSeq;
    private const float camSpeed = 5f;

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
        Transform playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerCombat = player.GetComponent<PlayerCombatScript>();
        playerScript = player.GetComponent<PlayerScript>();
        playerStats = player.GetComponent<PlayerStats>();
        inventory = GetComponent<Inventory>();
        items = GetComponent<ItemsList>();

        // Camera SetUp
        cameras = new Kamera[2];
        cameras[0] = new Kamera(playerCamera, "player");
        cameras[1] = new Kamera(freeCam, "free");
        Kamera.ChangeCamera(cameras, 0);
        cameraSeqFollower = 0;
        proceedInSeq = true;

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
            if (Input.GetKeyDown(KeyCode.Escape))
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

        if (cameraSequence.Count > 0)
        {
            if (proceedInSeq)
            {
                //Debug.Log($"Proceed in sequence: ({mode-1}) => {cameraSequence[cameraSeqFollower]}({mode})");

                if (cameraSeqFollower == 0)
                {
                    Kamera.GetCamera(cameras, "free").SetPosition(cameras[0].camera.transform.position);
                    Kamera.ChangeCamera(cameras, "free");
                    ableToMove = false;
                }
                proceedInSeq = false;
            }

            Kamera k = Kamera.GetFocusedCamera(cameras);
            if (k.moving)
            {
                if (k.haltTime < Time.time)
                    proceedInSeq = k.MoveTowards(Time.deltaTime * camSpeed);
            }
            else
            {
                if (cameraSeqFollower > 0 && cameraSeqFollower < cameraSequence.Count)
                {
                    if (cameraSequence[cameraSeqFollower - 1] == "toBoss" && !bosses[0].onCamera)
                    {
                        lights.LightTypeTurn(0, LightManager.LightType.Boss);
                        bosses[0].onCamera = true;
                    }
                    else if (cameraSequence[cameraSeqFollower - 1] == "toDoor" && cameraSequence[cameraSeqFollower] == "toBoss")
                        OpenDoors(DoorType.BossIn, false);
                }
                if (k.haltTime < Time.time)
                    k.SetUp((int)camModer[cameraSequence[cameraSeqFollower]]);
            }

            if (proceedInSeq)
            {
                k.haltTime += Time.time;
                cameraSeqFollower++;
                string debug = "Proseed in sequence to step " + cameraSeqFollower;

                if (cameraSequence.Count <= cameraSeqFollower)
                {
                    Kamera.ChangeCamera(cameras, 0);
                    bosses[0].onCamera = false;
                    cameraSequence = new();
                    ableToMove = true;                    

                    debug += " which ended the Sequence";
                }

                //Debug.Log(debug);
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
            UI.DisableUI(0);
            UI.EnableUI("pause");
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
        levObj.name = levObj.name.Split('_')[0] + "_" +levObj.name.Split('_')[1].Split('(')[0];
        items.SetAll(Resources.LoadAll<Item>($"Levels/{levObj.name}"));

        // Odstrani uz vlastnene itemy z item poolu
        items.RemoveArray(Inventory.instance.GetEquipment());
    }
    void PlayerDeath()
    {
        Inventory.instance.ClearInventory();
        Destroy(GameObject.FindGameObjectWithTag("Level"));
        audio.PlayTheme("stop");
        UI.EnableUI("death");
        UI.DisableUI(0);
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
                UI.DisableUI("inv");
            else
                UI.EnableUI("inv");

            inv = !inv;
        }
    }

    #endregion

    #region Game Events
    public void ResumeGame()
    {
        UI.EnableUI(0);
        UI.DisableUI("inv");
        UI.DisableUI("pause");
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
    public void AddBoss(BossStats boss)
    {
        if (generated)
            bosses.Add(boss);
    }
    public void BossKilled(BossStats boss)
    {
        if (generated && bosses.Contains(boss))
        {
            OpenDoors(DoorType.BossOut);
            OpenDoors(DoorType.BossIn);
            bosses.Remove(boss);
        }
    }
    public void EndTheGame()
    {
        Time.timeScale = 1f;
        sceneLoaded = false;
        SceneManager.LoadScene(2);
    }

    #endregion

    #region Doors
    public void AddDoor(DoorBehavior newDoor)
    {
        doors.Add(newDoor);
    }
    public void RemoveDoor(DoorBehavior oldDoor)
    {
        doors.Remove(oldDoor);
    }
    public void OpenDoors(DoorType type, bool open = true)
    {
        foreach (DoorBehavior d in doors)
            if (d.type.Equals(type))
                d.ChangeState(open);
    }
    public DoorBehavior[] GetDoors(DoorType type)
    {
        List<DoorBehavior> dos = new();
        foreach (DoorBehavior d in doors)
            if (d.type == type)
                dos.Add(d);
        return dos.ToArray();
    }

    #endregion

    private class Kamera
    {
        public string name;
        public float haltTime;
        private Vector2 target;
        public Transform camera;
        private float moveSpeed;
        public static int FocusedCamera = 0;
        public bool moving { get; private set; }
        public bool focused { get; private set; }

        public Kamera(Transform camera, string name)
        {
            target = Vector2.zero;
            this.camera = camera;
            this.name = name;
            focused = false;
            moving = false;
            moveSpeed = 1;
            haltTime = 0;
        }
        public void SetPosition(Vector2 position)
        {
            //Debug.Log($"Teleporting {name.ToUpper()} from [{camera.position.x}, {camera.position.y}] to [{position.x}, {position.y}]");
            camera.position = new (position.x, position.y, camera.position.z);
        }
        public bool SetTarget(Vector2 newTarget)
        {
            //Debug.Log($"Target {name.ToUpper()} setted from [{target.x}, {target.y}] to [{newTarget.x}, {newTarget.y}]");
            if (!moving && camera.position.x != newTarget.x && camera.position.y != newTarget.y)
            {
                target = newTarget;
                moving = true;
                return true;
            }
            return false;
        }
        public void ChangeCamera(Kamera[] list)
        {
            list[FocusedCamera].focused = false;
            FocusedCamera = GetIndex(list, this);
            Enabled(true);
        }
        public bool MoveTowards(float speed)
        {
            if (target.x == camera.position.x && target.y == camera.position.y)
            {
                //Debug.Log($"Target [{target.x}, {target.y}] reached!");
                moving = false;
            }
            else // if need moving
            {
                Vector2 move = Vector2.MoveTowards(camera.transform.position, target, speed * moveSpeed);
                camera.transform.position = new Vector3(move.x, move.y, camera.transform.position.z);
                //Debug.Log($"Moving {name.ToUpper()} towards [{target.x}, {target.y}]");
            }

            return !moving;
        }
        public bool Focused(Vector2 target)
        {
            return focused && camera.position.x == target.x && camera.position.y == target.y;
        }
        public void SetSpeed(float speed)
        {
            moveSpeed = speed;
        }
        public void Enabled(bool active)
        {
            camera.gameObject.SetActive(active);
            focused = active;
        }
        public void SetUp(int mode)
        {
            instance.GetModeTarget(mode, out target);
            haltTime = camModer["haltTime " + mode];
            moveSpeed = camModer["speed " + mode];
            moving = true;

            //Debug.Log($"Camera {name} set to Mode [{instance.GetModReverse(mode)}]:\ntarget on: [{target.x}, {target.y}]\nhalt time: {haltTime}\nmove speed: {moveSpeed}");
        }
        public static void ChangeCamera(Kamera[] list, string camName)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].name == camName)
                {
                    list[FocusedCamera].Enabled(false);
                    list[i].Enabled(true);
                    FocusedCamera = i;
                }
        }
        public static void ChangeCamera(Kamera[] list, int camIndex)
        {
            list[FocusedCamera].Enabled(false);
            list[camIndex].Enabled(true);
            FocusedCamera = camIndex;
        }
        public static Kamera GetCamera(Kamera[] list, string name)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].name == name)
                    return list[i];
            return null;
        }
        private static int GetIndex(Kamera[] list, Kamera kamera)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == kamera)
                    return i;
            }
            return -1;
        }
        public static Kamera GetFocusedCamera(Kamera[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].focused)
                    return list[i];
            return null;
        }
    }
    public bool IsCameraFocused(string name)
    {
        return Kamera.GetCamera(cameras, name).focused;
    }
    public void CameraSequence(string seqName)
    {
        switch (seqName)
        {
            case "boss":
                cameraSequence = new()
                { "toDoor", "toBoss", "toPlayer" };
                cameraSeqFollower = 0;
                proceedInSeq = true;
                break;
            
            default:
                Debug.LogWarning("There is no such camera seqeunce as: " + seqName);
                break;
        }
    }
    private void GetModeTarget(int mode, out Vector2 position)
    {
        //bool suc = true;
        switch (mode)
        {
            case 0:
                position = Kamera.GetCamera(cameras, "player").camera.transform.position;
                break;
            case 1:
                position = bosses[0].transform.position;
                break;
            case 2:
                position = GetDoors(DoorType.BossIn)[0].GetClosedPos();
                break;

            default:
                Debug.LogWarning("Mode " + mode + " was not identified! ");
                position = Vector2.zero;
                //suc = false;
                break;
        }
        /*
        if (suc)
            Debug.Log($"Target of mode: {GetModReverse(mode)}({mode}) position recieved [{position.x}, {position.y}]");*/
    }
    
    /*private string GetModReverse(float val)
    {
        foreach (string s in camModer.Keys)
            if (camModer[s] == val)
                return s;
        return "[NULL]";
    }*/
}