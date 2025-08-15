using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    [SerializeField] private Vector2 _minMaxSpeed;
    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] private Vector2 _moveInput;
    [SerializeField, ReadOnly] private Vector2 _lastMoveInput;
    [SerializeField, ReadOnly] private Vector2 _currentVelocity = Vector3.zero; // Tracks current velocity
    [SerializeField, ReadOnly] private List<Collider2D> _touchedColliders;//tracks colliders currently being touched by the player

    void Update()
    {
        Movement();
        RotatePlayer();
        
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            _moveInput = Vector2.zero;
            return;
        }
        _moveInput = ctx.ReadValue<Vector2>();
        if (_moveInput.magnitude < 0.35f)
            _moveInput = Vector2.zero; // Ignore small inputs to prevent jitter
    }
    private void Movement()
    {
        if(_touchedColliders.Count > 0)
        {
            // If touching a collider, stop moving
            _currentVelocity = Vector2.zero;
            return;
        }
        if (_moveInput.magnitude > 0)
        {
            _currentVelocity += _moveInput.normalized * _acceleration * Time.deltaTime;
            float clampedMag = Mathf.Clamp(_currentVelocity.magnitude, _minMaxSpeed.x, _minMaxSpeed.y);
            Vector2 desiredVel = _moveInput.normalized * clampedMag;
            _currentVelocity = desiredVel;
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

    private void RotatePlayer()
    {
        bool rotateLeft = _moveInput.x < 0f;
        if (_moveInput == Vector2.zero)
        {
            rotateLeft = _lastMoveInput.x < 0f;
        }

        Vector3 scale = transform.localScale;
        if (rotateLeft && scale.x > 0f)
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (!rotateLeft && scale.x < 0f)
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _currentVelocity = Vector2.zero;
        _touchedColliders.Add(collision.collider);
        ContactPoint2D contactPoint2D = collision.GetContact(0);
        pushOutsideOfWalls(contactPoint2D.point);
    }
    private void pushOutsideOfWalls(Vector2 hitWallPoint)
    {
        Vector2 pushOutside = (Vector2)transform.position - hitWallPoint;
        pushOutside = pushOutside.normalized;
        transform.position += (Vector3)pushOutside.normalized * 0.05f;
        _rb.angularVelocity = 0f; // Reset angular velocity to prevent spinning
        _rb.linearVelocity = Vector2.zero; // Reset linear velocity to stop movement
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("out off wall");
        _touchedColliders.Remove(collision.collider);
        _rb.angularVelocity = 0f; // Reset angular velocity to prevent spinning
        _rb.linearVelocity = Vector2.zero; // Reset linear velocity to stop movement
    }
}
