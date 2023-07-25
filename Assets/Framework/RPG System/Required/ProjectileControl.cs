using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public struct ProjectileState
{
    [Tooltip("The index of the desired ProjectileEmitter from the list/hierarchy order of the attached Weapon script.")]
    public int index;

    [Tooltip("Should this ProjectileEmitter's particles play or stop?")]
    public bool play;
}

public class ProjectileControl : StateMachineBehaviour
{
    [Tooltip("List of ProjectileEmitter indexes and whether to play them upon entering this State.")]
    public List<ProjectileState> projectileStatesEnter;

    [Tooltip("List of ProjectileEmitter indexes and whether to play them upon exiting this State.")]
    public List<ProjectileState> projectileStatesExit;

    // The Weapon we want the ProjectileEmitters from. Automatically detected.
    private Weapon m_weapon;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Automatically get the Weapon if yet to be found.
        if (m_weapon == null)
            m_weapon = animator.GetComponent<Weapon>();

        // Evaluate each ProjectileState with its associated Particle Emitter on the Weapon upon entry.
        for (int i = 0; i < projectileStatesEnter.Count; i++)
            EvalState(projectileStatesEnter[i], m_weapon.projectileEmitters[projectileStatesEnter[i].index].pas);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Automatically get the Weapon if yet to be found.
        if (m_weapon == null)
            m_weapon = animator.GetComponent<Weapon>();

        // Evaluate each ProjectileState with its associated Particle Emitter on the Weapon upon exit.
        for (int i = 0; i < projectileStatesExit.Count; i++)
            EvalState(projectileStatesExit[i], m_weapon.projectileEmitters[projectileStatesExit[i].index].pas);
    }

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

    /// <summary>
    /// Evaluates a ProjectileState on whether to Play or Stop its ProjectileEmitter's Particle System.
    /// </summary>
    /// <param name="pState">The ProjectileState to be evaluated.</param>
    /// <param name="pas">The Particle System to Play or Stop.</param>
    public void EvalState(ProjectileState pState, ParticleSystem pas)
    {
        if (pState.play)
            pas.Play();
        else
            pas.Stop();
    }
}
