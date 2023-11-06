using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.IK;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class PlayerScript : MonoBehaviour
{    
    public Sprite empty;
    public HealthBar healthBar;
    public LayerMask enemyLayers;
    public Vector2 hitBox;

    public float interRange;
    public int health;
    public int level;

    private PlayerCombatScript playerCom;
    private Rigidbody2D rb;
    private Animator animator;
    private Inventory inventory;
    private Sprite[] clothes = new Sprite[3];    // tri typy oblecena

    private Vector2 moveDir;

    private float moveSpeed = 5;

    private float nextDamage = 0;
    private float inviTime = 0.5f;

    private int maxHealth;
    private int numE;

    private bool combatAct = false;
    private bool armed = false;

    private void Start() 
    {
        // References
        playerCom = GetComponent<PlayerCombatScript>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        maxHealth = health;
        healthBar.SetMaxHealth(health);
        //transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        inventory = Inventory.instance;
        inventory.onItemChangeCallback += UpdateEquipment;
        numE = System.Enum.GetNames(typeof(EquipmentSlot)).Length;

        // Set up
        combatAct = playerCom.enabled;
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
    private void Update()
    {
        if (!GameManager.isPaused)
        {
            ProcessInput();
            Move();
            AnimateMovement();
            CollisionCheck();
        }
    }
    private void UpdateEquipment()
    {
        for (int i = 0; i < numE; i++)
        {
            Equipment e = inventory.Equiped(i);
            if (e != null)
                switch (i) 
                {
                    case 0:
                    case 1:
                    case 2:
                        if (e.texture != clothes[i])
                            clothes[i] = e.texture;
                        break;
                    case 3: 
                    case 4:
                        armed = true;
                        playerCom.EquipWeapon(e);
                        break;
                    default:
                        break;
                }
            else
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:                            
                        clothes[i] = empty;
                        break;
                    case 3:
                        armed = false;
                        playerCom.EquipWeapon(empty, i);
                        break;
                    default:
                        break;
                }
        }
    }
    private void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButton(0) && !combatAct && armed)
        {
            playerCom.enabled = true;
            combatAct = true;
        }
        else if ((Input.GetMouseButton(2) || !armed) && combatAct)
        {
            playerCom.enabled = false;
            combatAct = false;
        }          

        moveDir = new Vector2(moveX, moveY).normalized;
    }
    private void Move() 
    { 
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
    private void AnimateMovement()
    {
        animator.SetFloat("Horizontal", moveDir.x);
        animator.SetFloat("Vertical", moveDir.y);
        animator.SetFloat("Speed", moveDir.sqrMagnitude);
    }
    public void TakeDamage(int damage)                  { Hurt(damage);         }
    public void SetPos(Vector2 pos)                     { rb.position = pos;    }
    public void SetLevel(int newlevel)                  { level = newlevel;     }
    public void SetHealth(int newhealth)                {  health = newhealth;  }
    public int GetLevel()                               { return level;         }
    public int GetHealth()                              { return health;        }
    public Vector2 GetPos()                             { return rb.position;   }

    // DAMAGING \\
    private void CollisionCheck()
    {
        if (Time.time > nextDamage)
        {
            // Get Coilidning Enemies
            Collider2D[] Enemies = Physics2D.OverlapBoxAll(transform.position, hitBox, 0, enemyLayers);
            // Enemis Doin Damage
            foreach (Collider2D enemy in Enemies)
                Hurt(enemy.GetComponent<EnemyScript>().DoDamage());

            if (Enemies.Length < 1)
            {
                // Get Coilidning Projectiles
                Collider2D[] Projectiles = Physics2D.OverlapBoxAll(transform.position, hitBox, 0, enemyLayers);
                // Projectiles Doin Damage
                foreach (Collider2D projectile in Projectiles)
                    Hurt(projectile.GetComponent<ProjectileScript>().DoDamage());
            }

            Collider2D[] obj = Physics2D.OverlapCircleAll(transform.position, interRange);
            foreach (var coll in obj)
            {
                if (coll.TryGetComponent(out Interactable temp))
                {
                    temp.AddToInventory();
                    //Destroy(coll.gameObject);
                }
            }
        }
    }
    private void Hurt(int damage) 
    {
        if (damage != 0)
        {
            health -= damage;
            healthBar.SetHealth(health);
            animator.SetTrigger("Hurt");

            if (health <= 0)
                Die();

            // Invincibility
            nextDamage = Time.time + inviTime;
        }
    }
    private void Die()
    {
        rb.simulated = false;
        playerCom.enabled = false;
        combatAct = false;
        animator.SetBool("isAlive", false);
        //Debug.Log("Hrac zomrel");
        GameManager.playerLives = false;
    }
    public void Resurect() 
    { 
        health = maxHealth; 
        healthBar.SetHealth(health);
        //playerCom.enabled = true;
        rb.simulated = true;
        animator.SetBool("isAlive", true);
    }
}