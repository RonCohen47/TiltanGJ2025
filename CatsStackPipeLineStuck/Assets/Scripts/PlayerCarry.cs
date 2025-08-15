using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    // carry generic assignment or ????. if its assignment, it will have assignmentDataSO which conains the id of the assignment.
    //this way, the player can carry any assignment, and the assignment will be identified by its id when is used, with event of assignment acomplished.
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    ICarryable _carryable;
    private bool _isCarrying = false;
    [SerializeField] private float _throwForce;
    [SerializeField] private LayerMask _carryableMask;
    [SerializeField] private float _pickupRadius = 0.5f;
    [SerializeField] private Vector2 _aimThrowInput;
    bool _isAiming = false;
    public void TryPick(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            PickRelease();
        }
    }
    void PickRelease()
    {
        _carryable = !_isCarrying ? Physics2D.OverlapCircle(transform.position, _pickupRadius, _carryableMask) as ICarryable : null;//_carriableMask is always ICarryable
        _isCarrying = !_isCarrying;//togle carry state
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
        Debug.Log("thrown an item!");
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }
}
