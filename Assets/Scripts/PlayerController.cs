using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject dot;
    [SerializeField]
    private float dotDistance = 0.5f;
    [SerializeField]
    private float dotMaxDistance = 6f;
    [SerializeField]
    public float forceMax = 10f;
    [SerializeField]
    private List<Camera> cameras;
    [SerializeField]
    private InputActionAsset inputActions;
    private InputAction pointAction;
    private InputAction clickAction;
    private InputAction cameraAction;

    private Camera currentCamera;
    public Vector3 hitForce { get; set; } = Vector3.zero;
    public bool CanHit { get; set; } = true;
    private bool forceApplied = false;
    private List<GameObject> dots = new List<GameObject>();
    private IObjectPool<GameObject> dotsPool;

    private Rigidbody ballRb;
    private bool isDragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        // Init dots pool
        int maxDotsCount = (int)(dotMaxDistance / (dotDistance + dot.transform.localScale.x));
        dotsPool = new ObjectPool<GameObject>(
            () => Instantiate(dot, transform.position, Quaternion.identity, transform),
            dot => dot.SetActive(true),
            dot => dot.SetActive(false),
            dot => Destroy(dot),
            false,
            maxDotsCount,
            maxDotsCount
        );
        currentCamera = Camera.main;
        InitControls();
    }

    void InitControls()
    {
        inputActions.FindActionMap("Gameplay").Enable();
        pointAction = inputActions.FindAction("Point");
        clickAction = inputActions.FindAction("Click");
        cameraAction = inputActions.FindAction("Camera");
    }

    // Update is called once per frame
    void Update()
    {
        // On mouse down, begin drag
        if (clickAction.IsPressed() && CanHit)
        {
            Vector2 mousePosition = pointAction.ReadValue<Vector2>();
            Ray ray = currentCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                }
            }
        }

        // Show dots to indicate strength and direction
        UpdateDots();

        // On mouse release hit the ball
        if (!clickAction.IsPressed() && isDragging)
        {
            isDragging = false;
            Vector3 worldPosition = GetMouseWorldPosition();
            Vector3 startPoint = transform.position;
            float distance = Mathf.Min(Vector3.Distance(startPoint, worldPosition), dotMaxDistance);
            float force = forceMax * (distance / dotMaxDistance);
            Vector3 dir = (startPoint - worldPosition).normalized;
            hitForce = dir * force;
        }

        // Switch camera on key press
        if (cameraAction.WasPressedThisFrame())
        {
            SwitchCamera();
        }
    }

    void FixedUpdate()
    {
        forceApplied = false;
        if (hitForce != Vector3.zero)
        {
            Debug.Log("Applying hit force: " + hitForce);
            ballRb.AddForce(hitForce, ForceMode.Impulse);
            hitForce = Vector3.zero; // Reset hit force after applying
            CanHit = false; // Disable hitting until all balls are stopped
            forceApplied = true; // velocity will be zero until next frame
            Debug.Log("Player made a hit");
        }
    }

    public bool IsMoving()
    {
        bool isMoving = forceApplied || ballRb.linearVelocity.sqrMagnitude > 0.01f;
        if (!isMoving)
        {
            // Stop if movement velocity is below threshold
            StopMovement();
        }
        return isMoving;
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(-10, 0.5f, 4);
        StopMovement();
    }

    private void StopMovement()
    {
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.Sleep();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = pointAction.ReadValue<Vector2>();
        Ray ray = currentCamera.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.localScale.y / 2, 0));
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            point.y = transform.localScale.y / 2; // Set y to center of the ball
            return point;
        }
        return Vector3.zero;
    }

    private void UpdateDots()
    {
        int dotsNeeded = 0;
        if (isDragging)
        {
            // Where to place the dots - direction
            Vector3 mousePosition = GetMouseWorldPosition();
            Vector3 startPoint = transform.position;
            Vector3 dir = (startPoint - mousePosition).normalized;
            Vector3 ballSurface = startPoint - dir * transform.localScale.y / 2;
            // Count how many dots we need
            float distance = Mathf.Min(Vector3.Distance(ballSurface, mousePosition), dotMaxDistance);
            dotsNeeded = (int) (distance / dotDistance);
            if (dots.Count < dotsNeeded)
            {
                while (dots.Count < dotsNeeded)
                {
                    // Get new dot from pool
                    GameObject newDot = dotsPool.Get();
                    dots.Add(newDot);
                }
            }
            // Calculate place for first dot and position all dots
            Vector3 firstDotPos = ballSurface - dir * dotDistance;
            for (int i = 0; i < dotsNeeded; i++)
            {
                GameObject dotObj = dots[i];
                dotObj.transform.position = firstDotPos - dotDistance * i * dir;
            }
        }
        // Release unused dots
        for (int i = dots.Count - 1; i >= dotsNeeded; i--)
        {
            GameObject dotToRelease = dots[i];
            dotsPool.Release(dotToRelease);
            dots.RemoveAt(i);
        }
    }

    private void SwitchCamera()
    {
        if (cameras.Count == 0) return;
        // Disable current camera
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }
        // Get next camera in the list
        int nextIndex = (cameras.IndexOf(currentCamera) + 1) % cameras.Count;
        currentCamera = cameras[nextIndex];
        currentCamera.gameObject.SetActive(true);
    }
}