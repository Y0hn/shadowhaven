using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCombatScript : MonoBehaviour
{
    public float attackRange;
    public LayerMask enemyLayers;

    #region References
    private GameObject projectile = null;
    private Transform idleProjectile;
    private SpriteRenderer bowString;
    private Transform rotatePoint;
    private PlayerScript player;
    private PlayerStats stats;
    private Transform tetiva;
    private Transform target;
    private Transform HandS;
    private Transform hand;
    private Collider2D col;
    private Camera cam;
    #endregion

    private Sprite[] textureSheet;
    private int weaponInvIndex;
    private int textureIndex;
    private Vector3 mousePos;

    private float attackDist = 45;  // distance for melee weapon to travel to do damage
    private float lastRotZ = 0;
    private float rotZ;

    public bool onControler = false;
    private float fireTime = 0;
    public float fireRate;
    private bool fireReady;
    private bool controler;
    private bool melee;

    private void Start()
    {
        if (player == null)
            SetReferences();
    }
    private void SetReferences()
    {
        player = GetComponent<PlayerScript>();
        cam = GetComponentInChildren<Camera>();
        stats = GetComponent<PlayerStats>();
        rotatePoint = transform.GetChild(0);
        hand = rotatePoint.transform.GetChild(0);
        target = hand.GetChild(1);
        //HandS = rotatePoint.transform.GetChild(1);
        col = hand.GetComponent<Collider2D>();
        idleProjectile = hand.GetChild(0).GetChild(0);
        tetiva = hand.GetChild(0).GetChild(1);
        bowString = tetiva.GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (GameManager.instance.ableToMove)
        {
            #region rotZ
            Vector2 rotation = Vector2.zero;
            rotation.x =    Mathf.Round(Input.GetAxis("Joy X") * 100) / 100;
            rotation.y = -  Mathf.Round(Input.GetAxis("Joy Y") * 100) / 100;
            //Debug.Log($"Controler readings [{rotation.x},{rotation.y}]");
            Vector3 mouse = Input.mousePosition;
            if (Mathf.Abs(rotation.x) < 0.01f && 0.01f > Mathf.Abs(rotation.y) || mousePos != mouse)
            {
                Vector3 m = cam.ScreenToWorldPoint(mouse);
                rotation = m - transform.position;
                mousePos = mouse;
                //Debug.Log($"Mouse Pos[{mousePos.x},{mousePos.y}]");
            }
            if (Mathf.Abs(rotation.x) > 0.25f && 0.25f < Mathf.Abs(rotation.y))
            {
                rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                rotatePoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);
                //Debug.Log($"Rotation is [{rotation.x},{rotation.y}] which coresponds to angle {rotZ}");
            }
            #endregion
            if (!melee)
            {
                if (Input.GetAxis("Fire") > 0)
                {
                    //textureSheet = ((Weapon)GameManager.inventory.Equiped(weaponInvIndex)).texture;
                    bool textureChange = false;

                    if (fireTime == 0f)
                    {
                        fireTime = Time.time + 1 / fireRate;
                        fireReady = false;
                        textureIndex = 1;
                    }
                    else if (fireTime - (1 / fireRate) / 2 < Time.time && Time.time < fireTime && textureIndex < 2)
                    {
                        textureChange = true;
                        textureIndex = 2;
                    }
                    else if (fireTime < Time.time && textureIndex < 3)
                    {
                        idleProjectile.gameObject.SetActive(true);
                        textureChange = true;
                        textureIndex = 3;
                        fireReady = true;
                    }

                    if (textureChange && textureIndex < textureSheet.Length - 1)
                        bowString.sprite = textureSheet[textureIndex];
                }
                else if (fireReady)
                {
                    RangedAttack();
                    if (textureSheet.Length > 0)
                        bowString.sprite = textureSheet[1];
                    idleProjectile.gameObject.SetActive(false);
                    fireReady = false;
                    fireTime = 0f;
                }
                else if (fireTime != 0)
                {
                    if (textureSheet.Length > 0)
                        bowString.sprite = textureSheet[1];
                    idleProjectile.gameObject.SetActive(false);
                    fireReady = false;
                    fireTime = 0f;
                }

            }
            else if (!(lastRotZ - attackDist < rotZ && rotZ < lastRotZ + attackDist))
            {
                MeleeAttack();
                lastRotZ = rotZ;
            }
        }
    }
    private void MeleeAttack()
    {
        Vector2 v = target.position - hand.position;
        Vector2 attP1 = new(hand.position.x, hand.position.y);
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
    }
    private void RangedAttack()
    {
        GameObject o = Instantiate(projectile, hand.transform.position, Quaternion.identity);
        o.GetComponent<ProjectileScript>().damage = stats.damage.GetValue();
        o.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        o.name += "-" + gameObject.tag;
        //Debug.Log("Projectile " + o.name + " seted damage to: " + stats.damage.GetValue());
        GameManager.audio.Play("bow-fire");
    }
    public bool EquipWeapon(Weapon weap)
    {
        // References
        if (player == null)
            SetReferences();

        if (weap != null)
        {
            SpriteRenderer rendProj = idleProjectile.GetComponent<SpriteRenderer>();
            weaponInvIndex = GameManager.inventory.GetIndexEquiped(weap);
            tetiva.gameObject.SetActive(false);
            SpriteRenderer Weapon = hand.GetChild(0).GetComponent<SpriteRenderer>();
            //SpriteRenderer WeaponS = HandS.GetChild(0).GetComponent<SpriteRenderer>();
            col = hand.GetComponent<Collider2D>();
            Weapon.sprite = weap.texture[0];            
            Weapon.color = weap.color;

            switch (weap.type)
            {
                case Type.Melee: 
                    melee = true;  
                    fireRate = 0;
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
                            //x = float.Parse(temp[0]);
                            //y = float.Parse(temp[1]);
                            idleProjectile.localScale = new Vector3(1, 1.5f, 1);
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
            idleProjectile.gameObject.SetActive(false);
            GetComponent<PlayerStats>().SetDamage(weap.damageModifier);
            textureSheet = ((Weapon)GameManager.inventory.Equiped(weaponInvIndex)).texture;
            //Debug.Log("Equiped weapon " + weap.name + " with damage of " + weap.damageModifier + " currrent value of damage is: " + stats.damage.GetValue());
            return true;
        }
        else
        {
            //Debug.LogWarning("Equiping weapon was null");
            enabled = false;
            return false;
        }
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