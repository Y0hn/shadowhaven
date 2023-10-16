using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyScript : MonoBehaviour
{
    public GameObject enemy;
    public float lookRadius;
    public HealthBar healthBar;

    private Animator animator;
    private GameObject player;
    private Transform target;
    private PlayerScript playerScript;
    private Vector2 attackRange;
    private Vector2 hitBox;
    private int fullHealth;
    private int health;

    private void Start()
    {
        // References
        animator = GetComponent<Animator>();
        player = PlayerManager.instance.player;
        target = player.transform;
        playerScript = player.GetComponent<PlayerScript>();      
        
        // Set Stats
        /* UnpackData(out int baseHealth);
        int playerLevel = playerScript.GetLevel();
        int minHealth = int.Parse(Math.Round(playerLevel * 1.5 + baseHealth).ToShortString());
        int maxHealth = int.Parse(Math.Round(playerLevel * 1.6 + baseHealth * 1.5).ToShortString());
        fullHealth = UnityEngine.Random.Range(minHealth, maxHealth);
        health = fullHealth;
        */
    }
    private void Update()
    {
        float distance = Vector2.Distance(target.position, transform.position);

        if      (distance < lookRadius)
            animator.SetBool("isFollowing", true);
        else if (animator.GetBool("isFollowing"))
            animator.SetBool("isFollowing", false);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }/*
    private void UnpackData(out int baseHealth )
    {
        
        string[] data = EnemyData.GetData(enemy.name).Split(' ');
        baseHealth = int.Parse(data[0]);
        attackRange = new Vector2(float.Parse(data[1]), float.Parse(data[2]));
        attackSpeed = float.Parse(data[3]);
        moveSpeed = float.Parse(data[4]);
        
    }*/
    
    public void TakeDamage(int damage)
    {
        health -= damage;

        animator.SetTrigger("Hurt");

        // Play animation
        if (health < 0)
            Die();
    }
    private void Die() 
    { 
        Debug.Log("Enemy died!");
        animator.SetTrigger("Die");
        Destroy(this);
    }
}
/*public class ZombieScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public EnemyScript enemy;
    public GameObject body;

    private Vector2 dir;
    private Vector2 playerPos;
    private float nextAttack = 0;
    private float attackRangeX;
    private float attackRangeY;
    private float attackRate;
    private float speed = 0;
    private void SetUp()
    {
        speed = enemy.GetSpeed();
        attackRate = enemy.GetAttackRate();
        Vector2 temp = enemy.GetAttackRange();
        attackRangeX = temp.x;
        attackRangeY = temp.y;
    }
    private void FixedUpdate()
    {
        if (Time.time < 0.5)
            if (enemy.dataLoaded)
                SetUp();
        if (animator.GetBool("IsAlive"))
        {
            Animate();
            enemy.SetDir(dir);
        }
        else
        {
            rb.velocity = Vector2.zero;
            Destroy(this);
        }
    }
    private void OnDrawGizmosSelected()
    { Gizmos.DrawWireCube(rb.position, new Vector3(attackRangeX, attackRangeY, 0)); }
    private void FollowBegavior() 
    {
        playerPos = enemy.GetPlayerPos();
        if (dir.x != 0 && Vector2.Distance(playerPos, rb.position) <= attackRangeX)
            Attack();
        else if (dir.y != 0 && Vector2.Distance(playerPos, rb.position) <= attackRangeY)
            Attack();
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 1);
            rb.position = Vector2.MoveTowards(rb.position, playerPos, speed * Time.deltaTime);
        }
    }
    private void Attack()
    {
        if (Time.time > nextAttack)
        {
            animator.SetFloat("Speed", 0);
            animator.SetTrigger("Attack");
            nextAttack = Time.time + 1 / attackRate;
        }
    }
    private void Animate()
    {
        if (playerPos.x < rb.position.x - attackRangeX + 0.5)
            dir = new Vector2(-1, 0);
        else if (playerPos.x > rb.position.x + attackRangeX - 0.5)
            dir = new Vector2(1, 0);
        else if (playerPos.y < rb.position.y)
            dir = new Vector2(0, -1);
        else if (playerPos.y > rb.position.y)
            dir = new Vector2(0, 1);
        else
            animator.SetFloat("Speed", 0);

        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }
    private GameObject FindChildByName(GameObject parent, string childName)
    {
        for (int i = 0; i < parent.transform.childCount; i++) 
        { 
            if (parent.transform.GetChild(i).name == childName)
                return parent.transform.GetChild(i).gameObject;

            GameObject tmp = FindChildByName(parent.transform.GetChild(i).gameObject, childName);

            if (tmp != null)
                return tmp;
        }

        return null;
    }
}*/