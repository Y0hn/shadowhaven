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

    private int level = 1;
    private int health = 100;

    private void Start() 
    { 
        // References
        player = GameObject.FindGameObjectWithTag("Player");
        weapon = FindChildByTag(player, "Weapon");
        rb = player.GetComponent<Rigidbody2D>();
        animator = player.GetComponent<Animator>();
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

            if (Time.time > nextDamage)
            {
                // Get Coilidning Enemies
                Collider2D[] Enemies = Physics2D.OverlapBoxAll(transform.position, hitBox, 0, enemyLayers);
                // Enemis Doin Damage
                foreach (Collider2D enemy in Enemies)
                {
                    Debug.Log("We got hit by " + enemy.name);
                    Hurt(enemy.GetComponent<EnemyScript>().DoDamage());
                }
                // Invincibility
                nextDamage = Time.time + inviTime;
            }
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
        health -= damage;
        healthBar.SetHealth(health);
        animator.SetTrigger("Hurt");

        if (health < 0)
            Die();
    }
    private void Die()
    {
        rb.simulated = false;
        weapon.SetActive(false);
        animator.SetTrigger("Die");
        Debug.Log("Hrac zomrel");
        GameManager.playerLives = false;
        Destroy(this);
    }
    public void SetPos(Vector2 pos)                     { rb.position = pos;    }
    public void SetLevel(int newlevel)                  { level = newlevel;     }
    public void SetHealth(int newhealth)                {  health = newhealth;  }
    public int GetLevel()                               { return level;         }
    public int GetHealth()                              { return health;        }
    public Vector2 GetPos()                             { return rb.position;   }
}