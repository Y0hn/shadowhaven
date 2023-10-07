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
    public Rigidbody2D rigitBody;
    public Animator animator;
    public GameObject WeaponL;
    public GameObject WeaponR;
    public Transform attackPointL;
    public Transform attackPointR;
    public LayerMask enemyLayers;
    public float attackRange;
    public float attackRate;
    public int damage;
    public float animatorMoveSpeed = 0;

    private Vector2 moveDir;
    private Vector2 pastMoveDir;
    private int level;
    private int health;
    private bool atck = false;
    private bool goneUp;
    private bool goneRight;
    private bool twoHanded = false;
    private float moveSpeed = 5;
    private float nextAttackTime = 0;

    private void Start() { level = 1; health = 100 * level; }
    
    void Update()
    {
        if (!GameManager.isPaused)
        {
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
        rigitBody.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
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
        rigitBody.velocity = Vector2.zero;
        
        if (!twoHanded)
            if (0 < pastMoveDir.y)
            {
                WeaponL.SetActive(false);
                WeaponR.SetActive(true);
            }
        animator.SetFloat("Horizontal", pastMoveDir.x);
        animator.SetFloat("Vertical", pastMoveDir.y);
        animator.SetTrigger("Attack");
        List<Collider2D> hitEnemies = new List<Collider2D>();
        Collider2D[] hitEnemiesR = Physics2D.OverlapCircleAll(attackPointR.position, attackRange, enemyLayers);
        Collider2D[] hitEnemiesL = Physics2D.OverlapCircleAll(attackPointL.position, attackRange, enemyLayers);

        if (twoHanded)
        {
            hitEnemies.AddRange(hitEnemiesR);
            hitEnemies.AddRange(hitEnemiesL);
        }
        else
            if (0 == pastMoveDir.x && 0 < pastMoveDir.y)
                hitEnemies.AddRange(hitEnemiesR);
            else
                hitEnemies.AddRange(hitEnemiesL);

        foreach(Collider2D enemy in hitEnemies.ToArray())
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
        }

    }
    private void OnDrawGizmosSelected()
    {
        if (attackPointR != null)
            Gizmos.DrawWireSphere(attackPointR.position, attackRange);
        if (attackPointL != null)
            Gizmos.DrawWireSphere(attackPointL.position, attackRange);
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
    public void SetAttackRange(float newAttackRange) { attackRange = newAttackRange; }
    public float GetAttackRange() {  return attackRange; }
    #endregion
}