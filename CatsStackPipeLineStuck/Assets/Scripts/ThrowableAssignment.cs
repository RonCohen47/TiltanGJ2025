using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowableAssignment : MonoBehaviour, IThrowable
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] AssignmentSO _assignmentSO;
    [SerializeField] private float _throwingCooldown = 2; //
    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] private PlayerCarry _carryingParent; // Parent transform to attach the carried object to
    [SerializeField, ReadOnly] private float _throwingTimer;
    [SerializeField, ReadOnly] bool _hasThrown = false;
    public AssignmentSO Data { get => _assignmentSO; set => _assignmentSO = value; }

    private void Update()
    {
        if (_hasThrown)
        {
            _throwingTimer += Time.deltaTime;
            if (_throwingCooldown <= _throwingTimer)
            {
                _throwingTimer = 0; // Reset the timer if cooldown is not reached
                _hasThrown = false;
                OnLanded();
            }
        }
    }
    public void BeginThrow()
    {
        // Detach from carry parent, enable physics
        _carryingParent.ClearCarryable();
        _hasThrown = true;
    }

    public void Throw(Vector2 direction, float force)
    {
        // Ensure normalized direction; add slight arc
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void OnLanded()
    {
        Debug.Log("on lended");
        _rb.angularVelocity = 0f; // Reset angular velocity to prevent spinning
        _rb.linearVelocity = Vector2.zero; // Reset linear velocity to stop movement
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
        Debug.Log("<color=green>attached to player</color>");
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
