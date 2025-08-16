using System.Linq;
using UnityEngine;

public abstract class Station : MonoBehaviour
{
    public AssignmentSO currentAssignment;
    [ReadOnly] public bool PlayerInRange;
    [ReadOnly] public bool IsOccupied;
    [SerializeField] private LayerMask playerLayerMask;

    public AssignmentSO OutputAssignment(AssignmentType playerType)//AFTER Processing
    {
        if (currentAssignment != null)
        {
            currentAssignment._nextAssignment.FirstOrDefault(c => c.assignmentType == playerType);
            return currentAssignment;
        }
        return null;
    }

    public void InputAssignment(AssignmentSO assignment)
    {
        if (currentAssignment == null)
        {
            currentAssignment = assignment;
            IsOccupied = true;
        }
    }
    public void ClearInput()
    {
        currentAssignment = null;
        IsOccupied = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayerMask.value & (1 << collision.gameObject.layer)) != 0)
            PlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
            PlayerInRange = false;
    }

}
