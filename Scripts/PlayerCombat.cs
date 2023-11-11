using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    public LayerMask enemyLayers;

    private Transform rotatePoint;
    private Transform Hand;
    private Transform HandS;

    public float attackRange;

    private PlayerScript player;
    private Collider2D col;
    private Camera cam;
    private GameObject projectile = null;
    
    private Vector3 mousePos;

    public int damage;
    private const int posun = 3;    // ak sa v buducosti bude menit tak bude problem

    private float attackDist = 45;  // distance for melee weapon to do damage
    public float fireRate;
    private float fireTime = 0;
    private float rotZ;
    private float lastRotZ = 0;

    private bool melee;
    
    private void Start()
    {
        // References
        player = GetComponent<PlayerScript>();
        cam = GetComponentInChildren<Camera>();
        rotatePoint = transform.GetChild(0);
        Hand = rotatePoint.transform.GetChild(0);
        HandS = rotatePoint.transform.GetChild(1);
        col = Hand.GetComponent<Collider2D>();
    }
    private void Update()
    {
        if (!GameManager.isPaused)
        {
            #region rotZ
            mousePos = Input.mousePosition;
            mousePos = cam.ScreenToWorldPoint(mousePos);

            Vector2 rotation = mousePos - transform.position;
            rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            rotatePoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            #endregion

            if (!GameManager.inv) // Unable to fire with inventory opened
            {
                if (Input.GetMouseButton(0))
                {
                    if (!melee)
                        RangedAttack();
                }
            }
            if (melee)
            {
                if (rotZ > lastRotZ + attackDist || lastRotZ - attackDist > rotZ)
                {
                    MeleeAttack();
                    lastRotZ = rotZ;
                }
            }
        }
    }
    private void MeleeAttack()
    {
        Vector2 attP = new Vector2(Hand.transform.position.x, Hand.transform.position.y);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attP, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
            enemy.GetComponent<EnemyScript>().TakeDamage(damage);
    }
    private void RangedAttack()
    {
        if (Time.time >= fireTime)
        {
            GameObject o = Instantiate(projectile, Hand.transform.position, Quaternion.identity);
            o.name += "-" + gameObject.tag;
            o.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            fireTime = Time.time + 1 / fireRate;
        }
    }
    public void EquipWeapon(Weapon weap)
    {
        // References
        Hand = transform.GetChild(0).GetChild(0);
        HandS = transform.GetChild(0).GetChild(1);
        if (weap != null)
        {
            SpriteRenderer Weapon = Hand.GetChild(0).GetComponent<SpriteRenderer>();
            SpriteRenderer WeaponS = HandS.GetChild(0).GetComponent<SpriteRenderer>();
            col = Hand.GetComponent<Collider2D>();
            
            Weapon.sprite = weap.texture;            
            Weapon.color = weap.color;

            switch ((int)weap.type)
            {
                case 0: melee = true;  damage = weap.damageModifier; break;  // Melee
                case 1:
                case 2: melee = false; projectile = weap.projectile; break;  // Magic + Ranged
            }
            col.enabled = melee;
        }
        else
            enabled = false;
    }
    #region Enable/Disabe
    private void OnEnable()
    { 
        transform.GetChild(0).gameObject.SetActive(true);
    }
    private void OnDisable()
    { 
        transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}