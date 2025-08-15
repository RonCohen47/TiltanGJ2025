using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowableAssignment : MonoBehaviour, IThrowable
{
    private Transform _carryingParent; // Parent transform to attach the carried object to

    private Rigidbody2D _rb;

    [SerializeField] AssignmentSO _assignmentSO;
    public AssignmentSO Data { get => _assignmentSO; set => _assignmentSO = value; }

    public void BeginThrow()
    {
        // Detach from carry parent, enable physics
        Detach();
    }

    public void Throw(Vector2 direction, float force)
    {
        // Ensure normalized direction; add slight arc
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void OnLanded()
    {
        Debug.Log("on lended");
        // Snap/stop when landing on a valid surface if needed
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnLanded();
    }

    public void AttachToParent(Transform parent,Transform attachPosition)
    {
        Debug.Log("<color=green>attached to player");
        _carryingParent = parent;
        transform.SetParent(_carryingParent, true);
        transform.position = attachPosition.position;
    }

    public void Detach()
    {
        _carryingParent = null;
        transform.SetParent(null, true);
    }
}
