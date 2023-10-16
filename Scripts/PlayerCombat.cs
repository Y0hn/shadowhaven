using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerCombatScript : MonoBehaviour
{
    public GameObject Hand;
    public GameObject HandSecondary;
    public LayerMask enemyLayers;

    public float attackRange;

    private PlayerScript player;
    private Animator animator;
    private Camera cam;    

    private Vector2 mousePos;

    private float rotZ;
    private float lastRotZ;
    private float nextAttack = 0;
    private float interAttack = 1f;
    private float attackDist = 45;

    private int damage;

    private bool CombatActive;
    private bool melee = true;

    private Collider2D hitBox;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        hitBox = Hand.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        HandSecondary.SetActive(false);
        Hand.SetActive(false);
        CombatActive = false;
    }
    private void Update()
    {
        // Record of degree in past 
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + interAttack;

            if (melee)
                lastRotZ = transform.rotation.z;
        }

        if      (Input.GetMouseButton(0))
        {
            if (!CombatActive)
            {
                Hand.SetActive(true);
                CombatActive=true;
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (!CombatActive)
            {
                Hand.SetActive(true);
                CombatActive=true;
            }
        }
        else if (Input.GetMouseButton(2)) 
        { 
            Hand.SetActive(false);
            CombatActive=false;
        }

        if (CombatActive)
        {
            mousePos = Input.mousePosition;
            mousePos = cam.ScreenToWorldPoint(mousePos);

            Vector2 rotation = mousePos - player.GetPos();
            rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ);

            if (transform.rotation.z > lastRotZ + attackDist || lastRotZ - attackDist > transform.rotation.z)
                MeleeAttack();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 attP = Hand.transform.position;
        Gizmos.DrawWireSphere(attP, attackRange);
    }
    private void MeleeAttack()
    {
        Vector2 attP = Hand.transform.position;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attP, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
        }
    }
}
