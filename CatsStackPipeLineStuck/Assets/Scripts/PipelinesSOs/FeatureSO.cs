using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureSO", menuName = "Scriptable Objects/FeatureSO")]
public class FeatureSO : ScriptableObject
{
    //id
    public string featureName;
    public List<AssignmentSO> requiredAssignments; // order-agnostic contents on a plate
    public float dueTimeSeconds = 60f;            // seconds to fulfill
}
