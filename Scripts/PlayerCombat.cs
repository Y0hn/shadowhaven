using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    public float attackRange;
    public LayerMask enemyLayers;

    #region References
    private Transform WeaponIdleProjectile;
    private GameObject projectile = null;
    private SpriteRenderer bowString;
    private Transform rotatePoint;
    private PlayerScript player;
    private PlayerStats stats;
    private Transform target;
    private Transform HandS;
    private Transform hand;
    private Collider2D col;
    private Camera cam;
    #endregion

    private int weaponInvIndex;
    private Vector3 mousePos;

    private float attackDist = 45;  // distance for melee weapon to travel to do damage
    private float meleeSoundTimer;
    private float meleeSTime = 1f;
    private float lastRotZ = 0;
    private float rotZ;

    private float fireTime = 0;
    public float fireRate;
    private bool melee;

    
    private void Start()
    {
        // References
        player = GetComponent<PlayerScript>();
        cam = GetComponentInChildren<Camera>();
        stats = GetComponent<PlayerStats>();
        rotatePoint = transform.GetChild(0);
        hand = rotatePoint.transform.GetChild(0);
        target = hand.GetChild(1);
        //HandS = rotatePoint.transform.GetChild(1);

        col = hand.GetComponent<Collider2D>();
        WeaponIdleProjectile = hand.GetChild(0).GetChild(0);
        bowString = hand.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();

        meleeSoundTimer = 0;
    }
    private void Update()
    {
        if (GameManager.ableToMove)
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
                if (!melee && Input.GetMouseButton(1))
                {
                    Sprite[] texture = ((Weapon)Inventory.instance.Equiped(weaponInvIndex)).texture;
                    bool multiSheet = texture.Length > 1;

                    if (fireTime == 0f)
                        fireTime = Time.time + 1 / fireRate;
                    else if (fireTime < Time.time)
                    {
                        if (multiSheet)
                            hand.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = texture[3];
                        WeaponIdleProjectile.gameObject.SetActive(true);
                        // Animacia naprahovania luku
                        if (Input.GetMouseButtonDown(0))
                        {
                            RangedAttack();
                            if (texture.Length > 0)
                                hand.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = texture[1];
                            WeaponIdleProjectile.gameObject.SetActive(false);
                            fireTime = 0f;
                        }
                    }
                    // Ranged Animation
                    if (fireTime - (1 / fireRate) / 2 < Time.time && Time.time < fireTime)
                    {
                        if (multiSheet)
                            bowString.sprite = texture[2];
                    }
                }
                else if (fireTime != 0f)
                {
                    fireTime = 0f;
                    bowString.sprite = ((Weapon)Inventory.instance.Equiped(weaponInvIndex)).texture[1];
                    WeaponIdleProjectile.gameObject.SetActive(false);
                }

            }
            if (melee)
            {
                if (!(lastRotZ - attackDist < rotZ && rotZ < lastRotZ + attackDist))
                {
                    MeleeAttack();
                    lastRotZ = rotZ;
                }
            }
        }
    }
    private void MeleeAttack()
    {
        Vector2 v = target.position - hand.position;
        Vector2 attP1 = new Vector2(hand.position.x, hand.position.y);
        Vector2 attP2 = new(hand.position.x - v.y, hand.position.y + v.x);

        List<Collider2D> hitEnemies = new();
        hitEnemies.AddRange(Physics2D.OverlapCircleAll(attP1, attackRange, enemyLayers));
        // v = new(hitEnemies.Count, 0); // debug
        foreach (Collider2D enemy in Physics2D.OverlapCircleAll(attP2, attackRange, enemyLayers))
            if (!hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                v = new(v.x, v.y + 1);
            }

        foreach (Collider2D enemy in hitEnemies)
            enemy.GetComponent<EnemyStats>().TakeDamage(stats.damage.GetValue());

        //Debug.Log($"[Time: {Time.time}] Melee Attack Enemies hited {hitEnemies.Count}\nattP1[{attP1.x},{attP1.y}] hitted {v.x} enemies \nattP2[{attP2.x},{attP2.y}] hitted {v.y} enemies");
        if (hitEnemies.Count == 0 && meleeSoundTimer < Time.time)
        {
            AudioManager.instance.Play("sword-wush");
            meleeSoundTimer = Time.time + meleeSTime;
        }
    }
    private void RangedAttack()
    {
        GameObject o = Instantiate(projectile, hand.transform.position, Quaternion.identity);
        o.GetComponent<ProjectileScript>().damage = stats.damage.GetValue();
        o.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        o.name += "-" + gameObject.tag;

        AudioManager.instance.Play("bow-fire");
    }
    public void EquipWeapon(Weapon weap)
    {
        // References
        hand = transform.GetChild(0).GetChild(0);
        HandS = transform.GetChild(0).GetChild(1);
        Transform tetiva = hand.GetChild(0).GetChild(1);
        Transform idleProjectile = hand.GetChild(0).GetChild(0);
        SpriteRenderer rendProj = idleProjectile.GetComponent<SpriteRenderer>();
        tetiva.gameObject.SetActive(false);

        if (weap != null)
        {
            SpriteRenderer Weapon = hand.GetChild(0).GetComponent<SpriteRenderer>();
            //SpriteRenderer WeaponS = HandS.GetChild(0).GetComponent<SpriteRenderer>();
            col = hand.GetComponent<Collider2D>();
            
            Weapon.sprite = weap.texture[0];            
            Weapon.color = weap.color;

            switch (weap.type)
            {
                case Type.Melee: 
                    melee = true;  
                    break;  // Melee
                case Type.Ranged:
                case Type.Magic: 
                    melee = false; 
                    projectile = weap.projectile;
                    tetiva.gameObject.SetActive(true);
                    if (weap.texture.Length > 1)
                        tetiva.GetComponent<SpriteRenderer>().sprite = weap.texture[1];
                    rendProj.sprite = projectile.GetComponent<SpriteRenderer>().sprite;
                    break;  // Magic + Ranged
            }
            col.enabled = melee;

            // Weapon additional properties
            string[] t = weap.description.Split('#');
            if (t.Length > 1)
            {
                string s = t[1];
                string[] sAr = s.Split(')');
                for (int i = 0; i < sAr.Length-1; i++)
                {
                    string[] temp = sAr[i].Split('(');
                    string parameter = temp[0];
                    temp = temp[1].Split('_');

                    switch (parameter)
                    {
                        case "ProjPos": // Porjectile position
                            float x = float.Parse(temp[0]);
                            float y = float.Parse(temp[1]);
                            idleProjectile.localPosition = new Vector3(x, y, 0);
                            //Debug.Log($"Idle projectile position set to ({x},{y},0)");
                            break;
                        case "ProjScale":
                            x = float.Parse(temp[0]);
                            y = float.Parse(temp[1]);
                            idleProjectile.localScale = new Vector3(x, y, 1);
                            //Debug.Log($"Idle projectile scale set to ({x},{y},0)");
                            break;
                        case "FireRate":
                            fireRate = float.Parse(temp[0]);
                            break;

                        case "StringColor":
                            float 
                                r = float.Parse(temp[0]),
                                g = float.Parse(temp[1]),
                                b = float.Parse(temp[2]);
                            tetiva.GetComponent<SpriteRenderer>().color = new(r, g, b);
                            break;

                        default:
                            Debug.LogWarning($"Parameter \"{parameter}\" of {weap.name} was not recognized!");
                            break;
                    }
                }
            }
            rendProj.gameObject.SetActive(false);
            weaponInvIndex = Inventory.instance.GetIndexEquiped(weap);
            GetComponent<PlayerStats>().SetDamage(weap.damageModifier);
            //Debug.Log("Player Damage set to: " + weap.damageModifier);
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