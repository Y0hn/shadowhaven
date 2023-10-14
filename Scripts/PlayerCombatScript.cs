using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerCombatScript : MonoBehaviour
{
    public GameObject Hand;

    private PlayerScript player;
    private Animator animator;
    private Camera cam;
    private Vector2 mousePos;
    private bool CombatActive;

    private void Start()
    {
        CombatActive = false;
        Hand.SetActive(false);
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    private void Update()
    {
        if      (Input.GetMouseButton(0))
        {
            if (!CombatActive)
            {
                Hand.SetActive(true);
                CombatActive=true;
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (!CombatActive)
            {
                Hand.SetActive(true);
                CombatActive=true;
            }
        }
        else if (Input.GetMouseButton(2)) 
        { 
            Hand.SetActive(false);
            CombatActive=false;
        }
        if (CombatActive)
        {
            mousePos = Input.mousePosition;
            mousePos = cam.ScreenToWorldPoint(mousePos);

            Vector2 rotation = mousePos - player.GetPos();
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }
}
