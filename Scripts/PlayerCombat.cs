using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerCombatScript : MonoBehaviour
{
    public LayerMask enemyLayers;
    public GameObject projectile;

    private Transform rotatePoint;
    private Transform Hand;
    private Transform HandSecondary;

    public float attackRange;

    private PlayerScript player;
    private Collider2D col;
    private Camera cam;

    private Vector2 mousePos;

    private float rotZ;
    private float lastRotZ;
    private float attackDist;

    private int damage;

    private bool CombatActive;
    private bool melee;
    private float fireTime;
    private float fireRate;

    private void Start()
    {
        // References
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rotatePoint = transform.GetChild(0);
        Hand = rotatePoint.transform.GetChild(0);
        HandSecondary = rotatePoint.transform.GetChild(1);
        col = Hand.GetComponent<Collider2D>();
        // Set Up
        HandSecondary.gameObject.SetActive(false);
        Hand.gameObject.SetActive(false);
        CombatActive = false;
        attackDist = 45;
        damage = 30;
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
                    Hand.gameObject.SetActive(true);
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
                Hand.gameObject.SetActive(false);
                CombatActive = false;
                //Interaction();
            }

            if (CombatActive)
            {
                mousePos = Input.mousePosition;
                mousePos = cam.ScreenToWorldPoint(mousePos);

                Vector2 rotation = mousePos - player.GetPos();
                rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                rotatePoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);

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
        /*
        Vector2 attP = new Vector2 (Hand.position.x,Hand.position.y);
        Gizmos.DrawWireSphere(attP, attackRange);
        */
    }
    private void WeaponSwap()
    {
        //Hand.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = weapons[weap];

        melee = !melee;
        col.enabled = melee;

        if (!CombatActive)
        {
            Hand.gameObject.SetActive(true);
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
    public void EquipWeapon(Equipment equipment)
    {
        if (equipment != null)
        {
            damage = equipment.damageModifier;
            Hand.GetChild(0).GetComponent<SpriteRenderer>().sprite = equipment.texture;
        }
        else
        {
            damage = equipment.damageModifier;
            Hand.GetChild(0).GetComponent<SpriteRenderer>().sprite = equipment.texture;
        }
    }
}
