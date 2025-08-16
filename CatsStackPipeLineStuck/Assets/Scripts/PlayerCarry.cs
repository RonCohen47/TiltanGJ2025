using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.InputSystem;
using static AssignmentData;

public class PlayerCarry : MonoBehaviour
{
    // carry generic assignment or ????. if its assignment, it will have assignmentDataSO which conains the id of the assignment.
    //this way, the player can carry any assignment, and the assignment will be identified by its id when is used, with event of assignment acomplished.
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    [SerializeField] private Transform _attachPos;
    [SerializeField] private SpineAnimationController _controller;
    [SerializeField] private SpriteRenderer _circleRenderer; // Circle renderer to indicate carrying state
    [Header("Carry Settings")]
    [SerializeField] private float _throwForce;
    [SerializeField] private float _pickupRadius = 0.5f;
    [SerializeField] private float _stationDetectionRadius = 0.5f; // Radius to detect stations when throwing
    [SerializeField] private LayerMask _carryableMask;
    [SerializeField] private LayerMask _stationMask;


    [Header("Read-Only Params")]
    [SerializeField, ReadOnly] private bool _isCarrying = false;
    [SerializeField, ReadOnly] private bool _isProcessing = false;
    [SerializeField, ReadOnly] private Vector2 _aimThrowInput;
    [SerializeField, ReadOnly] private float _processTimer = 0;
    [SerializeField, ReadOnly] private float _processDuration = 0;
    [SerializeField, ReadOnly] private ProcessStation _processedStation;
    [SerializeField, ReadOnly] private AssignmentType _playerType;
    bool _isAiming = false;
    private ICarryable _carryable;
    public void Init(AssignmentType assignmentType)
    {
        _playerType = assignmentType;
        switch (_playerType)
        {
            case AssignmentType.Dev: // Default
                _controller.SetSkin("Yasha");
                _circleRenderer.color = Color.green;
                break;
            case AssignmentType.Art: // Carrying
                _controller.SetSkin("Beau");
                _circleRenderer.color = Color.blue;

                break;
            case AssignmentType.Sound: // Processing
                _controller.SetSkin("Sirius");
                _circleRenderer.color = Color.red;

                break;
            default:
                _controller.SetSkin("Yasha");

                break;

        }
    }
    private void Update()
    {
        if (_isProcessing)
        {
            _processTimer += Time.deltaTime; // Decrease cooldown timer
            if (_processTimer >= _processDuration)
            {
                _processTimer = 0;
                _isProcessing = false; // Reset snapping state after cooldown
                OnProcessEnded();
            }
        }
    }
    public void TryPick(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            PickRelease();
        }
    }
    
    void PickRelease()
    {
        if(!_isCarrying)
            Pick();
        else
            Release();

        _controller.ToggleHoldItem(_isCarrying);

    }
    private void Pick()
    {
        Collider2D carryableColllider = Physics2D.OverlapCircle(transform.position, _pickupRadius, _carryableMask);
        if (!_isCarrying)//not carring
        {
            if (_carryable == null)//not carring
                if (carryableColllider != null && carryableColllider.TryGetComponent(out ICarryable carryable))
                {
                    // Try to get the ICarryable component from the collider
                    _carryable = carryable;
                    _isCarrying = true; // If we found a carryable object, we are carrying it, otherwise not
                    _carryable?.AttachToParent(transform, _controller);//attach to parent if picked, detach if released
                    //Debug.Log($"{(_isCarrying ? "picked" : "released")} an item!");
                    ThrowableAssignment throwable = (_carryable as ThrowableAssignment);
                    if(throwable.Station is BriefStation)
                    {
                        Debug.Log("spawn brief");
                        (throwable.Station as BriefStation).SpawnBrief();//spawn new brief.
                    }
                }
                else
                    Debug.LogWarning("No carryable object found in range or the object does not implement ICarryable interface.");
            else Debug.Log("there is a carryable");
        }
    }
    private void Release()
    {
        if (_isCarrying)
        {
            if (_carryable == null)
                Debug.LogWarning("carrying but the item is null");
            _carryable.Detach(); // If we are carrying something, detach it
            _carryable = null; // Clear the carryable reference
            _isCarrying = false; // Update the carrying state
            //Debug.Log($"{(_isCarrying ? "picked" : "released")} an item!");
        }
    }
    public void ThrowProcess(InputAction.CallbackContext ctx)
    {
        CoopPlayerController player = GetComponent<CoopPlayerController>();
        if (ctx.started)
        {
            if (_isCarrying)//carrying and 
            {
                LockPlayerInput();
            }
            else if(!_isProcessing && TryHitStation(out ProcessStation hitStation))
            {
                Debug.Log("processing");
                _controller.ToggleWorking(true);
                LockPlayerInput();
                ProcessStation processStation = hitStation;
                if(processStation.IsOccupied)
                {
                    _isProcessing = true;//started processing
                    _processDuration = processStation.InteractionDuration;
                }
                _processedStation = processStation;
            }
        }
        if (ctx.canceled)
            if (_isProcessing)
            {
                _controller.ToggleWorking(false);
                _isProcessing = false; // Stop processing when the action is canceled
            }
            else if (_isCarrying && _carryable is IThrowable)
            {
                if (!_isAiming)
                    _aimThrowInput = transform.up;
                IThrowable throwable = _carryable as IThrowable;
                throwable.BeginThrow();
                throwable.Throw(GetComponent<CoopPlayerController>().ThrowDirection, _throwForce);
                Debug.Log($"<color=green>Throwing {_carryable} in direction {_aimThrowInput} with force {_throwForce}</color>");
                _isCarrying = false;

                _controller.ToggleHoldItem(_isCarrying);
                player.InputLocked = false;
            }
        
    }
    private void LockPlayerInput()
    {
        CoopPlayerController player = GetComponent<CoopPlayerController>();
        player.InputLocked = true; // Lock input to prevent movement while throwing
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // Reset velocity to prevent unwanted movement
    }
    public void ClearCarryable()
    {
        if (_carryable != null)
        {
            _carryable.Detach();
            _carryable = null;
            _isCarrying = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }
    private bool TryHitStation(out ProcessStation hitStation)
    {
        Transform origin = transform;
        Vector3 originPos = origin.position; // Get the position of the carrying parent or this object if not carried
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin.position, _stationDetectionRadius, _stationMask);
        //if(hits.Length > 0)
        //    Debug.Log("found stations");
        //else
        //    Debug.Log("no stations found");
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

        hitStation = closest?.GetComponent<ProcessStation>();
        return hitStation != null;
        //else, didnt hit a station, so do nothing
    }
    private void OnProcessEnded()
    {
        
        Pick();
        IThrowable throwable = _carryable as IThrowable;
        ThrowableAssignment processedAssignment = (throwable as ThrowableAssignment);
        processedAssignment.SetAssignment(_processedStation.OutputAssignment(_playerType));
        Debug.Log("ended process");
    }
}
