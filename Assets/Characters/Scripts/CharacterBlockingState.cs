using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterBlockingState : CharacterMovementBase
{
    [SerializeField] Rig blockingRig;
    private bool isBlocking = false;

    [HideInInspector] public UnityEvent<bool> UpdateBlockingStatus;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        UpdateBlockingStatus.Invoke(true);
        isBlocking = true;
    }

    private void Update()
    {
        ChangeBlockingRiggingWeight();
    }

    [SerializeField] float weightChangeAcceleration = 1f;
    private void ChangeBlockingRiggingWeight()
    {
        if (!isBlocking)
        {
            if (blockingRig.weight >= 0f)
                blockingRig.weight -= weightChangeAcceleration * Time.deltaTime;
            else
                blockingRig.weight = 0f;
        }
        else
        {
            if (blockingRig.weight <= 1f)
                blockingRig.weight += weightChangeAcceleration * Time.deltaTime;
            else
                blockingRig.weight = 1f;
        }
    }

    void OnBlock(InputValue inputValue)
    {
        if (IsBlockButtonReleased(inputValue))
        {
            UpdateBlockingStatus.Invoke(false);
            isBlocking = false;
            StartCoroutine(DisableScript());
        }
    }

    private static bool IsBlockButtonReleased(InputValue inputValue)
    {
        return Mathf.Approximately(inputValue.Get<float>(), 0f);
    }

    private IEnumerator DisableScript()
    {
        yield return new WaitUntil(() => blockingRig.weight <= 0f);

        this.enabled = false;
    }
}