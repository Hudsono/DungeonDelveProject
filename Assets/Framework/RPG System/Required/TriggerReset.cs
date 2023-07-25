using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resets a list of given Trigger parameters upon entering a state. Useful for issues with Triggers being enabled in places where they shouldn't have been.
/// </summary>
public class TriggerReset : StateMachineBehaviour
{
    [Tooltip("Names of Trigger parameters of this Animator to be reset.")]
    public List<string> triggerNames;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Go through and reset the defined Trigger parameters.
        for (int i = 0; i < triggerNames.Count; i++)
            animator.ResetTrigger(triggerNames[i]);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
