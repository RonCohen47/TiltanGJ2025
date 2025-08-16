using UnityEngine;

public abstract class Station : MonoBehaviour
{
    [SerializeField, ReadOnly] protected AssignmentSO currentAssignment;
    [SerializeField, ReadOnly] protected bool PlayerInRange;
    [SerializeField, ReadOnly] public bool IsOccupied;
    [SerializeField] private LayerMask playerLayerMask;

    public AssignmentSO OutputAssignment()
    {
        if (currentAssignment != null)
        {
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
