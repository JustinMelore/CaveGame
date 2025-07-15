using System;
using UnityEngine;

public class ObjectiveItem : MonoBehaviour
{
    public static event Action<ObjectiveItem> OnObjeciveRangeEnter;
    public static event Action<ObjectiveItem> OnObjectiveRangeExit;

    private void OnTriggerEnter(Collider other)
    {
        OnObjeciveRangeEnter?.Invoke(this);
    }

    private void OnTriggerExit(Collider other)
    {
        OnObjectiveRangeExit?.Invoke(this);
    }
}
