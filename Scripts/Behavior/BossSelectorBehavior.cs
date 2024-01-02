using UnityEngine;

public class BossSelectorBehavior : StateMachineBehaviour
{
    public string targetTag;
    public bool toClose = false;
    public string triger = "attack";
    public float minChangeInterval = 0.01f;

    private Transform targetTra;
    private string settedTriger;
    private float timer;
    private float range;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        range = animator.GetComponent<EnemyStats>().lookRadius;

        // Finding closet target
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
        timer = Time.time + minChangeInterval;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 pos = animator.transform.position;
        bool behaviorChange = false;

        float dist = Vector2.Distance(targetTra.position, pos);

        int n = animator.GetComponent<BossStats>().numberOfAttacks + 1;

        switch (animator.name)
        {
            case "ZomBoss":
                // range = 5
                const float tolerancy = 0.2f;
                float targetY = targetTra.position.y,
                thisY = pos.y;

                if (dist < range)
                {
                    // Bonk
                    behaviorChange = true;
                    n = 1;
                }
                else if (thisY - tolerancy < targetY && targetY < thisY + tolerancy)
                {
                    // Charge
                    behaviorChange = true;
                    n = 2;
                }
                break;

            default:
                Debug.Log("Behavior was defaulted for: " + animator.name);
                n = Random.Range(1, n);
                behaviorChange = true;
                break;
        }

        if (behaviorChange && timer < Time.time)
        {
            settedTriger = triger + n;
            animator.SetTrigger(settedTriger);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!settedTriger.Equals(""))
        {
            animator.ResetTrigger(settedTriger);
            settedTriger = "";
        }
    }
}
