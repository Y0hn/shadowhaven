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
    private bool start;
    //private int bossType = 0;

    protected override void Start()
    {
        base.Start();
        start = true;
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
        // 0 => START
        if (start)
        {
            GameManager.instance.AddBoss(this);
            start = !GameManager.generated;
        }
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
                            AudioManager.instance.PlayTheme("stop");
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
                // Play Sound
                AudioManager.instance.PlayTheme("boss-intro");
                heBar.gameObject.SetActive(true);
                GameManager.instance.BossMoveCamera(transform.position, focusTime);
                GameManager.instance.SetDoorType(DoorType.BossIn, false);
                animator.SetBool("move", false);
                animator.enabled = true;
            }
        }
        // 2 => health refresh
        else
        {
            healthBar.Set(curHealth);
        }
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
        GameManager.instance.BossKilled(this);
        Destroy(gameObject, 3);
    }
    public void SetY(float newY)
    {
        activateBorderY = newY;
    }
    public void ShowBar(bool state)
    {
        healthBar.transform.parent.gameObject.SetActive(state);
    }
    private void OnDestroy()
    {
        GameManager.instance.BossKilled(this);
    }
}
