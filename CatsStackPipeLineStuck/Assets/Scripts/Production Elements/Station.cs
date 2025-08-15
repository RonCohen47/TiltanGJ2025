using UnityEngine;

public abstract class Station : MonoBehaviour
{
    protected AssignmentSO currentAssignment;
    protected bool PlayerInRange;
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            PlayerInRange = true;
        }
      

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            // other is in playerLayerMask
        }
    }

}
