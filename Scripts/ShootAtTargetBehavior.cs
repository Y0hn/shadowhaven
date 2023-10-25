using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class ShootAtTargetBehavior : StateMachineBehaviour
{
    public float rate;
    public string tag;
    public bool hide;
    public GameObject projectile;

    private Transform spawn;
    private Transform targetTra;
    private GameObject rotatePoint;
    private float rotZ;
    private float nextAtk = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Finding rotete point in children
        for (int i = 0; i < animator.transform.childCount; i++)
        {
            if (animator.transform.GetChild(i).tag == "Weapon")
            {
                rotatePoint = animator.transform.GetChild(i).gameObject;
                break;
            }
        }
        spawn = rotatePoint.transform.GetChild(0).transform;
        if (hide)
            rotatePoint.SetActive(true);
        // Finding closet target
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        float minDistance = Mathf.Infinity;
        foreach (GameObject target in targets) 
        { 
            float distance = Vector2.Distance(target.transform.position, animator.transform.position);
            if (distance < minDistance)
            { 
                targetTra = target.transform;
                minDistance = distance;
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Time.time >= nextAtk)
        {
            GameObject.Instantiate(projectile, spawn.position, rotatePoint.transform.rotation);
            nextAtk = Time.time + 1 / rate;
        }
        Vector2 rotation = targetTra.position - animator.transform.position;            
        rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotatePoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hide)
            rotatePoint.SetActive(false);
    }
}
