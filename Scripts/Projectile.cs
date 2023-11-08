using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class ProjectileScript : MonoBehaviour
{
    public int damage;
    public float force;
    public float timeToDie;
    public float size;
    public LayerMask targetLayers;

    private Vector3 targetPos;
    private Vector2 velocity;
    private Rigidbody2D rb;

    private int targets;

    void Start()
    {
        //Debug.Log("Fired projectile name: " + name);

        // References
        string s = name.Split('-')[1];
        targetLayers &= ~(1 << LayerMask.NameToLayer(s));
        rb = GetComponent<Rigidbody2D>();
        if      (s == "Player")
        {
            /* Mozno inokedy
            Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            targetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            */
            targetPos = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(0).GetChild(1).position;
            targets = 1;
        }
        else // if (transform.name.Contains("Enemy")) 
        { 
            Transform playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            targetPos = playerTrans.position;
            targets = 0;
        }
        // Variables
        Vector3 direction = targetPos - transform.position;
        Vector3 rotation = transform.position - targetPos;
        velocity = new Vector2(direction.x, direction.y).normalized * force;
        rb.velocity = velocity;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        timeToDie = Time.time + timeToDie;
    }
    void Update()
    {
        if (Time.time >= timeToDie)
        {
            Destroy(transform.gameObject);
            Destroy(this);
        }
        else
        {
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, size, targetLayers);

            if (hitTargets.Length > 0)
            {
                foreach (Collider2D target in hitTargets)
                {
                    if (targets == 0)
                    {
                        target.GetComponent<PlayerScript>().TakeDamage(damage);
                        Destroy(gameObject);
                    }
                    else
                    {
                        target.GetComponent<EnemyScript>().TakeDamage(damage);
                        Destroy(gameObject);
                    }
                }
            }
            if (!rb.velocity.Equals(velocity))
                Destroy(gameObject);
        }
    }
    public int DoDamage() { return damage; }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, size);
    }
}
