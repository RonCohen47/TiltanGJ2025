using UnityEngine;

[CreateAssetMenu(fileName = "AssignmentSO", menuName = "Scriptable Objects/ProcessSO")]
public class ProcessSO : ScriptableObject
{
    public AssignmentSO InputAssignment;
    public AssignmentSO OutputAssignment;      // e.g., tomato -> chopped tomato
    public float TimeToComplete = 4;         // seconds to complete
    public bool CanBurn;
    public AssignmentSO BurnedOutputAssignment; // optional
    public float BurnTime = 7;     // time after done to burn
}