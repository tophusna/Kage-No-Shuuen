using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedObject : MonoBehaviour
{
    [SerializeField] RectTransform indicatorPrefab = null;
    public RectTransform IndicatorPrefab => indicatorPrefab;

    private bool isIndicatorVisible = false;
    public bool IsIndicatorVisible => isIndicatorVisible;
    public void SetIsIndicatorVisible(bool isIndicatorVisible) => this.isIndicatorVisible = isIndicatorVisible;

    private void Start()
    {
        IndicatorController.manager.AddTrackingIndicator(this);
        isIndicatorVisible = false;
    }

    private void OnDestroy()
    {
        IndicatorController.manager.RemoveTrackingIndicator(this);
    }

    public void HideIndicator()
    {
        isIndicatorVisible = false;
    }
}
