using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerCombatScript : MonoBehaviour
{
    public GameObject Hand;
    public GameObject HandSecondary;
    public GameObject projectile;
    public LayerMask enemyLayers;
    public Sprite[] weapons;

    public float attackRange;

    private PlayerScript player;
    private Collider2D col;
    private Animator animator;
    private Camera cam;    


    private Vector2 mousePos;

    private float rotZ;
    private float lastRotZ;
    private float attackDist = 45;

    private int damage = 50;

    private bool CombatActive;
    //private bool fliped;
    private bool melee;
    private float fireTime;
    private float fireRate;

    private void Start()
    {
        // References
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        col = Hand.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        // Set Up
        HandSecondary.SetActive(false);
        Hand.SetActive(false);
        CombatActive = false;
        fireTime = 0;
        fireRate = 2;
        lastRotZ = 0;
        melee = true;
    }
    private void Update()
    {
        if (!GameManager.isPaused)
        {
            // Eneterin combat-mode
            if (Input.GetMouseButton(0))
            {
                if (!CombatActive)
                {
                    Hand.SetActive(true);
                    CombatActive = true;
                }
                else if (!melee)
                    RangedAttack();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                WeaponSwap();
            }
            else if (Input.GetMouseButton(2))
            {
                Hand.SetActive(false);
                CombatActive = false;
            }

            if (CombatActive)
            {
                mousePos = Input.mousePosition;
                mousePos = cam.ScreenToWorldPoint(mousePos);

                Vector2 rotation = mousePos - player.GetPos();
                rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, rotZ);

                if (melee)
                {
                    if (rotZ > lastRotZ + attackDist || lastRotZ - attackDist > rotZ)
                    {
                        MeleeAttack();
                        lastRotZ = rotZ;
                    }
                }                   
             }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 attP = new Vector2 (Hand.transform.position.x,Hand.transform.position.y);
        Gizmos.DrawWireSphere(attP, attackRange);
    }
    private void WeaponSwap()
    {
        int weap;

        if (melee)
            weap = 2;
        else
            weap = 1;

        Hand.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = weapons[weap];

        melee = !melee;
        col.enabled = melee;

        if (!CombatActive)
        {
            Hand.SetActive(true);
            CombatActive = true;
        }
    }
    private void MeleeAttack()
    {
        Vector2 attP = new Vector2(Hand.transform.position.x, Hand.transform.position.y);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attP, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
    }
    private void RangedAttack()
    {
        if (Time.time >= fireTime)
        {
            Instantiate(projectile, Hand.transform.position, Quaternion.identity);
            fireTime = Time.time + 1 / fireRate;
        }
    }
}
