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
    public LayerMask enemyLayers;

    public float attackRange;

    private PlayerScript player;
    private Collider2D hitBox;
    private Animator animator;
    private Camera cam;    


    private Vector2 mousePos;

    private float rotZ;
    private float lastRotZ;
    private float attackDist = 45;
    private float attackOfst = 0.7f;

    private int damage = 20;

    private bool CombatActive;
    private bool melee = true;

    private void Start()
    {
        // References
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
            }
            else if (Input.GetMouseButton(1))
            {
                if (!CombatActive)
                {
                    Hand.SetActive(true);
                    CombatActive = true;
                }
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

                if (melee && (rotZ > lastRotZ + attackDist || lastRotZ - attackDist > rotZ))
                {
                    MeleeAttack();
                    lastRotZ = rotZ;
                }
                /*
                if (-90 < rotZ && rotZ < 90)
                    Hand.transform.rotation = Quaternion.Euler(0, 0, -180);
                else
                    Hand.transform.rotation = Quaternion.Euler(0, 0, 0);
                */
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 attP = new Vector2 (Hand.transform.position.x,Hand.transform.position.y + attackOfst);
        Gizmos.DrawWireSphere(attP, attackRange);
    }
    private void MeleeAttack()
    {
        Vector2 attP = new Vector2(Hand.transform.position.x, Hand.transform.position.y + attackOfst);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attP, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
        }
    }
}
