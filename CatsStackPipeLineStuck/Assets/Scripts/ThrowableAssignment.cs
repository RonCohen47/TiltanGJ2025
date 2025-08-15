using System;
using DG.Tweening;
using UnityEngine;

public class ThrowableAssignment : MonoBehaviour, IThrowable
{
    [Header("References")]
    [SerializeField] Collider2D _collider; // Collider to detect landing
    [SerializeField] AssignmentSO _assignmentSO;
    [SerializeField] private float _throwingDuration = 2; //
    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] private PlayerCarry _carryingParent; // Parent transform to attach the carried object to
    [SerializeField, ReadOnly] private float _throwingTimer;
    [SerializeField, ReadOnly] bool _hasThrown = false;
    public AssignmentSO Data { get => _assignmentSO; set => _assignmentSO = value; }
    private Tween _throwTween;
    public void BeginThrow()
    {
        // Detach from carry parent, enable physics
        _carryingParent.ClearCarryable();
        _hasThrown = true;
        _collider.enabled = true; // Enable collider to detect landing
    }

    public void Throw(Vector2 direction, float force)
    {
        // cancel any previous throw tween
        _throwTween?.Kill();
        // Ensure normalized direction; add slight arc
        Vector3 end = transform.position + (Vector3)(direction.normalized * force);
        _throwTween = transform.DOMove(end, _throwingDuration)
        .SetEase(Ease.OutQuad)
        .OnComplete(() => {
            _throwTween = null;
            });   // clear handle when done
    }

    public void OnLanded()
    {
        _throwTween?.Kill(); // or _throwTween?.Kill(true);
        _throwTween = null;
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
        _collider.enabled = false;
        _carryingParent = parent.GetComponent<PlayerCarry>();
        transform.SetParent(_carryingParent.transform, true);
        transform.position = attachPosition.position; // Snap to the attach position
    }

    public void Detach()
    {
        _carryingParent = null;
        transform.SetParent(null, true);
        _collider.enabled = true;
    }
}
