using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskStrafeAroundTarget : Node
{
    private float patrolSpeed;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        patrolSpeed = ((SoldierBehaviour)belongingTree).PatrolSpeed;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (!target)
        {
            state = NodeState.FAILURE; 
            return state;
        }
        
        if (navMeshAgent.enabled) 
        {
            navMeshAgent.stoppingDistance = 0.5f;

            navMeshAgent.speed = patrolSpeed;

            navMeshAgent.destination = transform.position + transform.right;
            transform.LookAt(target);
        }
        

        state = NodeState.RUNNING;
        return state;
    }
}
