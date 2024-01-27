using System.Collections.Generic;
using UnityEngine;

public class BossStats : EnemyStats
{
    public int numberOfAttacks;
    private float activateBorderY = float.PositiveInfinity;
    private Transform target;
    private Transform heBar;
    private float timer;
    private const float focusTime = 2.0f;
    private const float loadHealthTime = 2f;
    private List<int> behavior;
    private int fakeHealth;
    private bool barFilling;
    private bool entry;
    //private int bossType = 0;

    protected override void Start()
    {
        base.Start();
        stunable = false;
        heBar = healthBar.transform;
        healthBar.transform.parent.gameObject.SetActive(true);
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Reset variable
        timer = 0f;
        entry = true;
        fakeHealth = 0;
        behavior = new();
        barFilling = false;
        healthBar.SetMax(maxHealth);
        healthBar.Set(0);
    }
    private void Update()
    {
        // 1 => ENTRY
        if (entry)
        {
            // 1.2
            if (animator.enabled)
            {
                // 1.2.1
                if (barFilling)
                {
                    // 1.2.1.1
                    if (fakeHealth < maxHealth)
                    {
                        // If Skiped
                        if (!GameManager.instance.GetMovingCam())
                        {
                            healthBar.Set(maxHealth);
                            animator.SetBool("move", true);
                            entry = false;
                            timer = 0;
                        }
                        // Default path
                        else if (timer < Time.time)
                        {
                            fakeHealth++;
                            healthBar.Set(fakeHealth);
                            timer = Time.time + loadHealthTime / maxHealth;
                        }
                    }
                    // 1.2.1.1.2 => LAST
                    else
                    {
                        // Wait for camera to focus
                        if (GameManager.instance.cameraFocused)
                        {
                            animator.SetBool("move", true);
                            barFilling = false;
                            entry = false;
                            timer = 0;
                        }
                    }
                }
                // 1.2.2
                else if (GameManager.instance.cameraFocused)
                {
                    animator.SetTrigger("intro");
                    fakeHealth = 0;
                    barFilling = true;
                }
            }
            // 1.1
            else if (target.position.y >= activateBorderY)
            {
                heBar.gameObject.SetActive(true);
                GameManager.instance.MoveCameraTo(transform.position, focusTime);
                animator.SetBool("move", false);
                animator.enabled = true;
            }
        }
        // 2 => health refresh
        else
        {
            healthBar.Set(curHealth);
            /* Behavior
            if (behavior.Count > 1)
            {
                if (Time.time > timer)
                {
                    AnimateMovement(behavior[1]);
                    Debug.Log($"Attacking with attack {behavior[1]}");
                    if (behavior.Count < 2)
                        behavior[0] = behavior[1];
                    behavior.RemoveAt(1);
                }
            }
            else
            {
                GenerateMovement();
            }
            */
        }
    }
    private void GenerateMovement()
    {
        int n = numberOfAttacks + 1;
        int rand;
        int last = behavior[0];
        behavior = new();

        // Aby neutocil tym cim skoncil
        do rand = Random.Range(0, n);
        while (behavior.Contains(last));
        behavior.Add(rand);

        for (int i = 0; i < n; i++)
        {
            do rand = Random.Range(0, n);
            while (behavior.Contains(rand));
            behavior.Add(rand);
        }
    }
    private void AnimateMovement(int moveId)
    {
        switch (moveId) 
        { 
            case 0:     // Follow
                animator.SetBool("move", true);
                timer = Random.Range(100, 500);
                break;
            default:    // Attack with id
                animator.SetBool("move", false);
                animator.SetTrigger("attack" + moveId);
                timer = Random.Range(100, 200);
                rb.velocity = Vector3.zero;
                animator.ResetTrigger("attack" + moveId);
                break;
        }
        timer = Time.time + timer/100;
    }
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
    }
    protected override void Die()
    {
        // Die animation
        animator.SetTrigger("die");
        collid.enabled = false;
        rb.simulated = false;
        GameManager.instance.BossKilled();
        Destroy(gameObject, 3);
    }
    public void SetY(float newY)
    {
        activateBorderY = newY;
    }
}
