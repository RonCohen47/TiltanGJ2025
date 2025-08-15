using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowableAssignment : MonoBehaviour, IThrowable
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] AssignmentSO _assignmentSO;
    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] private PlayerCarry _carryingParent; // Parent transform to attach the carried object to
    public AssignmentSO Data { get => _assignmentSO; set => _assignmentSO = value; }

    public void BeginThrow()
    {
        // Detach from carry parent, enable physics
        _carryingParent.ClearCarryable();
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
        if (_carryingParent != null)
        {
            _carryingParent.ClearCarryable(); // Clear any existing carryable if already attached
            Debug.LogWarning("Already attached to a parent. Detaching first.");
        }
        Debug.Log("<color=green>attached to player");
        _carryingParent = parent.GetComponent<PlayerCarry>();
        transform.SetParent(_carryingParent.transform, true);
        transform.position = attachPosition.position;
    }

    public void Detach()
    {
        _carryingParent = null;
        transform.SetParent(null, true);
    }
}
