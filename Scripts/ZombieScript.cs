using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieScript : MonoBehaviour
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

    void Start()
    {
        HealWounds(body);
        RandomDamage();
    }
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
            FollowBegavior();
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
        else if (playerPos.x > rb.position.x + attackRangeX -0.5)
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
    private void RandomDamage()
    {
        int damaged = UnityEngine.Random.Range(0, 6);
        List<int> used = new List<int>();

        for (int i = 0; i < damaged; i++)
        {
            int hurt;
            do hurt = UnityEngine.Random.Range(0, 6);
            while (used.Contains(hurt));
            used.Add(hurt);

            hurt = hurt * 10 + UnityEngine.Random.Range(1, 3);

            FindChildByName(body, NumberToName(hurt)).SetActive(true);
        }
    }
    private string NumberToName(int i)
    {
        string name = "";

        switch (i / 10)
        {
            case 0: name = "head";   break;
            case 1: name = "torso";  break;
            case 2: name = "armL";   break;
            case 3: name = "armR";   break;
            case 4: name = "legL";   break;
            case 5: name = "legR";   break;
        }

        switch (i % 10)
        {
            case 1: name = name + "-damage1"; break;
            case 2: name = name + "-damage2"; break;
            default: break;
        }

        return name;
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
    private void HealWounds(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            int j = i * 10;
            HealWound(ref parent, NumberToName(j + 1));
            HealWound(ref parent, NumberToName(j + 2));
        }

    }
    private void HealWound(ref GameObject parent, string name)
    {
        FindChildByName(parent, name).SetActive(false);
    }
}
