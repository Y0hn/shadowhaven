using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.IK;
using UnityEngine.XR;

public class PlayerScript : MonoBehaviour
{    
    public HealthBar healthBar;
    public LayerMask enemyLayers;
    public Vector2 hitBox;

    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;
    private GameObject weapon;

    private Vector2 moveDir;

    private float moveSpeed = 5;

    private float nextDamage = 0;
    private float inviTime = 0.5f;

    public int health = 100;
    private int maxHealth;
    public int level = 1;

    private void Start() 
    {
        // References
        player = GameObject.FindGameObjectWithTag("Player");
        weapon = FindChildByTag(player, "Weapon");
        rb = player.GetComponent<Rigidbody2D>();
        animator = player.GetComponent<Animator>();

        maxHealth = health;
        healthBar.SetMaxHealth(health);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(hitBox.x, hitBox.y, 0));
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
    private void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

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
        }
    }
    private GameObject FindChildByTag(GameObject parent, string tag)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).tag == tag)
                return parent.transform.GetChild(i).gameObject;

            GameObject tmp = FindChildByTag(parent.transform.GetChild(i).gameObject, tag);

            if (tmp != null)
                return tmp;
        }
        return null;
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
        weapon.SetActive(false);
        animator.SetBool("isAlive", false);
        //Debug.Log("Hrac zomrel");
        GameManager.playerLives = false;
    }
    public void TakeDamage(int damage)                  { Hurt(damage);         }
    public void SetPos(Vector2 pos)                     { rb.position = pos;    }
    public void SetLevel(int newlevel)                  { level = newlevel;     }
    public void SetHealth(int newhealth)                {  health = newhealth;  }
    public int GetLevel()                               { return level;         }
    public int GetHealth()                              { return health;        }
    public Vector2 GetPos()                             { return rb.position;   }
    public void Resurect() 
    { 
        health = maxHealth; 
        healthBar.SetHealth(health);
        weapon.SetActive(true);
        rb.simulated = true;
        animator.SetBool("isAlive", true);
    }
}