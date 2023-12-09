using UnityEngine;

public class BossStats : EnemyStats
{
    private float activateBorderY = float.PositiveInfinity;
    private Transform target;
    private Transform heBar;
    private const float focusTime = 2.0f;
    private const float loadHealthTime = 2f;
    private float timer = 0f;
    private int fakeHealth;
    private bool barFilling;
    private bool entry;
    //private int bossType = 0;

    protected override void Start()
    {
        base.Start();
        stunable = false;
        heBar = healthBar.transform;
        // Zistenie typu miestnosti
        switch (RoomType())
        {
            case "20x20":
                transform.position = new Vector2(transform.position.x, transform.position.y + 10);
                break;
            default:
                // 10x10
                break;
        }
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Reset variable
        fakeHealth = 0;
        barFilling = false;
        entry = true;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(0);
    }
    private void Update()
    {
        if (entry)  // ENTRY
        {
            if      (barFilling)
            {
                if (fakeHealth < maxHealth)
                {
                    if (timer < Time.time)
                    {
                        fakeHealth++;
                        healthBar.SetHealth(fakeHealth);
                        timer = Time.time + loadHealthTime / maxHealth;
                    }
                }
                else
                {
                    barFilling = false;
                    entry = false;
                    timer = 0;
                }
            }
            else if (GameManager.instance.cameraFocused)
            {
                animator.SetTrigger("roar");
                fakeHealth = 0;
                barFilling = true;
            }
            else if (target.position.y >= activateBorderY && !animator.enabled)
            {
                heBar.gameObject.SetActive(true);
                GameManager.instance.MoveCameraTo(transform.position, focusTime);
                animator.enabled = true;
            }
        }
        else
        {
            healthBar.SetHealth(curHealth);
            animator.SetBool("follow", true);
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
        GameManager.instance.BossKilled();
        Destroy(gameObject, 3);
    }
    private string RoomType()
    {
        string[] s = transform.parent.name.Split(' ');

        if (s.Length > 1)
            return s[1];
        else
            return null;
    }
    public void SetY(float newY)
    {
        activateBorderY = newY;
    }
}
