using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    private Animator animator;
    private Transform target;
    private EnemyStats stats;
    private Rigidbody2D rb;

    private string bossName;

    private float rangeMax;
    private float rangeMin;

    private void Start()
    {
        // References
        bossName = transform.name;
        animator = GetComponent<Animator>();
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if      (bossName.Contains("Zom"))
        {

        }
        else if (bossName.Contains("Blob"))
        {

        }
        // ...
    }
}
