using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    // carry generic assignment or ????. if its assignment, it will have assignmentDataSO which conains the id of the assignment.
    //this way, the player can carry any assignment, and the assignment will be identified by its id when is used, with event of assignment acomplished.
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    [SerializeField] private Transform _attachPos;
    [Header("Carry Settings")]
    [SerializeField] private bool _isCarrying = false;
    [SerializeField] private float _throwForce;
    [SerializeField] private float _pickupRadius = 0.5f;
    [SerializeField] private LayerMask _carryableMask;
    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] private Vector2 _aimThrowInput;
    
    bool _isAiming = false;
    private ICarryable _carryable;
    public void TryPick(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            PickRelease();
        }
    }
    void PickRelease()
    {
        Collider2D carryableColllider = Physics2D.OverlapCircle(transform.position, _pickupRadius, _carryableMask);
        if (_carryable == null && carryableColllider != null && carryableColllider.TryGetComponent(out ICarryable carryable))
        {
             // Try to get the ICarryable component from the collider
            _carryable = carryable;
            _isCarrying = true; // If we found a carryable object, we are carrying it, otherwise not
            _carryable?.AttachToParent(transform, _attachPos);//attach to parent if picked, detach if released
        }
        else if(_carryable != null && _isCarrying)
        {
            _carryable.Detach(); // If we are carrying something, detach it
            _carryable = null; // Clear the carryable reference
        }
        Debug.Log($"{(_isCarrying ? "picked" : "released")} an item!");
    }
    public void Aim(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
            _isAiming = true;
        if (ctx.performed)
            _aimThrowInput = ctx.ReadValue<Vector2>();
        else if (ctx.canceled)
        {
            _isAiming = false;
            _aimThrowInput = Vector2.zero; // Reset aim direction when not aiming
        }
    }
    public void Throw(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled && _isCarrying)
            if(_carryable is IThrowable)
            {
                if(!_isAiming)
                    _aimThrowInput = transform.up; 
                IThrowable throwable = _carryable as IThrowable;
                throwable.BeginThrow();
                throwable.Throw(_aimThrowInput.normalized, _throwForce);
                _isCarrying = false;
            }

    }
    public void ClearCarryable()
    {
        if (_carryable != null)
        {
            _carryable.Detach();
            _carryable = null;
            _isCarrying = false;
            Debug.Log("Carryable was stolen");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }
}
