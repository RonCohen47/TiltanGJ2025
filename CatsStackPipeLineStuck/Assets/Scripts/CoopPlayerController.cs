using System;
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
    [SerializeField] private SpineAnimationController _animator;
    [SerializeField] private Rigidbody2D _rb;
    [Header("Movement Settings")]
    [SerializeField, Range(0,20)] float _deceleration = 6f; // How quickly the movement slows down
    [SerializeField, Range(0, 20)] private float _acceleration = 10f;
    [SerializeField, Range(0, 0.1f)] private float _stopEpsilon = 0.05f;  // snap to 0 below this
    [SerializeField, Range(-1, 1)] private float _flipDotThreshold = -0.2f; // <= means “opposite enough”
    [SerializeField, Range(0, 1)] private float _ySpeedModifier;
    [SerializeField] private Vector2 _minMaxSpeed;
    [Header("Dash Settings")]
    [SerializeField] float _dashPower;
    [SerializeField] float _dashDuration;
    [SerializeField] float _dashCooldown;
    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] float _dashInactiveTimer;
    [SerializeField, ReadOnly] float _dashDurationTimer;
    [SerializeField, ReadOnly] bool _hasDashed;
    [SerializeField, ReadOnly] bool _canDash = true; // Can dash if true, otherwise false
    [ReadOnly] public bool InputLocked;
    [SerializeField, ReadOnly] private Vector2 _dashDir; // Direction of the dash
    [SerializeField, ReadOnly] private Vector2 _moveInput;
    [SerializeField, ReadOnly] private Vector2 _lastMoveInputNot0;
    [SerializeField, ReadOnly] private Vector2 _lastFrameMoveInput;
    [SerializeField, ReadOnly] private Vector2 _currentVelocity = Vector3.zero; // Tracks current velocity
    public Vector2 ThrowDirection => _moveInput == Vector2.zero ? _lastMoveInputNot0 : _moveInput; // Use last input if current is zero
    private void Start()
    {
        _dashDurationTimer = _dashDuration; // Reset dash duration timer when not dashing
    }
    void Update()
    {
        HandleDash();
        RotatePlayer();
    }
    private void HandleDash()
    {
        // ---- Dash state update ----
        if (_hasDashed)
        {
            if (_dashInactiveTimer > 0f)
                _dashInactiveTimer -= Time.deltaTime;
            else
            {
                _dashInactiveTimer = _dashCooldown;
                _canDash = true; // Reset dash cooldown
            }
            if (_dashDurationTimer > 0f)
            {
                _dashDurationTimer -= Time.deltaTime;
                if (_dashDurationTimer <= 0f)
                {
                    _hasDashed = false; // Reset dash state
                    _dashDurationTimer = _dashDuration; // Start cooldown timer
                }
            }
        }
    }
    void FixedUpdate()
    {
        MovementFixed();
    }

    private void MovementFixed()
    {
        if (InputLocked)
            return;
        // Pick a dash direction: prefer live input, then last non-zero, then current velocity, fallback right
        Vector2 dashDir =
            (_moveInput != Vector2.zero ? _moveInput :
            (_lastMoveInputNot0 != Vector2.zero ? _lastMoveInputNot0 :
            (_currentVelocity.sqrMagnitude > 0.0001f ? _currentVelocity.normalized : Vector2.right)));
        _dashDir = dashDir.normalized;
        Vector2 _dashVelocity = _dashDir * _dashPower;

        _rb.linearVelocity = new Vector2(_currentVelocity.x, _currentVelocity.y * _ySpeedModifier);
        if (_moveInput.magnitude > 0f)
        {
            _currentVelocity += _moveInput.normalized * _acceleration * Time.fixedDeltaTime;
            _currentVelocity += _hasDashed ? (-_currentVelocity.normalized * _deceleration * Time.fixedDeltaTime) : Vector2.zero;
            float clampedMag = Mathf.Clamp(_currentVelocity.magnitude, _minMaxSpeed.x, _minMaxSpeed.y + (/*when dash get more max speed*/ _hasDashed ? 2f : 0));
            Vector2 desiredVel = _moveInput.normalized * clampedMag;
            _currentVelocity = desiredVel;
        }
        else
        {
            // Decelerate toward 0 and snap to 0 near rest
            if (_currentVelocity.magnitude <= _stopEpsilon)
                _currentVelocity = Vector2.zero;
            else
                _currentVelocity += (-_currentVelocity.normalized * _deceleration * Time.fixedDeltaTime);
        }
        // Apply velocity via Rigidbody2D (y scaled)
        _rb.linearVelocity = new Vector2(_currentVelocity.x, _currentVelocity.y * _ySpeedModifier) + (_hasDashed ? _dashVelocity : Vector2.zero);
        _animator.GroundedMovementAnimationUpdate(_rb.linearVelocity.magnitude);


        if (_moveInput != Vector2.zero)
            _lastMoveInputNot0 = _moveInput;
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _moveInput = ctx.ReadValue<Vector2>();
            if (_moveInput.magnitude < 0.35f)
                _moveInput = Vector2.zero; // Ignore small inputs to prevent jitter
        }
        else
            _moveInput = Vector2.zero;
    }//
    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        if (_canDash)
            _hasDashed = true;
    }
    private void RotatePlayer()
    {
        Vector3 newScaleRotated = transform.localScale;
        bool rotateLeft = _moveInput.x < 0f;
        //is idle?
        if (_moveInput == Vector2.zero)
        {
            //has moved before?
            if (_lastMoveInputNot0 != Vector2.zero)
            {
                rotateLeft = _lastMoveInputNot0.x < 0f;
                newScaleRotated.x = rotateLeft ? -1f : 1f;
            }
        }
        else//isnt idle
        {
            newScaleRotated.x = rotateLeft ? -1f : 1f;
        }
        transform.localScale = newScaleRotated;
    }
}
