using System;
using UnityEngine;

public class ObjectiveItem : MonoBehaviour
{
    public static event Action<ObjectiveItem> OnObjeciveRangeEnter;
    public static event Action<ObjectiveItem> OnObjectiveRangeExit;

    [SerializeField] private SphereCollider detectionRange;

    private void OnTriggerEnter(Collider other)
    {
        OnObjeciveRangeEnter?.Invoke(this);
    }

    private void OnTriggerExit(Collider other)
    {
        OnObjectiveRangeExit?.Invoke(this);
    }

    public float GetObjectiveRange()
    {
        return detectionRange.radius;
    }
}
