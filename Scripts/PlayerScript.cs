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
    private float attackRate;
    private float attackRange;

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
    /*
    void FixedUpdate()
    {
        if (health >= 0)
        {
            if (Time.time >= nextAttackTime && atck)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
            else if (Time.time >= nextAttackTime)
            {
            }
        }
        else
            Die();        
    }*/
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
    }/*
    private void EyeMovement()
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
    }/*
    private Vector2 MoveTovards(Vector2 pointA, Vector3 pointB, float distance)
    {
        Vector2 result = Vector2.zero;

        if (pointA.x < pointB.x)
            result.x = 1;
        else if (pointA.x > pointB.x)
            result.x = -1;
        if (pointA.y < pointB.y)
            result.y = 1;
        else if (pointA.y > pointB.y)
            result.y = -1;

        result = new Vector2(result.x * distance, result.y * distance);

        return result;
    }*/
    /*
    private void PastDirection()
    {
        double x = 0, y = 0;

        if (moveDir.x != 0)
            x = Math.Round(moveDir.x);
        else if (moveDir.y != 0)
            y = Math.Round(moveDir.y);

        if (x != y)
            pastMoveDir = new Vector2(float.Parse(x.ToString()), float.Parse(y.ToString()));
    }
    private void Attack()
    {
        animator.SetFloat("speed", 0);
        rigitbody.velocity = Vector2.zero;
        
        animator.SetFloat("Horizontal", pastMoveDir.x);
        animator.SetFloat("Vertical", pastMoveDir.y);
        animator.SetTrigger("Attack");
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
        }
       
    } not working */

    #region GetSetVar
    /*
    public void GetSet()
    {

    }
    public int GetSet(int id)
    {
        return 0;
    }*/
    public void SetDamage(int newDamage)                { damage = newDamage;           }
    public int GetDamage()                              { return damage;                }
    public void SetHealth(int newHealth)                { health = newHealth;           }
    public int GetHealth()                              { return health;                }
    public void SetLevel(int newLevel)                  { level = newLevel;             }
    public int GetLevel()                               { return level;                 }
    public void SetAttRate(int newattRate)              { attackRate = newattRate;      }
    public float GetAttRate()                           { return attackRate;            }
    public void SetSpeed(float newSpeed)                { moveSpeed = newSpeed;         }
    public float GetSpeed()                             { return moveSpeed;             }
    public void SetAttackRange(float newAttackRange)    { attackRange = newAttackRange; }
    public float GetAttackRange()                       { return attackRange;           }
    public void SetPos(Vector2 pos)                     { rb.position = pos;            }
    public Vector2 GetPos()                             { return rb.position;           }

    #endregion
}