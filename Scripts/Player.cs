using System.Collections.Generic;
using UnityEngine;
public class PlayerScript : MonoBehaviour
{    
    public Sprite empty;
    public LayerMask enemyLayers;
    public Vector2 hitBox;

    public float interRange;

    #region References

    private PlayerCombatScript playerCom;
    private Inventory inventory;
    private Animator animator;
    private PlayerStats stats;
    private Rigidbody2D rb;
    private SpriteRenderer torso;
    private SpriteRenderer helmet;

    #endregion

    #region Timers

    private float nextPickup = 0;
    private const float pickUpTime = 1;

    #endregion

    private int numE;
    private int helmetTI = 0;   // texture index
    private int torsoTI = 0;

    private const int minDmg = 1;

    private Vector2 moveDir;
    private float moveSpeed = 5;

    private bool weaponJustEquiped;
    private bool armed = false;
    private bool combatAct = false;
    private bool primaryWeap = true;
    private bool enablePiskup = true;



    private void Start() 
    {
        #region References

        playerCom = GetComponent<PlayerCombatScript>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        inventory = Inventory.instance;
        inventory.onItemChangeCallback += UpdateEquipment;

        numE = System.Enum.GetNames(typeof(EquipmentSlot)).Length + 1;

        /* CHILDREN 
         * 1 - BODY   
         *      0 - head 
         *          0 - helmet
         *      1 - torso
         *          0 - helmet
         */
        helmet = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        torso = transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();

        #endregion

        // Set up
        playerCom.enabled = false;
        combatAct = playerCom.enabled;
    }
    private void Update()
    {
        if (!GameManager.isPaused)
        {
            ProcessInput();
            Move();
            AnimateMovement();

            CollisionCheck();
            PickUpCheck();

            if (Input.GetKeyDown(KeyCode.P))
                stats.TakeDamage(int.MaxValue);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(hitBox.x, hitBox.y, 0));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interRange);

        Transform hand = transform.GetChild(0).GetChild(0);
        if (hand.parent.gameObject.activeSelf)
        {
        Vector2 pos = hand.position;
        float range = GetComponent<PlayerCombatScript>().attackRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, range);
        }
    }

    #region Input Syestem

    private void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if      ((Input.GetMouseButton(0) && !combatAct && armed) || weaponJustEquiped)
        {
            weaponJustEquiped = false;
            playerCom.enabled = true;
            combatAct = true;
        }
        else if (Input.GetMouseButtonDown(1) && armed && combatAct)
            primaryWeap = !primaryWeap;
        else if ((Input.GetMouseButton(2) || !armed) && combatAct)
        {
            playerCom.enabled = false;
            combatAct = false;
        }
    }
    private void Move()
    { rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed); }
    private void AnimateMovement()
    {
        animator.SetFloat("Horizontal", moveDir.x);
        animator.SetFloat("Vertical", moveDir.y);
        animator.SetFloat("Speed", moveDir.sqrMagnitude);


        if (animator.GetFloat("Speed") > 0)
        {
            Armor a;
            if (moveDir.y < 0)
            {
                helmetTI = 0;
                torsoTI = 0;
            }
            else if (moveDir.x > 0)
            {
                helmetTI = 1;
                torsoTI = 0;
            }
            else if (moveDir.x < 0)
            {
                helmetTI = 1;
                torsoTI = 0;
            }
            else if (moveDir.y > 0)
            {
                helmetTI = 2;
                torsoTI = 1;
            }

            a = (Armor)inventory.Equiped(0);
            if (a != null)
                if (a.texture.Length > 1)
                {
                    helmet.sprite = a.texture[helmetTI];
                    helmet.flipX = (moveDir.x < 0);
                }

            a = (Armor)inventory.Equiped(1);
            if (a != null)
                if (a.texture.Length > 1)
                {
                    torso.sprite = a.texture[torsoTI];
                }
        }
        else
        {
            Armor a;
            if (helmetTI != 0)
            {
                a = (Armor)inventory.Equiped(0);
                if (a != null)
                    helmet.sprite = a.texture[0];
                helmetTI = 0;
            }
            if (torsoTI != 0)
            {
                a = (Armor)inventory.Equiped(1);
                if (a != null)
                    torso.sprite = a.texture[0];
                torsoTI = 0;
            }
        }
    }

    #endregion

    private void CollisionCheck()
    {
        if (Time.time > stats.nextDamage)
        {
            // Get Coilidning Enemies
            Collider2D[] Enemies = Physics2D.OverlapBoxAll(transform.position, hitBox, 0, enemyLayers);
            // Enemis Doin Damage
            foreach (Collider2D enemy in Enemies)
                stats.TakeDamage(enemy.GetComponent<EnemyStats>().DoDamage());

            if (Enemies.Length < 1)
            {
                // Get Coilidning Projectiles
                Collider2D[] Projectiles = Physics2D.OverlapBoxAll(transform.position, hitBox, 0, enemyLayers);
                // Projectiles Doin Damage
                foreach (Collider2D projectile in Projectiles)
                    stats.TakeDamage(projectile.GetComponent<ProjectileScript>().DoDamage());
            }
        }
    }
    private void PickUpCheck()
    {
        if (enablePiskup)
        {
            Collider2D[] obj = Physics2D.OverlapCircleAll(transform.position, interRange);
            foreach (var coll in obj)
                if (coll.TryGetComponent(out Interactable temp))
                    temp.AddToInventory();
        }
        else if (Time.time >= nextPickup)
            enablePiskup = true;
    }

    private void UpdateEquipment()
    {
        for (int i = 0; i < numE; i++)
        {
            Equipment e = inventory.Equiped(i);
            Armor a;
            bool b = false;

            if (e != null)
                switch (i)
                {
                    case 0:
                        a = (Armor)e;
                        if (a.texture[0] != helmet.sprite)
                        {
                            helmetTI = 0;
                            helmet.sprite = a.texture[helmetTI];
                            helmet.color = a.color;
                        }
                        break;
                    case 1:
                        a = (Armor)e;
                        if (a.texture[0] != torso.sprite)
                        {
                            torso.sprite = a.texture[0];
                            torso.color = a.color;
                        }
                        break;
                    case 2:
                    case 3:
                        if (!armed)
                            weaponJustEquiped = true;
                        armed = true;
                        break;
                    default:
                        break;
                }
            else
                switch (i)
                {
                    case 0:
                        helmet.sprite = empty;
                        break;
                    case 1:
                        torso.sprite = empty;
                        break;
                    case 2:
                        b = armed;
                        break;
                    case 3:
                        if(b)
                            armed = false;
                        break;
                    default:
                        break;
                }

            ChangeWeapon(primaryWeap, false);
        }
    }

    #region GetSet

    public void SetPos(Vector2 pos)                     { rb.position = pos;    }
    public Vector2 GetPos()                             { return rb.position;   }
    public void SetActiveCombat(bool set)               { playerCom.enabled = set; }

    #endregion

    public void ChangeWeapon(bool primar, bool overwrite)
    {
        int j;
        if (primar)
            j = 2;
        else
            j = 3;

        playerCom.EquipWeapon((Weapon)Inventory.instance.Equiped(j));

        primaryWeap = primar;

        if (overwrite)
            if (armed)
                playerCom.enabled = true;
    }
    public void DropedItem()
    {
        enablePiskup = false;
        nextPickup = Time.time + pickUpTime;
    }
    public void Resurect() 
    {
        stats.Resurect();
    }
}