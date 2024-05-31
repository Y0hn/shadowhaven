using System.Collections.Generic;
using UnityEngine;

public class BossStats : EnemyStats
{
    public bool onCamera { get; set; }
    public int numberOfAttacks;
    private float activateBorderY = float.PositiveInfinity;
    private Transform target;
    private float timer;
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
        healthBar.gameObject.SetActive(true);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        // Reset variable
        timer = 0f;
        entry = true;
        fakeHealth = 0;
        behavior = new();
        onCamera = false;
        barFilling = false;
        healthBar.SetMax(maxHealth);
        healthBar.Set(0);
        ShowBar(false);
        Debug.Log($"Boss [{name}] started");
    }
    private void Update()
    {
        // 0 => START
        if (start)
        {
            GameManager.instance.SetBoss(this);
            start = !GameManager.instance.generated;
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
                        if (timer < Time.time)
                        {
                            fakeHealth++;
                            healthBar.Set(fakeHealth);
                            timer = Time.time + loadHealthTime / maxHealth;
                        }
                    }
                    // 1.2.1.1.2 => LAST
                    else
                    {
                        // Wait for camera to focus on Boss
                        if (!onCamera)
                        {
                            barFilling = false;
                            LastInisiate();
                        }
                    }
                }
                // 1.2.2
                else if (onCamera)
                {
                    GameManager.audio.PlayTheme("stop");
                    animator.SetTrigger("intro");
                    barFilling = true;
                    fakeHealth = 0;
                    ShowBar(true);
                }

                // SKIPED
                if (GameManager.camera.IsCameraFocused("player"))
                {
                    LastInisiate();
                    ShowBar(true);
                }
            }
            // 1.1
            else if (target.position.y >= activateBorderY)
            {
                // Play Sound
                GameManager.audio.PlayTheme("boss-intro");
                GameManager.camera.CameraSequence("boss");
                GameManager.instance.DisEnableAllMobs(false);
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
    private void LastInisiate()
    {
        GameManager.audio.PlayTheme("boss-theme_" + name);
        GameManager.instance.DisEnableAllMobs(true);
        animator.SetBool("move", true);
        healthBar.SetMax(maxHealth);
        healthBar.Set(maxHealth);
        entry = false;
        timer = 0;
    }
    public override void TakeDamage(int dmg)
    {
        if (!entry)
            base.TakeDamage(dmg);
        Debug.Log($"Boss taken damage [{curHealth}/{maxHealth}]");
    }
    protected override void Die()
    {
        // Die animation
        animator.SetTrigger("die");
        collid.enabled = false;
        rb.simulated = false;
        GameManager.instance.BossKilled();
        GameManager.audio.PlayTheme("stop");
        Destroy(gameObject, 3);
    }
    public void SetY(float newY)
    {
        activateBorderY = newY;
    }
    public bool Active()
    {
        return !entry;
    }
    public void ShowBar(bool state)
    {
        healthBar.transform.parent.gameObject.SetActive(state);
    }
    private void OnDestroy()
    {
        GameManager.instance.BossKilled(true);
    }
    public void Uninstall() { Die(); }
}
