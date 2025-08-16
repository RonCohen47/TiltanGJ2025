using System.Linq;
using UnityEngine;

public abstract class Station : MonoBehaviour
{
    public AssignmentSO currentAssignment;
    [ReadOnly] public bool PlayerInRange;
    [ReadOnly] public bool IsOccupied;
    [ReadOnly] public ThrowableAssignment ThrowableAssignment;
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

    public void InputAssignment(AssignmentSO assignment, ThrowableAssignment throwableAssignment)
    {
        if (currentAssignment == null)
        {
            ThrowableAssignment = throwableAssignment; 
            currentAssignment = assignment;
            IsOccupied = true;
        }
    }
    public void ClearInput()
    {
        currentAssignment = null;
        IsOccupied = false;
        ThrowableAssignment = null;
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
