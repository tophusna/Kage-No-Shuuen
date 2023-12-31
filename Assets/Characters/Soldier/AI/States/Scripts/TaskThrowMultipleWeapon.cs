using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskThrowMultipleWeapon : Node
{
    [SerializeField] int amountToThrow = 3;
    [SerializeField] float distanceThresholdForThrowing = 5f;
    [SerializeField] float timeBetweenThrows = 5f;
    [SerializeField] float timeBetweenSingleWeapons = 0.1f;
    [SerializeField] ThrowingWeaponBase throwWeapon;
    [SerializeField] AudioClip throwSound;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;
    private AudioSource audioSource;

    private float throwCounter = 5f;
    private int singleWeaponsThrownCounter = 0;
    private bool isThrowingAnimationRunning = false;
    [SerializeField] bool shouldWeaponDisappear = true;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        audioSource = ((SoldierBehaviour)belongingTree).AudioSource;
    }

    public override NodeState Evaluate()
    {
        if (isThrowingAnimationRunning)
        {
            state = NodeState.RUNNING;
            return state;
        }

        object t = GetData("target");
        Transform target = (Transform)t;

        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        throwCounter += Time.deltaTime;
        if (IsTargetCloserThanThrowingThreshold(target))
        {
            state = NodeState.FAILURE;
            return state;
        }
        
        if(throwCounter <= timeBetweenThrows)
        {
            state = NodeState.FAILURE;
            return state;
        }

        navMeshAgent.speed = 0f;
        
        if(shouldWeaponDisappear)
            throwWeapon.gameObject.SetActive(true);

        transform.LookAt(target);
        characterAnimator.PlayThrowingAnimation();
        isThrowingAnimationRunning = true;
        Parent.SetData("interactionAnimation", true);
        throwCounter = 0f;

        state = NodeState.RUNNING;
        return state;
    }

    private bool IsTargetCloserThanThrowingThreshold(Transform target)
    {
        return (target.position - transform.position).sqrMagnitude < (distanceThresholdForThrowing * distanceThresholdForThrowing);
    }

    // Called from an animation event
    public void ThrowWeapon()
    {
        StartCoroutine(ThrowInSequence());
        if (shouldWeaponDisappear)
            throwWeapon.gameObject.SetActive(false);
    }

    private IEnumerator ThrowInSequence()
    {
        while (isThrowingAnimationRunning)
        {
            if(singleWeaponsThrownCounter >= amountToThrow) { break; }

            throwWeapon?.Throw();
            audioSource.PlayOneShot(throwSound);
            singleWeaponsThrownCounter++;

            yield return new WaitForSeconds(timeBetweenSingleWeapons);
        }
        singleWeaponsThrownCounter = 0;
    }

    // Called from an animation event
    public void ExitThrowingState()
    {
        isThrowingAnimationRunning = false;
        ClearData("interactionAnimation");
    }
}
