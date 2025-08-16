using System;
using DG.Tweening;
using UnityEngine;

public class ThrowableAssignment : MonoBehaviour, IThrowable
{
    [Header("References")]
    [SerializeField] Collider2D _collider; // Collider to detect landing
    [Header("Throw and drop Settings")]
    [SerializeField] private float _throwingDuration = 2; //
    [SerializeField] private LayerMask _stationMask;
    [SerializeField] private float _stationDetectionRadius = 0.5f; // Radius to detect stations when throwing
    [SerializeField] private float _stationSnapCooldown = 0.5f; // Cooldown to prevent snapping 2 colliders too quickly
    [SerializeField] private float _stationSnapTimer = 0f; // Cooldown to prevent snapping too quickly

    public AssignmentSO _assignmentSO;
    [Header("Read-Only Params")]
    [ReadOnly] public Station _processStation;
    [SerializeField, ReadOnly] private PlayerCarry _carryingParent; // Parent transform to attach the carried object to
    [SerializeField, ReadOnly] private float _throwingTimer;
    public AssignmentSO Data { get => _assignmentSO; set => _assignmentSO = value; }
    private Tween _throwTween;
    private bool _canSnapped = false; // To prevent snapping multiple times in a single throw
    private bool _isHeld = false;
    private SpineAnimationController _AttachedController;


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

        if (_isHeld && _AttachedController != null) transform.position = _AttachedController.GetBonePosition();
    }
    public void SetAssignment(AssignmentSO assignment)
    {
        _assignmentSO = assignment;
        if (_assignmentSO != null)
        {
            // Optionally, set the sprite or other properties based on the assignment
            // e.g., GetComponent<SpriteRenderer>().sprite = _assignmentSO.Sprite;
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
            if(TryHitStation(out Station hitStation) != null) OnLanded();
        })
        .OnComplete(() => { _throwTween = null; });   // clear handle when done
    }
    private Station TryHitStation(out Station hitStation)
    {
        Transform origin = _carryingParent == null ? transform : _carryingParent.transform;
        Vector3 originPos = origin.position; // Get the position of the carrying parent or this object if not carried
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin.position, _stationDetectionRadius, _stationMask);
        Vector2 p = transform.position;

        Collider2D closest = null;
        float shortestDistance = float.PositiveInfinity;

        foreach (var h in hits)
        {
            if (!h) continue;
            // if (h.isTrigger) continue; // uncomment if you want solids only

            float newDistance = Vector2.Distance(originPos, h.transform.position);  // squared distance origin -> collider
            if (newDistance < shortestDistance)
            {
                shortestDistance = newDistance;
                closest = h;
            }
        }

        hitStation = closest?.GetComponent<Station>();
        return hitStation;
        //else, didnt hit a station, so do nothing
    }
    private void OnHitStation(Station station)
    {
        if (_canSnapped)
        {
            //Debug.Log("<color=lightblue>Snap to station</color>");
            transform.position = station.transform.position;
            Station processStation = station.GetComponent<Station>();
            processStation.InputAssignment(_assignmentSO);
            _processStation = processStation;


            _canSnapped = false;
        }
    }
    public void OnLanded()
    {
        if (TryHitStation(out Station hitStation) != null) OnHitStation(hitStation);
        _throwTween?.Kill(); // or _throwTween?.Kill(true);
        _throwTween = null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnLanded();
    }

    public void AttachToParent(Transform parent,SpineAnimationController AttachBone)
    {
        if (_carryingParent != null)
        {
            _carryingParent.ClearCarryable(); // Clear any existing carryable if already attached
            Debug.LogWarning("Already attached to a parent. Detaching first.");
            _throwTween?.Kill();
            _throwTween = null;
        }
        //Debug.Log("<color=green>attached to player</color>");
        _collider.enabled = false;
        _carryingParent = parent.GetComponent<PlayerCarry>();
        transform.SetParent(_carryingParent.transform, true);
        _AttachedController = AttachBone; // Snap to the attach position
        _isHeld = true;
        _processStation?.ClearInput();
    }

    public void Detach()
    {
        if (TryHitStation(out Station hitStation) != null) OnHitStation(hitStation);
        _carryingParent = null;
        transform.SetParent(null, true);
        _isHeld = false;
        _AttachedController = null;
        _collider.enabled = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stationDetectionRadius);
    }
}

