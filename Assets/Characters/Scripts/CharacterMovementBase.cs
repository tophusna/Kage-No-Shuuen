using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CharacterMovementBase : MonoBehaviour
{
    [Header("Movement Characteristics")]
    [SerializeField] protected float speed = 6f;
    public float Speed => speed;

    private static Camera mainCamera;
    private static CharacterController charController;

    protected static Vector3 movementDirection;
    public static Vector3 MovementDirection => movementDirection;

    private static float velocityY = 0f;

    protected void SetCameraAndCharController(CharacterController characterController)
    {
        mainCamera = Camera.main;
        charController = characterController;
    }

    static float movingSpeed;
    static Vector3 currentHorizontalMovement = Vector3.zero;
    [SerializeField] static float accMovementDir = 1.5f; // m/s2
    protected void UpdateMovement(float speed, Vector3 movementDirection)
    {
        UpdateCharacterSpeed(speed);
        ApplyAccelerationSmoothingToMovingDirection(movementDirection);

        Vector3 horizontalMovement = movingSpeed * Time.deltaTime * currentHorizontalMovement;
        Vector3 verticalMovement = UpdateVerticalMovement();
        
        charController.Move(horizontalMovement + verticalMovement);

        OrientateCharacterForwardWhenMoving();
    }

    private void ApplyAccelerationSmoothingToMovingDirection(Vector3 movementDirection)
    {
        Vector3 desiredHorizontalMovement = UpdateHorizontalMovement(movementDirection);
        Vector3 direction = desiredHorizontalMovement - currentHorizontalMovement;
        float speedChangeToApply = accMovementDir * Time.deltaTime;
        speedChangeToApply = Mathf.Min(speedChangeToApply, direction.magnitude);
        currentHorizontalMovement += direction.normalized * speedChangeToApply;
    }

    [SerializeField] static float moveAcceleration = 5;    // m/s2
    private void UpdateCharacterSpeed(float targetSpeed)
    {
        if (movingSpeed < targetSpeed)
        {
            movingSpeed += moveAcceleration * Time.deltaTime;
            if (movingSpeed > targetSpeed) { movingSpeed = targetSpeed; }
        }
        else if (movingSpeed > targetSpeed)
        {
            movingSpeed -= moveAcceleration * Time.deltaTime;
            if (movingSpeed < targetSpeed) { movingSpeed = targetSpeed; }
        }
    }

    private Vector3 UpdateVerticalMovement()
    {
        velocityY = Physics.gravity.y * Time.deltaTime;

        if (charController.isGrounded)
        {
            velocityY = -0.1f;
        }

        return new Vector3(0, velocityY, 0);
    }

    private Vector3 UpdateHorizontalMovement(Vector3 movementDirection)
    {
        Vector3 movement;
        movement = ApplyMovementRelativeToCameraPosition(movementDirection);

        return movement;
    }

    private Vector3 ApplyMovementRelativeToCameraPosition(Vector3 movementDirection)
    {
        Vector3 movement = mainCamera.transform.TransformDirection(movementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }

    // New Implementation
    // TODO: put it in its own class?
    private static float timeToOrientateCharacterForward = 0.25f;
    private void OrientateCharacterForwardWhenMoving()
    {
        if (Mathf.Abs(movingSpeed) > 0.1f)
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);

            DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);

            // Alternativa sin DoTween pero con un fallo
            // forwardOrientationSpeed lo ten�a a 3f, sin embargo al ir en diagonal hacia atr�s y mover mucho al personaje le terminaba viendo la cara.
            //Vector3 forwardVectorToLookAtThisFrame = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            //Vector3 angleToOrientateCharacterThisFrame = Vector3.RotateTowards(transform.forward, forwardVectorToLookAtThisFrame, forwardOrientationSpeed * Time.deltaTime, 0f);
            //transform.rotation = Quaternion.LookRotation(angleToOrientateCharacterThisFrame, Vector3.up);
        }
    }
}