using Unity.VisualScripting;
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
    private float lastAtc = 0;
    private float tChange;
    private float timer;
    private float range;
    private bool ch;
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
        ch = true;
        tChange = Random.Range(minChangeInterval, maxChangeInterval);
        timer = Time.time + tChange;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (setTriger == 0)
            switch (animator.name)
            {
                case "ZomBoss":
                    if      (ConditionForAttack(animator, "bonk", 1))
                        setTriger = 1;
                    else if (ConditionForAttack(animator, "charge", 2))
                        setTriger = 2;
                    else if (ConditionForAttack(animator, "spit", 3))
                        setTriger = 3;
                    else
                    {
                        //Debug.Log(name + " walk only");
                    }
                    //Debug.Log("Choosed: " + setTriger);
                    break;
                case "ArchDevil":
                    if      (ConditionForAttack(animator, "trust", 1))
                        setTriger = 1;
                    else if (ConditionForAttack(animator, "slash", 2))
                        setTriger = 2;

                    break;
                default:
                    Debug.LogError("Boss " + animator.name + " has no known behavior !");
                    animator.enabled = false;
                    break;
            }
        else if (timer < Time.time && ch)
        {
            //Debug.Log("Set Trigger is: " + setTriger + ", Last Trigger was: " + pastTrigger + " and time past: " + tChange + " last attack was: " + lastAtc + " and Time is: " + Time.time);
            animator.SetTrigger(triger + setTriger);
            ch = false;
        }
    }
    private bool ConditionForAttack(Animator animator, string atck, int trigger)
    {
        bool condition = false;
        Vector2 pos = animator.transform.position;
        float dist = Vector2.Distance(targetTra.position, pos);
        bool timesUp = lastAtc <= Time.time - (maxChangeInterval + maxChangeInterval);

        switch (atck)
        {
            case "bonk":
                condition = dist < range && (pastTrigger % 10 != trigger || timesUp);
                break;
            case "charge":
                condition = dist > 5 * tolerancy && pos.y - tolerancy < targetTra.position.y && targetTra.position.y < pos.y + tolerancy && pastTrigger % 10 != trigger;
                break;
            case "spit":
                condition = dist > range * 2 && (pastTrigger % 10 != trigger || (timesUp && pastTrigger < 23));
                break;
            case "trust":
                condition = dist < range * 1.5f && (pastTrigger % 10 != trigger || timesUp && pastTrigger < 21);
                break;
            case "slash":
                condition = dist < range && (pos.y - tolerancy < targetTra.position.y && targetTra.position.y < pos.y + tolerancy) && (pastTrigger % 10 != trigger || timesUp);
                break;
            default:
                Debug.LogError($"Fatal: Boss {name} requesting non-existent attack {atck} !!!");
                break;
        }
        //
        if (condition) Debug.Log($"Condition for {atck} is fullfiled (pasttrigger = {pastTrigger})");
        return condition;        
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triger + setTriger);
        if (pastTrigger % 10 == setTriger)
            pastTrigger += 10;
        else
            pastTrigger = setTriger;
        lastAtc = Time.time;
        setTriger = 0;
    }
}