using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.IK;

public class PlayerScript : MonoBehaviour
{    
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject WeaponL;
    public GameObject WeaponR;
    public Transform attackPointL;
    public Transform attackPointR;
    public LayerMask enemyLayers;
    public float attackRange;
    public float attackRate;
    public int damage;
    public float moveSpeed;

    private int level;
    private int health;
    private Vector2 moveDir;
    private bool goesRight;
    private bool goesUp;
    private bool atck;
    private float nextAttackTime = 0;
    private void Start() { health = 100; }

    void Update()
    {
        if (!GameManager.isPaused)
        {
            if (atck)
                atck = false;
            ProcessInput();
        }
    }
    void FixedUpdate()
    {
        if (health >= 0)
        {
            if (Time.time >= nextAttackTime)
                if (atck)
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            Move();
            Animate();
        }
        else
            Die();
    }
    private void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Space))
            atck = true;
        if (moveY != 0)
        {
            goesUp = moveY > 0;
            goesRight = false;
        }
        if (moveX != 0)
            goesRight = moveX > 0;
        moveDir = new Vector2(moveX, moveY).normalized;
    }
    private void Move()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
    private void Animate()
    {
        animator.SetFloat("Horizontal", moveDir.x);
        animator.SetFloat("Vertical", moveDir.y);
        animator.SetFloat("speed", moveDir.sqrMagnitude);

        WeaponL.SetActive(!goesRight);
        WeaponR.SetActive(goesRight);
    }
    private void Attack()
    {
        animator.SetTrigger("Attack");

        Vector2 attackPoint;
        if (!goesRight)
            attackPoint = attackPointL.position;
        else
            attackPoint = attackPointR.position;


        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
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
    private void Die()
    {
        // Death animation
        Debug.Log("Hrac zomrel");
        Destroy(this);
    }

    #region GetSet
    public void SetHealth(int newHealth) { health = newHealth; }
    public int GetHealth() { return health; }
    public void SetLevel(int newLevel) { level = newLevel; }
    public int GetLevel() { return level; }

    #endregion
}