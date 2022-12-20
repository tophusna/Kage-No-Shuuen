using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterDodgingState : CharacterMovementBase
{
    [SerializeField] float speedDeceleration = 0.05f;

    [Header("Exit States")]
    [SerializeField] CharacterIdleState idleState;

    [HideInInspector] public UnityEvent makeCharaterDodge;

    private Vector3 dodgeFacingDirection;
    private float currentSpeed;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        makeCharaterDodge.Invoke();
        currentSpeed = speed;
    }

    private void Update()
    {
        transform.forward = dodgeFacingDirection;

        Vector3 movementOnPlane = currentSpeed * Time.deltaTime * dodgeFacingDirection;
        Vector3 verticalMovement = new Vector3(0f, Physics.gravity.y * Time.deltaTime, 0f);

        charController.Move(movementOnPlane + verticalMovement);
        EaseOutCurrentSpeed();
    }

    private float t;
    private void EaseOutCurrentSpeed()
    {
        currentSpeed -= speedDeceleration * Time.deltaTime;

        Debug.Log(currentSpeed);
    }

public void SetDodgeFacingDirection(Vector3 facingDirection)
    {
        dodgeFacingDirection = facingDirection;
    }

    public void ExitDodgingState()
    {
        idleState.enabled = true;
        this.enabled = false;

        OrientateCharacterForward();
    }
}
