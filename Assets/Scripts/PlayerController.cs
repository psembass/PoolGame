using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject dot;
    [SerializeField]
    private float dotDistance = 0.5f;
    [SerializeField]
    private float dotMaxDistance = 6f;
    [SerializeField]
    private float forceMax = 10f;
    // todo Common object pool for all objects?
    private List<GameObject> dots = new List<GameObject>();

    private Rigidbody ballRb;
    private bool isDragging = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        // Init dots pool
        int dotsCount = (int) (dotMaxDistance / (dotDistance + dot.transform.localScale.x));
        for (int i = 0; i < dotsCount; i++)
        {
            GameObject newDot = Instantiate(dot, transform.position, Quaternion.identity, transform);
            newDot.SetActive(false);
            dots.Add(newDot);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // On mouse down, begin drag
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Vector3 worldPosition = GetMouseWorldPosition();
            Vector3 startPoint = transform.position;
            float distance = Mathf.Min(Vector3.Distance(startPoint, worldPosition), dotMaxDistance);
            float force = forceMax * (distance / dotMaxDistance);
            Vector3 dir = (startPoint - worldPosition).normalized;
            Debug.Log("Add force to direction: " + dir);
            ballRb.AddForce(dir * force, ForceMode.Impulse);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // todo replace with raycast for 3D camera?
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.y = transform.localScale.y / 2; // At center of the ball
        return worldPosition;
    }

    private void UpdateDots()
    {
        // Hide existing dots
        foreach (GameObject child in dots)
        {
            child.SetActive(false);
        }
        if (isDragging)
        {
            // Where to place the dots - direction
            Vector3 mousePosition = GetMouseWorldPosition();
            Vector3 startPoint = transform.position;
            Vector3 dir = (startPoint - mousePosition).normalized;
            // Calculate place for first dot
            Vector3 dotPosition = startPoint - dir * (dotDistance + transform.localScale.y/2);
            dotPosition.y = transform.localScale.y / 2; // At center of the ball
            // Create new dots between start point and world position
            float distance = Vector3.Distance(startPoint, dotPosition);
            // Draw dots until we reach the mouse position or max distance
            float maxDistance = Mathf.Min(Vector3.Distance(startPoint, mousePosition), dotMaxDistance);
            while (distance < maxDistance)
            {
                ShowDot(dotPosition);
                dotPosition = dotPosition - dir * dotDistance;
                dotPosition.y = transform.localScale.y / 2; // At center of the ball
                distance = Vector3.Distance(startPoint, dotPosition);
            }
        }
    }

    private void ShowDot(Vector3 dotPosition)
    {
        foreach (GameObject dot in dots)
        {
            if (!dot.activeInHierarchy)
            {
                dot.transform.position = dotPosition;
                dot.SetActive(true);
                return;
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Hole"))
        {
            // todo Add UI and game over logic
            Debug.Log("You hit the hole");
            Destroy(gameObject);
        }
    }
}