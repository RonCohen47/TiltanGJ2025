using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class CoopPlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private Rigidbody2D _rb;
    private Vector2 _move;

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
    }

    // Called automatically by PlayerInput (Send Messages) for "Move"
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            _move = Vector2.zero;
            return;
        }
        _move = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 dir = new Vector2(_move.x, _move.y);
        Vector2 target =  _rb.position + dir * (_moveSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(target);
    }
}
