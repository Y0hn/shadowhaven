using UnityEngine;

public class BossSelectorBehavior : StateMachineBehaviour
{
    public string targetTag;
    public bool toClose = false;
    public const string triger = "attack";
    public const float minChangeInterval = 0.1f;
    public const float maxChangeInterval = 2f;

    private const float tolerancy = 0.2f;
    private Transform targetTra;
    private int pastTrigger = 0;
    private int setTriger = 0;
    private float timer;
    private float range;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        range = animator.GetComponent<EnemyStats>().lookRadius;
        //Debug.Log(name + " start walk with past triger: " + pastTriger);
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

        timer = Time.time + Random.Range(minChangeInterval, maxChangeInterval);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (setTriger == 0)
            switch (animator.name)
            {
                case "ZomBoss":
                    if      (ConditionForAttack(animator, "bonk"))
                        setTriger = 1;
                    else if (ConditionForAttack(animator, "charge"))
                        setTriger = 2;
                    else if (ConditionForAttack(animator, "spit"))
                        setTriger = 3;
                    else
                    {
                        //Debug.Log(name + " walk only");
                    }
                    break;

                default:
                    Debug.LogError("Boss " + name + " has no known behavior !");
                    break;
            }
        else if (timer < Time.time)
            animator.SetTrigger(triger + setTriger);
    }
    private bool ConditionForAttack(Animator animator, string atck)
    {
        bool condition = false;
        Vector2 pos = animator.transform.position;
        float dist = Vector2.Distance(targetTra.position, pos);

        switch (atck)
        {
            case "bonk":   
                condition = dist < range && pastTrigger != 1;
                break;
            case "charge":
                condition = pos.y - tolerancy < targetTra.position.y && targetTra.position.y < pos.y + tolerancy && pastTrigger != 2;
                break;
            case "spit":   
                condition = dist > range * 2 && pastTrigger != 3;
                break;

            default:
                Debug.LogError($"Fatal: Boss {name} does not have attack {atck} !!!");
                break;
        }

        return condition;        
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triger + setTriger);
        pastTrigger = setTriger;
    }
}