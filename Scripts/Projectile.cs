using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void Start()
    {
        // References
        rb = GetComponent<Rigidbody2D>();
        if      (transform.name.Contains("Player"))
        {
            Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            targetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (transform.name.Contains("Enemy")) 
        { 
            Transform playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            targetPos = playerTrans.position;
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
                if      (transform.name.Contains("Enemy"))
                    hitTargets[0].GetComponent<PlayerScript>().TakeDamage(damage);
                else if (transform.name.Contains("Player"))
                    foreach (Collider2D target in hitTargets)
                        target.GetComponent<EnemyScript>().TakeDamage(damage);
                Destroy(transform.gameObject);
            }
            else if (!rb.velocity.Equals(velocity))
                Destroy(transform.gameObject);
        }
    }
    public int DoDamage() { return damage; }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, size);
    }
}
