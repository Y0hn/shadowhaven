using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject enemy;
    public Animator animator;
    public bool dataLoaded = false;
    public PlayerScript player;

    private int fullHealth;
    private int health;
    private int baseHealth;
    private Vector2 playerPos;
    private Vector2 attackRange;
    private Vector2 Dir = new Vector2(1,0);
    private float attackSpeed;
    private float moveSpeed;

    private void Start()
    {
        SetStats();
        animator = (Animator)enemy.GetComponent("Animator");
        //player = (PlayerScript)GameObject.Find("Player").GetComponent("Player Script");
    }
    private void Update()
    {
        playerPos = player.GetPos();
    }
    private void SetStats()
    {
        UnpackData();

        int playerLevel = player.GetLevel();
        int minHealth = int.Parse(Math.Round(playerLevel * 1.5 + baseHealth).ToShortString());
        int maxHealth = int.Parse(Math.Round(playerLevel * 1.6 + baseHealth * 1.5).ToShortString());
        fullHealth = UnityEngine.Random.Range(minHealth, maxHealth);
        health = fullHealth;

        dataLoaded = true;
    }
    private void UnpackData()
    {
        string[] data = EnemyData.GetData(enemy.name).Split(' ');
        baseHealth = int.Parse(data[0]);
        attackRange = new Vector2(float.Parse(data[1]), float.Parse(data[2]));
        attackSpeed = float.Parse(data[3]);
        moveSpeed = float.Parse(data[4]);
    }
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
        animator.SetBool("IsAlive", false);

        if (Dir.x > 0 || 0 < Dir.y)
            animator.SetFloat("Horizontal", -1);
        else
            animator.SetFloat("Horizontal", 1);

        enemy.GetComponent<Collider2D>().enabled = false;
        Destroy(this);
    }

    #region GetSet
    public float GetSpeed() { return moveSpeed; }
    public Vector2 GetAttackRange() { return attackRange; }
    public float GetAttackRate() { return attackSpeed; }
    public Vector2 GetPlayerPos() { return playerPos; }
    public void SetDir(Vector2 newdir) { Dir = newdir; }
    #endregion
}
