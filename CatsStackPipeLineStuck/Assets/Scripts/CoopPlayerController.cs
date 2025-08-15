using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class CoopPlayerController : MonoBehaviour
{
    [Header("Componenet References")]
    [SerializeField] private Rigidbody2D _rb;
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField, Range(0,20)] float _deceleration = 6f; // How quickly the movement slows down
    [SerializeField, Range(0f, 20f)] private float _acceleration = 10f;
    [SerializeField,Range(0, 0.1f)] private float _stopEpsilon = 0.05f;  // snap to 0 below this
    [SerializeField, Range(-1f, 1f)] private float _flipDotThreshold = -0.2f; // <= means “opposite enough”

    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private Vector2 _lastMoveInput;
    [SerializeField] private Vector2 _currentVelocity = Vector3.zero; // Tracks current velocity
    // Called automatically by PlayerInput (Send Messages) for "Move"
    void Update()
    {
        Movement();
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            _moveInput = Vector2.zero;
            return;
        }
        _moveInput = ctx.ReadValue<Vector2>();
    }
    private void Movement()
    {
        Vector2 movementVector = _moveInput * _moveSpeed;
        if (_moveInput.magnitude > 0)
        {
            if (_currentVelocity.sqrMagnitude > 0.0001f)
            {
                float dot = Vector2.Dot(_currentVelocity.normalized, _moveInput.normalized);
                if (dot <= _flipDotThreshold)
                {
                    // Hard brake on flip so you don't “fight” old momentum
                    _currentVelocity = Vector2.zero;
                }
            }
            _currentVelocity += _moveInput.normalized * _acceleration * Time.deltaTime;
        }
        else
        {
            //Decelerate toward 0 and snap to 0 near rest
            if (_currentVelocity.magnitude <= _stopEpsilon)
                _currentVelocity = Vector2.zero;
            else
                _currentVelocity += -_lastMoveInput.normalized * _deceleration * Time.deltaTime;
        }
        // Apply velocity to position
        transform.position += new Vector3(_currentVelocity.x, _currentVelocity.y) * Time.deltaTime;
        if(_moveInput != Vector2.zero)
            _lastMoveInput = _moveInput;
    }
}
