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

    public LayerMask enemyLayers;
    public GameObject EyeR;
    public GameObject EyeL;

    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveDir;
    private Vector3 mousePos;
    private Vector2 handsDir;

    private float moveSpeed = 5;

    private int level;
    private int health;     
    private int damage;

    private void Start() 
    { 
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody2D>();
        animator = player.GetComponent<Animator>();
        level = 1; health = 100 * level; damage = 20; 
    }
    
    private void Update()
    {
        if (!GameManager.isPaused)
        {
            ProcessInput();
            Move();
            AnimateMovement();
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
    /*private void EyeMovement()
    {
        Vector2 pos = new Vector2(rb.position.x, rb.position.y + 0.7f);

        if (true)
        {
            Vector2 EyeLmove = MoveTovards(EyeL.transform.position, mousePos, 0.001f);
            float x = EyeL.transform.position.x;
            float y = EyeL.transform.position.y;

            if (x - 0.07 < EyeLmove.x && EyeLmove.x < x + 0.07)
                if (y - 0.07 < EyeLmove.x && EyeLmove.x < x + 0.7)
                    EyeL.transform.position = EyeLmove;

            EyeL.transform.position = new Vector3(EyeLmove.x, EyeLmove.y, -0.1f);
            attackPoint = EyeLmove;
        }
        if (true)
        {
            Vector2 EyeRmove = Vector2.MoveTowards(EyeR.transform.position, mousePos, 1 * Time.deltaTime);

            if (pos.x + 0.37 < EyeRmove.x && EyeRmove.x < pos.x + 0.23)
                if (pos.y - 0.07 < EyeRmove.x && EyeRmove.x < pos.x + 0.7)
                    EyeR.transform.position = EyeRmove;
        }
    }*/

    public void Hurt(int damage) 
    { 
        health -= damage;
        animator.SetTrigger("Hurt");

        if (health < 0)
            Die();
    }
    private void Die()
    {
        rb.simulated = false;
        animator.SetTrigger("Die");
        Debug.Log("Hrac zomrel");
        Destroy(this);
    }
    #region GetSetVar
    public void SetDamage(int newDamage)                { damage = newDamage;           }
    public int GetDamage()                              { return damage;                }
    public void SetHealth(int newHealth)                { health = newHealth;           }
    public int GetHealth()                              { return health;                }
    public void SetLevel(int newLevel)                  { level = newLevel;             }
    public int GetLevel()                               { return level;                 }
    public void SetSpeed(float newSpeed)                { moveSpeed = newSpeed;         }
    public float GetSpeed()                             { return moveSpeed;             }
    public void SetPos(Vector2 pos)                     { rb.position = pos;            }
    public Vector2 GetPos()                             { return rb.position;           }

    #endregion
}