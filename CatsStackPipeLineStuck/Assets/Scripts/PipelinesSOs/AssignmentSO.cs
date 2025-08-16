using UnityEngine;

[CreateAssetMenu(fileName = "AssignmentSO", menuName = "Scriptable Objects/AssignmentSO")]
public class AssignmentSO : ScriptableObject
{
    public AssignmentSO[] _nextAssignment;
    public string AssignmentName;
    public Sprite Sprite;
    public AssignmentType assignmentType;
    public bool IsPolished;
    public AssignmentType polishType;

}
public enum AssignmentType
{
    Dev,
    Art,
    Sound
}
