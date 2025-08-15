using UnityEngine;

public class AssignmentData : MonoBehaviour
{
    private AssignmentType Type;
    private AssignmentStatus Status;
    private PolishType Polish;


    public void Initialize(AssignmentType type)
    {
        Type = type;
        Status = AssignmentStatus.Empty;
        Polish = PolishType.None;

    }
    public enum AssignmentStatus
    {
        Empty,
        Basic,
        Polish,
        Polished
    }
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
