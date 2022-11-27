using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterInput), typeof(CharacterMovement), typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterInput characterInput;
    private CharacterMovement characterMovement;

    int movementForward;
    int movementSideways;

    Vector3 oldPosition;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterInput= GetComponent<CharacterInput>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Start()
    {
        movementForward = Animator.StringToHash("movementForward");
        movementSideways = Animator.StringToHash("movementSideways");

        oldPosition = transform.position;
    }

    private void Update()
    {
        UpdateStandingAnimationTransitions();

        oldPosition = transform.position;
    }

    private void UpdateStandingAnimationTransitions()
    {
        Vector3 distanceMoved = transform.position - oldPosition;
        float amountOfForwardMovement = Vector3.Project(distanceMoved, transform.forward).magnitude;
        float amountOfSidewaysMovement = Vector3.Project(distanceMoved, transform.right).magnitude;

        float forwardMovementDirection = Mathf.Sign(characterInput.MovementDirection.z);
        float sidewaysMovementDirection = Mathf.Sign(characterInput.MovementDirection.x);

        float currentVelocityForward = (amountOfForwardMovement / Time.deltaTime) / characterMovement.Speed;
        float currentVelocitySideways = (amountOfSidewaysMovement / Time.deltaTime) / characterMovement.Speed;

        animator.SetFloat(movementForward, currentVelocityForward * forwardMovementDirection);
        animator.SetFloat(movementSideways, currentVelocitySideways * sidewaysMovementDirection);
    }
}