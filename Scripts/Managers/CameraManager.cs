using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
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
    private List<string> cameraSequence = new();
    private const float camSpeed = 5f;
    private int cameraSeqFollower;
    private bool proceedInSeq;
    private string curCamSeq;
    private Kamera[] cameras;
    public Transform freeCam;

    void Start()
    {
        // Camera SetUp
        cameras = new Kamera[2];
        cameras[0] = new Kamera(GameObject.FindGameObjectWithTag("MainCamera").transform, "player");
        cameras[1] = new Kamera(freeCam, "free");
        Kamera.ChangeCamera(cameras, 0);
        cameraSeqFollower = 0;
        proceedInSeq = true;
    }
    void Update()
    {
        if (cameraSequence.Count > 0)
        {
            if (proceedInSeq)
            {
                //Debug.Log($"Proceed in sequence: ({mode-1}) => {cameraSequence[cameraSeqFollower]}({mode})");

                if (cameraSeqFollower == 0)
                {
                    Kamera.GetCamera(cameras, "free").SetPosition(cameras[0].camera.transform.position);
                    Kamera.ChangeCamera(cameras, "free");
                    GameManager.instance.ableToMove = false;
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
                    if (cameraSequence[cameraSeqFollower - 1] == "toBoss" && !GameManager.instance.boss.onCamera)
                    {
                        GameManager.lights.LightTypeTurn(0, LightManager.LightType.Boss);
                        GameManager.instance.boss.onCamera = true;
                    }
                    else if (cameraSequence[cameraSeqFollower - 1] == "toDoor" && cameraSequence[cameraSeqFollower] == "toBoss")
                        GameManager.enviroment.OpenDoors(DoorType.BossIn, false);
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
                    GameManager.instance.boss.onCamera = false;
                    cameraSequence = new();
                    GameManager.instance.ableToMove = true;

                    debug += " which ended the Sequence";
                }

                //Debug.Log(debug);
            }
        }
    }
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
            camera.position = new(position.x, position.y, camera.position.z);
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
            GameManager.camera.GetModeTarget(mode, out target);
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
    public string[] CameraSequence(string seqName, bool get = false)
    {
        string[] output;
        switch (seqName)
        {
            case "boss":
                output = new string[3]
                { "toDoor", "toBoss", "toPlayer" };
                cameraSeqFollower = 0;
                proceedInSeq = true;
                break;

            default:
                output = null;
                Debug.LogWarning("There is no such camera seqeunce as: " + seqName);
                break;
        }

        // Output
        if (get)
        {
            return output;
        }
        else
        {
            cameraSequence = output.ToList();
            curCamSeq = seqName;
            return null;
        }
    }
    public void SkipCurrentSequence()
    {
        cameraSequence = new();
        if (curCamSeq == "boss")
        {
            GameManager.lights.LightTypeTurn(0, LightManager.LightType.Boss);
            GameManager.lights.LightTypeTurn(1, LightManager.LightType.Boss);
            GameManager.enviroment.OpenDoors(DoorType.BossIn, false);
        }
        Kamera.ChangeCamera(cameras,0);
        GameManager.instance.ableToMove = true;
        cameraSeqFollower = 0;
        proceedInSeq = true;
    }
    private void GetModeTarget(int mode, out Vector2 position)
    {
        //bool suc = true;
        switch (mode)
        {
            case 0: // toPlayer
                position = Kamera.GetCamera(cameras, "player").camera.transform.position;
                break;
            case 1: // toBoss
                position = GameManager.instance.boss.transform.position;
                break;
            case 2: // toDoor
                position = GameManager.enviroment.GetDoors(DoorType.BossIn)[0].GetClosedPos();
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
}