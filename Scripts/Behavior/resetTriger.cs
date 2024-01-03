using UnityEngine;

public class resetTriger : StateMachineBehaviour
{
    public string triger = "triger";
    public bool uponEnter = false;
    public bool uponExit = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (uponEnter) 
        {
            animator.ResetTrigger(triger);
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (uponExit) 
        {
            animator.ResetTrigger(triger);
        }
    }
}
