using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.IK;

public class PlayerScript : MonoBehaviour
{    
    public Rigidbody2D rigitbody;
    public Animator animator;
    public GameObject WeaponL;
    public GameObject WeaponR;
    public LayerMask enemyLayers;
    public float attackRange;
    public float attackRate;
    public int damage;
    public float animatorMoveSpeed = 0;

    private Vector2 moveDir;
    private Vector2 pastMoveDir;
    private Vector2 attackPoint;
    private int level;
    private int health;
    private bool atck = false;
    private bool goneUp;
    private bool goneRight;
    private bool twoHanded = false;
    private float moveSpeed = 5;
    private float nextAttackTime = 0;

    private void Start() { level = 1; health = 100 * level; pastMoveDir = new Vector2(1,0); }
    
    void Update()
    {
        if (!GameManager.isPaused)
        {
            attackPoint = new Vector2(rigitbody.position.x + pastMoveDir.x, rigitbody.position.y + pastMoveDir.y);
            ProcessInput();
            PastDirection();
        }
    }
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
                Move();
                AnimateMovement();
            }
        }
        else
            Die();
    }
    private void ProcessInput()
    {
        atck = false;
        if (Input.GetKey(KeyCode.Space))
            atck = true;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;
    }
    private void Move()
    {
        rigitbody.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
    private void AnimateMovement()
    {
        if (!atck)
        {
            animator.SetFloat("Horizontal", moveDir.x);
            animator.SetFloat("Vertical", moveDir.y);
            animator.SetFloat("speed", moveDir.sqrMagnitude);
        }

        if (!twoHanded)
        {
            if (0 == moveDir.x && 0 < moveDir.y)
            {
                WeaponL.SetActive(false);
                WeaponR.SetActive(true);
            }
            else
            {
                WeaponL.SetActive(true);
                WeaponR.SetActive(false);
            }
        }
    }
    private void Attack()
    {
        animator.SetFloat("speed", 0);
        rigitbody.velocity = Vector2.zero;
        
        if (!twoHanded)
            if (0 < pastMoveDir.y)
            {
                WeaponL.SetActive(false);
                WeaponR.SetActive(true);
            }
        animator.SetFloat("Horizontal", pastMoveDir.x);
        animator.SetFloat("Vertical", pastMoveDir.y);
        animator.SetTrigger("Attack");
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint, attackRange);
    }
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
    private void Die()
    {
        // Death animation
        Debug.Log("Hrac zomrel");
        Destroy(this);
    }
    #region GetSetVariables
    public void SetHealth(int newHealth) { health = newHealth; }
    public int GetHealth() { return health; }
    public void SetLevel(int newLevel) { level = newLevel; }
    public int GetLevel() { return level; }
    public void SetPos(Vector2 pos) { rigitbody.position = pos; }
    public Vector2 GetPos() {  return rigitbody.position; }
    public void SetAttackRange(float newAttackRange) { attackRange = newAttackRange; }
    public float GetAttackRange() {  return attackRange; }
    #endregion
}