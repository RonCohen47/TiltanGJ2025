using UnityEngine;

[CreateAssetMenu(fileName = "AssignmentSO", menuName = "Scriptable Objects/AssignmentSO")]
public class AssignmentSO : ScriptableObject
{
    
    //id
    public string AssignmentName;
    public Sprite Sprite;
    public AssignmentType assignmentType;

    public enum AssignmentType
    {
        Dev,
        Art,
        Sound
    }

    public enum PolishType
    {
        None,
        Dev,
        Art,
        Sound
    }
}
