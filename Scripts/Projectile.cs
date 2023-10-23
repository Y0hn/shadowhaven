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
    public LayerMask enemy;

    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;

    void Start()
    {
        // References
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        // Variables
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.velocity = new Vector2 (direction.x, direction.y).normalized * force;
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
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, size, enemy);
            foreach (Collider2D ene in hitEnemies)
            {
                ene.GetComponent<EnemyScript>().TakeDamage(damage);
                Debug.Log("Enemy hit");
            }
            if (hitEnemies.Length > 0)
            {
                // Play animation (optional)
                Destroy(transform.gameObject);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, size);
    }
}
