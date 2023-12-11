using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class ShootAtTargetBehavior : StateMachineBehaviour
{
    public string targetTag;
    public float rate;
    public bool hide;
    public GameObject projectile;

    private Transform spawn;
    private Transform targetTra;
    private Transform rotatePoint;
    private float nextAtk = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Finding rotete point in children
        for (int i = 0; i < animator.transform.childCount; i++)
        {
            if (animator.transform.GetChild(i).tag == "Weapon")
            {
                rotatePoint = animator.transform.GetChild(i);
                break;
            }
        }
        spawn = rotatePoint.transform.GetChild(0).transform;
        if (hide)
            rotatePoint.gameObject.SetActive(true);
        // Finding closet target
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
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
        Vector2 rotation = targetTra.position - animator.transform.position;            
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotatePoint.rotation = Quaternion.Euler(0, 0, rotZ);

        if (Time.time >= nextAtk)
        {
            GameObject o = Instantiate(projectile, spawn.position, Quaternion.identity);
            o.name += "-" + LayerMask.LayerToName(animator.gameObject.layer);            
            o.GetComponent<ProjectileScript>().damage = 
                animator.transform.GetComponent<CharakterStats>().damage.GetValue();
            nextAtk = Time.time + 1 / rate;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hide)
            rotatePoint.gameObject.SetActive(false);
    }
}
