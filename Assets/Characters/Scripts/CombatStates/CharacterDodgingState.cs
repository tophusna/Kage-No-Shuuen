using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterDodgingState : CharacterStateBase
{
    [SerializeField] float speedDeceleration = 0.05f;

    [Header("Exit States")]
    [SerializeField] CharacterIdleState idleState;

    [HideInInspector] public UnityEvent MakeCharacterDodge;

    private Vector3 dodgeFacingDirection;
    private float currentSpeed;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        SetDodgeFacingDirection(currentHorizontalMovement.normalized);

        idleState.move.Disable();

        MakeCharacterDodge.Invoke();
        currentSpeed = speed;
    }

    private void OnDisable()
    {
        OrientateCharacterForward();

        if (gameObject.activeInHierarchy)
            idleState.EnableMovement();
    }

    private void Update()
    {
        transform.forward = dodgeFacingDirection;

        Vector3 movementOnPlane = currentSpeed * Time.deltaTime * dodgeFacingDirection;
        Vector3 verticalMovement = new Vector3(0f, Physics.gravity.y * Time.deltaTime, 0f);

        charController.Move(movementOnPlane + verticalMovement);
        EaseOutCurrentSpeed();
    }

    private void EaseOutCurrentSpeed()
    {
        currentSpeed -= speedDeceleration * Time.deltaTime;
    }

public void SetDodgeFacingDirection(Vector3 facingDirection)
    {
        dodgeFacingDirection = facingDirection;
    }
}
