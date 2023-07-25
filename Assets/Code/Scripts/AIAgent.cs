using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : Agent
{
    /// <summary>
    /// The NavMeshAgent calling navigational moves.
    /// </summary>
    public NavMeshAgent navMeshAgent;

    /// <summary>
    /// Placeholder--for now, is used to always target the player.
    /// </summary>
    public GameObject targetEnemy;

    /// <summary>
    /// How far away should this agent keep itself from the enemy?
    /// </summary>
    public float enemyDistance;

    /// <summary>
    /// How far away can this AI begin to attack its target enemy?
    /// </summary>
    public float attackDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool attacking;

    private bool attackingStop;

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.destination = targetEnemy.transform.position;
        Debug.DrawLine(transform.position, navMeshAgent.destination);

        if (Vector3.Distance(transform.position, targetEnemy.transform.position) < attackDistance)
            attacking = true;
        else if (attacking)
        {
            attackingStop = true;
            attacking = false;
        }
        else
        {
            attackingStop = false;
        }

        if (attacking)
        {
            isFacing = true;
            facingPosition = targetEnemy.transform.position;
        }
        else
        {
            isFacing = false;
        }
    }

    public override float Input_GetAxis(string axis)
    {
        if (axis == "Vertical")
        {
            float distZ = navMeshAgent.destination.z - transform.position.z;
            //distZ -= enemyDistance;
            //Debug.Log(distZ);
            return navMeshAgent.velocity.z;
        }
        if (axis == "Horizontal")
        {
            float distX = navMeshAgent.destination.x - transform.position.x;
            //distX -= enemyDistance;
            //Debug.Log(distX);
            return navMeshAgent.velocity.x;
        }
        else
        {
            return 0;
        }
    }

    public override bool Input_GetButton(string button)
    {
        if (button == "Fire1")
        {
            return attacking;
        }
        else
        {
            return false;
        }    
    }

    public override bool Input_GetButtonDown(string button)
    {
        //if (button == "Fire1")
        //{
        //    return attackingStop;
        //}
        //else
        {
            return false;
        }
    }
    public override bool Input_GetButtonUp(string button)
    {
        if (button == "Fire1")
        {
            return attackingStop;
        }
        else
        {
            return false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyDistance);
    }
}
