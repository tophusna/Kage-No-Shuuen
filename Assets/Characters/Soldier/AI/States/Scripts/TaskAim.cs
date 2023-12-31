using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;

public class TaskAim : Node
{
    [Header("Fanciness")]
    [SerializeField] GameObject quiverArrow;
    [SerializeField] GameObject loadedArrow;
    [SerializeField] Transform bowstring;
    [SerializeField] AudioClip pullBowstringSound;

    [Header("Animation Settings")]
    [SerializeField] float animationAcceleration = 0.05f;    
    [SerializeField] Transform leftHand;

    [Header("Rig Settings")]
    [SerializeField] Rig aimingRig;

    private Vector3 initialBowstringPosition = Vector3.zero;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;
    private HitStateBehaviour hitBehaviour;
    private DeadStateBehaviour deadBehaviour;
    private AudioSource audioSource;

    private bool isAimingAnimationPlaying = false;
    private bool isPullingBowstring = false;
    private bool isReadyToShoot = false;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        hitBehaviour = characterAnimator.Animator.GetBehaviour<HitStateBehaviour>();
        deadBehaviour = characterAnimator.Animator.GetBehaviour<DeadStateBehaviour>();
        audioSource = ((SoldierBehaviour)belongingTree).AudioSource;

        hitBehaviour.EnterState.AddListener(StopAiming);
        deadBehaviour.EnterState.AddListener(StopAiming);

        initialBowstringPosition = bowstring.localPosition;
    }

    private void OnDestroy()
    {
        hitBehaviour.EnterState.RemoveListener(StopAiming);
        deadBehaviour.EnterState.RemoveListener(StopAiming);
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (isAimingAnimationPlaying)
        {
            aimingRig.weight = (aimingRig.weight >= 1f) ? 1f : (aimingRig.weight += animationAcceleration * Time.deltaTime);

            if (isPullingBowstring)
            {
                bowstring.localPosition += bowstring.InverseTransformPoint(leftHand.TransformPoint(leftHand.localPosition));
            }

            if ((aimingRig.weight >= 1f) &&
                (isReadyToShoot))
            {
                transform.LookAt(target);
                navMeshAgent.speed = 0f;

                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.RUNNING;
            return state;
        }

        transform.LookAt(target);
        characterAnimator.PlayAimingAnimation(true);
        isAimingAnimationPlaying = true;
        Parent.SetData("interactionAnimation", true);

        state = NodeState.RUNNING;
        return state;
    }

    // Called from Animation Event
    internal void SpawnArrowInHand()
    {
        quiverArrow.SetActive(true);
    }

    internal void PullBowstring()
    {
        isPullingBowstring = true;
        audioSource.PlayOneShot(pullBowstringSound);
    }

    // Called from animation event
    internal void ReadyToShoot()
    {
        isPullingBowstring = false;
        isReadyToShoot = true;
    }

    // Called from Animation Event
    internal void SpawnArrowInBow()
    {
        quiverArrow.SetActive(false);
        loadedArrow.SetActive(true);
    }

    // Called from Animation Event
    internal void ReleaseBowstring()
    {
        isAimingAnimationPlaying = false;
        isReadyToShoot= false;
        bowstring.localPosition = initialBowstringPosition;
        loadedArrow.SetActive(false);
    }

    // Called from Animation Event
    internal void ReloadBow()
    {
        characterAnimator.PlayAimingAnimation(true);
        isAimingAnimationPlaying = true;
        Parent.SetData("interactionAnimation", true);
    }

    // TODO: Sometimes soldier stops and doesn't return to guarding point upon target lost.
    private void StopAiming()
    {
        characterAnimator.PlayAimingAnimation(false);

        if (loadedArrow.activeInHierarchy)
        {
            loadedArrow.SetActive(false);
        }

        aimingRig.weight = 0f;
    }
}
