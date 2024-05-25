using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointAtTargetBeh : StateMachineBehaviour
{
    public string targetTag;
    public string triggerOut = "";
    public string outPutParam = "";

    private Transform rotatePoint;
    private Transform targetTra;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rotatePoint == null)
            rotatePoint = animator.transform.GetChild(0);
        if (targetTra == null)
        {
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
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rotatePoint != null && targetTra != null)
        {
            Vector2 rotation = targetTra.position - animator.transform.position;
            
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            rotatePoint.rotation = Quaternion.Euler(0, 0, rotZ);
            if (outPutParam != "")
            {
                string[] param = outPutParam.Split(',');
                animator.SetFloat(param[0].Trim(), rotation.x);
                animator.SetFloat(param[1].Trim(), rotation.y);
            }
            Debug.Log($"Rotate point {rotatePoint.name} has been set on rotation [{rotZ}]");
        }
        else
            Debug.LogWarning("Rotate point or target is null");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (triggerOut != "")
            animator.ResetTrigger(triggerOut);
    }
}
