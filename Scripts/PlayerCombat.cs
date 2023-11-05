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


    public int damage;

    private float attackDist = 45;  // distance for melee weapon to do damage
    public float fireRate;
    private float fireTime = 0;
    private float rotZ;
    private float lastRotZ = 0;

    private bool CombatActive;
    private bool melee;

    private void Start()
    {
        // References
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rotatePoint = transform.GetChild(0);
        Hand = rotatePoint.transform.GetChild(0);
        HandSecondary = rotatePoint.transform.GetChild(1);
        col = Hand.GetComponent<Collider2D>();

        melee = true;
    }
    private void Update()
    {
        if (!GameManager.isPaused)
        {
            if (Input.GetMouseButton(0))
            {
                if (!melee)
                    RangedAttack();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                WeaponSwap();
            }

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
        SpriteRenderer sprite = Hand.GetChild(0).GetComponent<SpriteRenderer>();

        if (equipment != null)
        {
            damage = equipment.damageModifier;
            sprite.sprite = equipment.texture;
        }
        else
        {
            damage = equipment.damageModifier;
            sprite.sprite = equipment.texture;
        }
    }
    private void OnEnable()
    { transform.GetChild(0).gameObject.SetActive(true);     }
    private void OnDisable()
    { transform.GetChild(0).gameObject.SetActive(false);    }
}
