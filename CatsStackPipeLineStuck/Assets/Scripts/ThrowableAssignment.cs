using System;
using DG.Tweening;
using UnityEngine;

public class ThrowableAssignment : MonoBehaviour, IThrowable
{
    [Header("References")]
    [SerializeField] Collider2D _collider; // Collider to detect landing
    [SerializeField] AssignmentSO _assignmentSO;
    [Header("Throw and drop Settings")]
    [SerializeField] private float _throwingDuration = 2; //
    [SerializeField] private LayerMask _stationMask;
    [SerializeField] private float _stationDetectionRadius = 0.5f; // Radius to detect stations when throwing
    [SerializeField] private float _stationSnapCooldown = 0.5f; // Cooldown to prevent snapping 2 colliders too quickly
    [SerializeField] private float _stationSnapTimer = 0f; // Cooldown to prevent snapping too quickly

    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] private PlayerCarry _carryingParent; // Parent transform to attach the carried object to
    [SerializeField, ReadOnly] private float _throwingTimer;
    public AssignmentSO Data { get => _assignmentSO; set => _assignmentSO = value; }
    private Tween _throwTween;
    private bool _canSnapped = false; // To prevent snapping multiple times in a single throw
    private void Update()
    {
        if (!_canSnapped)
        {
            _stationSnapTimer -= Time.deltaTime; // Decrease cooldown timer
            if (_stationSnapTimer < 0)
            { 
                _stationSnapTimer = _stationSnapCooldown;
                _canSnapped = true; // Reset snapping state after cooldown
            }
        }
    }
    public void BeginThrow()
    {
        // Detach from carry parent, enable physics
        _carryingParent.ClearCarryable();
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
        .OnUpdate(() =>
        {
            if(TryHitStation(out Transform hitStation) != null) OnLanded();
        })
        .OnComplete(() => {
            _throwTween = null; 
        });   // clear handle when done
    }
    private Transform TryHitStation(out Transform hitStation)
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _stationDetectionRadius, _stationMask);
        Collider2D closest = null;
        float bestSqr = float.PositiveInfinity;
        Vector2 p = transform.position;

        foreach (var h in hits)
        {
            if (!h) continue;                 // safety
                                              // Skip triggers if you want only solid stations:
                                              // if (h.isTrigger) continue;

            Vector2 cp = h.ClosestPoint(p);   // nearest point on this collider
            float sqr = (cp - p).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                closest = h;
            }
        }
        hitStation = closest?.transform;
        return hitStation;
        //else, didnt hit a station, so do nothing
    }
    private void OnHitStation(Transform station)
    {
        if (_canSnapped)
        {
            Debug.Log("<color=lightblue>Snap to station</color>");
            transform.position = station.position;
            station.GetComponent<BasicStation>().InputAssignment(_assignmentSO);
            _canSnapped = false;
        }
    }
    public void OnLanded()
    {
        if (TryHitStation(out Transform hitStation) != null) OnHitStation(hitStation);
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
        if (TryHitStation(out Transform hitStation) != null) OnHitStation(hitStation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stationDetectionRadius);
    }
}

