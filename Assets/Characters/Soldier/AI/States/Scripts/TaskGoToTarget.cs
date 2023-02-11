using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TaskGoToTarget : Node
{
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] float interactionDistanceThreshold = 1.5f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;
    private Transform currentTarget;
    private Vector3 targetPosition = Vector3.zero;
    private bool isSearching = false;

    private void Start()
    {
        player = ((SoldierBehaviour)belongingTree).Player;
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        decisionMaker = ((SoldierBehaviour)belongingTree).DecisionMaker;
        decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    }

    private void OnDestroy()
    {
        decisionMaker.OnTargetLost.RemoveListener(EraseInterestingTarget);
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        Transform searchTarget = (Transform)GetData("searchTarget");

        currentTarget = target ? target : searchTarget;
        isSearching = searchTarget ? true : false;

        if (currentTarget)
        {
            targetPosition = currentTarget.position;

            if (IsTargetWithinInteractionDistance())
            {
                if (isSearching && !target) // Have we lost sight of target and are searching it?
                {
                    state = NodeState.SUCCESS;
                    return state;
                }

                if (GetData("interactionAnimation") == null) // is a full body attack/interaction animation not playing? Then we can move to target.
                {
                    navMeshAgent.destination = targetPosition;
                    navMeshAgent.speed = runningSpeed;
                }

                state = NodeState.RUNNING;
                return state;
            }
            else
            {
                transform.LookAt(targetPosition);
                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }

    private bool IsTargetWithinInteractionDistance()
    {
        return (targetPosition - transform.position).sqrMagnitude > (interactionDistanceThreshold * interactionDistanceThreshold);
    }

    private void EraseInterestingTarget()
    {
        object t = GetData("target");
        if(t == null) { return; }

        if (state == NodeState.RUNNING)
        {
            if (player)
            {
                Parent.SetData("searchTarget", (Transform)t);
                isSearching = true;
            }
            
            ClearData("target");
        }
    }
}
