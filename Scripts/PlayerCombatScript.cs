using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerCombatScript : MonoBehaviour
{
    public GameObject Hand;
    public GameObject HandSecondary;
    public LayerMask enemyLayers;

    private PlayerScript player;
    private Animator animator;
    private Camera cam;    
    private Vector3 attackRange;
    private Vector2 mousePos;
    private float rotZ;
    private int damage;
    private bool CombatActive;

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
        }
    }/*
    private void OnDrawGizmosSelected()
    {
        Vector2 attP = Hand.transform.position;

        Gizmos.DrawWireCube(new Vector2(attP.x, attP.y + attackRange.z), new Vector2(attackRange.x, attackRange.y));
    }
    private void Attack()
    {
        Vector2 attP = Hand.transform.position;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(new Vector2(attP.x, attP.y + attackRange.z), 
            new Vector2(attackRange.x, attackRange.y), rotZ, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
        }

    }*/
}
