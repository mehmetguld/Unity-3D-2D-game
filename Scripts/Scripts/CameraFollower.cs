using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollower : MonoBehaviour
{
    public enum CameraMode
    {
        Fixed,
        FollowPlayer,
        Waypoints
    }

    #region Settings
    [Header("General Settings")]
    public CameraMode currentMode = CameraMode.FollowPlayer;
    public Transform target;

    [SerializeField] private float smoothTime = 0.25f;
    private Vector3 _offset = new Vector3(0f, 0f, -10f);
    private Vector3 _velocity = Vector3.zero;

    [Header("Waypoint Settings")]
    public List<Transform> waypoints = new List<Transform>();
    public float forwardDuration = 3.0f;  // Duration from first to last waypoint
    public float backwardDuration = 5.0f; // Duration from last to first waypoint
    public Ease forwardEase = Ease.Linear;   // Forward movement ease
    public Ease backwardEase = Ease.Linear;  // Backward movement ease
    [SerializeField] private bool shouldFollowAfterWaypoints = true; 
    [SerializeField] private bool shouldLoopWaypoints = false;      

    [Header("Boundary Settings")]
    public bool useBoundaries = true;
    public bool showBoundaryGizmos = true;
    public Color boundaryColor = Color.red;
    public float boundaryLineWidth = 2f;
    public Collider2D boundaryCollider; 

    
    private float _minX = -10f;
    private float _maxX = 10f;
    private float _minY = -5f;
    private float _maxY = 5f;

    [Header("Look Ahead Settings")]
    public bool useLookAhead = true;
    public float lookAheadDistance = 2f;
    public float lookAheadSmoothTime = 0.1f;
    private Vector3 _lookAheadVelocity = Vector3.zero;
    private Vector3 _lookAheadPos = Vector3.zero;

    [Header("Gizmo Settings")]
    public bool showGizmos = true;
    public float waypointSize = 0.5f;
    public Color forwardPathColor = Color.blue;
    public Color waypointColor = Color.yellow;

    [Header("Fixed Settings")]
    public Transform fixedTarget;
    #endregion

    #region Private Variables
    private Sequence _waypointSequence;
    private bool _waypointsModeActive = false;
    private Camera _cameraComponent;
    #endregion

    private void Awake()
    {
        _cameraComponent = GetComponent<Camera>();

     
        if (boundaryCollider != null)
        {
            UpdateBoundariesFromCollider();
        }
    }

    private void FixedUpdate()
    {
       
        if (boundaryCollider != null && useBoundaries)
        {
            
            UpdateBoundariesFromCollider();
        }

        ApplyCameraMode(currentMode);
    }

    #region Camera Modes
    private void FollowPlayer()
    {
        if (target != null)
        {
            var targetPosition = target.position + _offset;
            
            if (useLookAhead)
            {
                Vector2 moveDirection = Vector2.zero;

                Rigidbody2D rb2d = target.GetComponent<Rigidbody2D>();
                if (rb2d != null && rb2d.linearVelocity.sqrMagnitude > 0.1f)
                {
                    moveDirection = rb2d.linearVelocity.normalized;
                }
                else
                {
                    Rigidbody rb = target.GetComponent<Rigidbody>();
                    if (rb != null && new Vector2(rb.linearVelocity.x, rb.linearVelocity.y).sqrMagnitude > 0.1f)
                    {
                        moveDirection = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y).normalized;
                    }
                }

                if (moveDirection != Vector2.zero)
                {
                    Vector3 lookAheadTarget = new Vector3(moveDirection.x * lookAheadDistance, moveDirection.y * lookAheadDistance, 0);
                    _lookAheadPos = Vector3.SmoothDamp(_lookAheadPos, lookAheadTarget, ref _lookAheadVelocity, lookAheadSmoothTime);
                    targetPosition += _lookAheadPos;
                }
                else
                {
                    // Slowly reset look ahead position if not moving
                    _lookAheadPos = Vector3.SmoothDamp(_lookAheadPos, Vector3.zero, ref _lookAheadVelocity, lookAheadSmoothTime);
                    targetPosition += _lookAheadPos;
                }
            }

            // Calculate new position with smooth damping
            Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);

            // Apply boundaries if enabled
            if (useBoundaries)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, _minX, _maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, _minY, _maxY);
            }

            transform.position = newPosition;
        }
    }

    private void FixedPoint()
    {
        if (fixedTarget != null)
        {
            Vector3 targetPosition = fixedTarget.position + _offset;

            // Apply boundaries if enabled
            if (useBoundaries)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, _minX, _maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, _minY, _maxY);
            }

            transform.position = targetPosition;
        }
    }

    private void FollowWaypoints()
    {
        // Skip if no waypoints
        if (waypoints.Count == 0) return;

        // Start sequence if not active
        if (!_waypointsModeActive)
        {
            StartWaypointSequence();
        }
    }
    #endregion

    #region Waypoint System
    private void StartWaypointSequence()
    {
        // Skip if no waypoints
        if (waypoints.Count == 0) return;

        // Kill existing sequence
        if (_waypointSequence != null)
        {
            _waypointSequence.Kill();
        }

        // Collect forward positions
        Vector3[] forwardPositions = new Vector3[waypoints.Count];
        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector3 pos = waypoints[i].position + _offset;

            // Apply boundaries if enabled
            if (useBoundaries)
            {
                pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
                pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
            }

            forwardPositions[i] = pos;
        }

        // Set initial position
        transform.position = forwardPositions[0];

       
        _waypointSequence = DOTween.Sequence();

        // Forward path
        Tween forwardTween = transform.DOPath(forwardPositions, forwardDuration, PathType.Linear)
            .SetEase(forwardEase);

        // Add forward movement to sequence
        _waypointSequence.Append(forwardTween);

        if (shouldLoopWaypoints)
        {
            // Collect backward positions (reversed)
            Vector3[] backwardPositions = new Vector3[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 pos = waypoints[waypoints.Count - 1 - i].position + _offset;

                // Apply boundaries if enabled
                if (useBoundaries)
                {
                    pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
                    pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
                }

                backwardPositions[i] = pos;
            }

            // Backward path
            Tween backwardTween = transform.DOPath(backwardPositions, backwardDuration, PathType.Linear)
                .SetEase(backwardEase);

            // Add backward movement to sequence
            _waypointSequence.Append(backwardTween);

            // Loop if required
            _waypointSequence.SetLoops(-1, LoopType.Restart);
        }

        // Handle sequence completion
        _waypointSequence.OnComplete(() => {
            _waypointsModeActive = false;

            // Switch to player follow when complete if required
            if (shouldFollowAfterWaypoints)
            {
                SetModeFollowPlayer();
            }
            else
            {
                // Stay at the last position
                currentMode = CameraMode.Fixed;
                // You could also create a special "Idle" state if needed
            }
        });

        // Start sequence
        _waypointsModeActive = true;
        _waypointSequence.Play();
    }
    #endregion

    #region Camera Effects
    
    public void ShakeCamera(float duration = 0.3f, float strength = 0.3f)
    {
        transform.DOShakePosition(duration, strength);
    }

   
    public void ZoomCamera(float targetSize, float duration = 0.5f)
    {
        if (_cameraComponent != null && _cameraComponent.orthographic)
        {
            _cameraComponent.DOOrthoSize(targetSize, duration);
        }
    }

   
    public void ZoomCameraPerspective(float targetFOV, float duration = 0.5f)
    {
        if (_cameraComponent != null && !_cameraComponent.orthographic)
        {
            _cameraComponent.DOFieldOfView(targetFOV, duration);
        }
    }
    #endregion

    #region Mode Handling
    private void ApplyCameraMode(CameraMode mode)
    {
        switch (mode)
        {
            case CameraMode.Fixed:
                FixedPoint();
                break;
            case CameraMode.FollowPlayer:
                FollowPlayer();
                break;
            case CameraMode.Waypoints:
                FollowWaypoints();
                break;
            default:
                FollowPlayer();
                break;
        }
    }

    // Method to switch camera modes
    private void SetCameraMode(CameraMode newMode)
    {
       
        if (currentMode == CameraMode.Waypoints && newMode != CameraMode.Waypoints)
        {
            if (_waypointSequence != null)
            {
                _waypointSequence.Kill();
            }
            _waypointsModeActive = false;
        }

        currentMode = newMode;

        // Auto-start waypoints if switching to waypoint mode
        if (newMode == CameraMode.Waypoints)
        {
            StartWaypointSequence();
        }
    }

    // Convenience methods for mode switching
    public void SetModeFixed()
    {
        SetCameraMode(CameraMode.Fixed);
    }

    private void SetModeFollowPlayer()
    {
        SetCameraMode(CameraMode.FollowPlayer);
    }

    private void SetModeWaypoints()
    {
        SetCameraMode(CameraMode.Waypoints);
    }


    private void RestartWaypointPath()
    {
        if (currentMode == CameraMode.Waypoints)
        {
            _waypointsModeActive = false;
            StartWaypointSequence();
        }
        else
        {
            SetModeWaypoints();
        }
    }

    // Set waypoint behavior
    public void SetWaypointBehavior(bool follow, bool loop)
    {
        shouldFollowAfterWaypoints = follow;
        shouldLoopWaypoints = loop;

        // If already in waypoint mode, restart to apply changes
        if (currentMode == CameraMode.Waypoints && _waypointsModeActive)
        {
            RestartWaypointPath();
        }
    }

    
    public void SetBoundaries(bool useBounds, float minXVal, float maxXVal, float minYVal, float maxYVal)
    {
        useBoundaries = useBounds;
        _minX = minXVal;
        _maxX = maxXVal;
        _minY = minYVal;
        _maxY = maxYVal;
    }

    
    public void SetLookAhead(bool useLookAheadValue, float distance)
    {
        useLookAhead = useLookAheadValue;
        lookAheadDistance = distance;
    }

    
    private void UpdateBoundariesFromCollider()
    {
        if (boundaryCollider == null || _cameraComponent == null)
            return;

        Bounds bounds = boundaryCollider.bounds;

        float cameraHeight = 0;
        float cameraWidth = 0;

        if (_cameraComponent.orthographic)
        {
            cameraHeight = _cameraComponent.orthographicSize * 2;
            cameraWidth = cameraHeight * _cameraComponent.aspect;
        }
        else
        {
           
            float distance = Mathf.Abs(_offset.z);
            cameraHeight = 2.0f * distance * Mathf.Tan(_cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
            cameraWidth = cameraHeight * _cameraComponent.aspect;
        }

        
        _minX = bounds.min.x + cameraWidth * 0.5f;
        _maxX = bounds.max.x - cameraWidth * 0.5f;
        _minY = bounds.min.y + cameraHeight * 0.5f;
        _maxY = bounds.max.y - cameraHeight * 0.5f;

       
        if (_minX > _maxX)
        {
            float center = (bounds.min.x + bounds.max.x) * 0.5f;
            _minX = _maxX = center;
        }

        if (_minY > _maxY)
        {
            float center = (bounds.min.y + bounds.max.y) * 0.5f;
            _minY = _maxY = center;
        }
    }

    // Public method to update boundaries from collider at runtime
    private void RefreshBoundariesFromCollider()
    {
        if (boundaryCollider != null)
        {
            UpdateBoundariesFromCollider();
            Debug.Log($"Camera boundaries updated from collider: X({_minX} to {_maxX}), Y({_minY} to {_maxY})");
        }
        else
        {
            Debug.LogWarning("No boundary collider assigned!");
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
     
        if (showGizmos && waypoints.Count > 0)
        {
            // Draw waypoints
            foreach (var t in waypoints)
            {
                if (t == null) continue;

                // Waypoint spheres
                Gizmos.color = waypointColor;
                Gizmos.DrawSphere(t.position, waypointSize);
            }

            // Draw forward path lines
            Gizmos.color = forwardPathColor;
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] == null || waypoints[i + 1] == null) continue;
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        if (showBoundaryGizmos && useBoundaries)
        {
            Gizmos.color = boundaryColor;

       
            float zPos = transform.position.z;

      
            Gizmos.DrawLine(new Vector3(_minX, _maxY, zPos), new Vector3(_maxX, _maxY, zPos));

    
            Gizmos.DrawLine(new Vector3(_maxX, _maxY, zPos), new Vector3(_maxX, _minY, zPos));


            Gizmos.DrawLine(new Vector3(_maxX, _minY, zPos), new Vector3(_minX, _minY, zPos));


            Gizmos.DrawLine(new Vector3(_minX, _minY, zPos), new Vector3(_minX, _maxY, zPos));

      
            float cornerSize = waypointSize * 0.5f;
            Gizmos.DrawCube(new Vector3(_minX, _minY, zPos), Vector3.one * cornerSize);
            Gizmos.DrawCube(new Vector3(_minX, _maxY, zPos), Vector3.one * cornerSize);
            Gizmos.DrawCube(new Vector3(_maxX, _minY, zPos), Vector3.one * cornerSize);
            Gizmos.DrawCube(new Vector3(_maxX, _maxY, zPos), Vector3.one * cornerSize);
        }
    }
    #endregion

    #region Editor Tools
 
    [ContextMenu("Refresh Boundaries From Collider")]
    private void EditorRefreshBoundaries()
    {
        RefreshBoundariesFromCollider();
    }

  
    private void OnDrawGizmosSelected()
    {
        if (boundaryCollider != null && showBoundaryGizmos)
        {

            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Bounds bounds = boundaryCollider.bounds;
            Gizmos.DrawCube(bounds.center, bounds.size);

         
            if (_cameraComponent == null)
                _cameraComponent = GetComponent<Camera>();

            if (_cameraComponent != null)
            {
                UpdateBoundariesFromCollider();

                Gizmos.color = Color.green;
                Vector3 cameraAreaCenter = new Vector3((_minX + _maxX) * 0.5f, (_minY + _maxY) * 0.5f, transform.position.z);
                Vector3 cameraAreaSize = new Vector3(_maxX - _minX, _maxY - _minY, 0.1f);

                if (cameraAreaSize is { x: > 0, y: > 0 })
                    Gizmos.DrawWireCube(cameraAreaCenter, cameraAreaSize);
            }
        }
    }
    #endregion
}
