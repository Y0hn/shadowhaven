using UnityEngine;

public class ShootAtTargetBehavior : StateMachineBehaviour
{
    public string targetTag;
    public float rate;
    // if rate < 0      ===> then instafire
    public bool hide;
    public string triggerOut = "";
    public GameObject projectile;

    private Transform spawn;
    private Transform rotatePoint;
    private Transform targetTra;
    private float nextAtk = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Started shooting");
        // Finding rotete point in children
        rotatePoint = animator.transform.GetChild(0);
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

        if (rate < 0)
        {
            Fire(animator);
        }
        else
            nextAtk = Time.time + 1 / rate;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Shooting");
        if (rotatePoint != null)
        {
            Vector2 rotation = targetTra.position - animator.transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            rotatePoint.rotation = Quaternion.Euler(0, 0, rotZ);
        }
        if (Time.time >= nextAtk && rate > 0)
        {
            Fire(animator);
        }

        if (!triggerOut.Equals(""))
            animator.SetTrigger(triggerOut);
    }
    private void Fire(Animator animator)
    {
        GameObject o = Instantiate(projectile, spawn.position, Quaternion.identity);
        o.name += "-" + LayerMask.LayerToName(animator.gameObject.layer);
        o.GetComponent<ProjectileScript>().damage =
            animator.transform.GetComponent<CharakterStats>().damage.GetValue();
        nextAtk = Time.time + 1 / rate;
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hide)
            rotatePoint.gameObject.SetActive(false);
        if (triggerOut != "")
            animator.ResetTrigger(triggerOut);
    }
}
