using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.IK;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public float moveSpeed;

    private int level;
    private int health = 100;
    private Vector2 moveDir;

    void Update()
    {
        ProcessInput();
    }
    void FixedUpdate()
    {
        if (health >= 0)
        {
            Move();
            Animate();
        }
        else
        {
            Debug.Log("Hrac zomrel");
            Destroy(this);
        }
    }
    private void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;
    }
    private void Move()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
    private void Animate()
    {
        animator.SetFloat("Horizontal", moveDir.x);
        animator.SetFloat("Vertical", moveDir.y);
        animator.SetFloat("speed", moveDir.sqrMagnitude);
    }

    public void SetHealth(int newHealth) { health = newHealth; }
    public int GetHealth() { return health; }
    public void SetLevel(int newLevel) { level = newLevel; }
    public int GetLevel() { return level; }
}