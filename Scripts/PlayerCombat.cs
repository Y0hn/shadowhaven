using System;
using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    public LayerMask enemyLayers;
    public GameObject projectile;

    private Transform rotatePoint;
    private Transform Hand;
    private Transform HandS;
    private Sprite empty;

    public float attackRange;

    private PlayerScript player;
    private Collider2D col;
    private Camera cam;

    private Vector3 mousePos;


    public int damage;
    private const int posun = 3;    // ak sa v buducosti bude menit tak bude problem

    private float attackDist = 45;  // distance for melee weapon to do damage
    public float fireRate;
    private float fireTime = 0;
    private float rotZ;
    private float lastRotZ = 0;

    private bool melee;
    
    private void Start()
    {
        // References
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rotatePoint = transform.GetChild(0);
        Hand = rotatePoint.transform.GetChild(0);
        HandS = rotatePoint.transform.GetChild(1);
        col = Hand.GetComponent<Collider2D>();
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

            Vector2 rotation = mousePos - transform.position;
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
        // Change weapons
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
    public void EquipWeapon(Equipment weap)
    {
        // References
        Hand = transform.GetChild(0).GetChild(0);
        HandS = transform.GetChild(0).GetChild(1);
        SpriteRenderer Weapon = Hand.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer WeaponS = HandS.GetChild(0).GetComponent<SpriteRenderer>();
        col = Hand.GetComponent<Collider2D>();

        int index = (int)weap.equipSlot - posun;
        int type = (int)weap.type;

        switch (index)
        {
            case 0:
                if (weap.texture != Weapon.sprite)
                    Weapon.sprite = weap.texture;
                damage = weap.damageModifier;
                switch (type)
                {
                    case 0: melee = true; break;  // Melee
                    case 1: 
                    case 2: melee = false; break;  // Magic + Ranged
                }
                col.enabled = melee;
                break;
            case 1:
                Debug.Log("Equipnuta secondary zbran: " + weap.name);
                if (weap.texture != WeaponS.sprite)
                    WeaponS.sprite = weap.texture;
                // nejako modifinut damage alebo neco take
                break;
            default:
                break;
        }
    }
    public void EquipWeapon(Sprite s, int i)
    {
        i -= posun;
        switch (i) 
        { 
            case 0: transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = s; break;
            case 1: transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = s; break;
        }
        
    }
    private void OnEnable()
    { transform.GetChild(0).gameObject.SetActive(true);     }
    private void OnDisable()
    { transform.GetChild(0).gameObject.SetActive(false);    }

}
