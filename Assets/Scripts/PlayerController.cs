using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody ballRb;
    private bool isDragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
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

        // On mouse release, stop dragging
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Vector3 dir = GetHitDirection();
            Debug.Log("Add force to direction: " + dir);
            ballRb.AddForce(dir * 10f, ForceMode.Impulse);
        }
    }

    private Vector3 GetHitDirection()
    {
        Vector3 mousePosition = Input.mousePosition;
        // todo replace with raycast for 3D camera?
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 startPoint;
        Quaternion rotation;
        transform.GetPositionAndRotation(out startPoint, out rotation);
        Vector3 dir = (startPoint - worldPosition).normalized;
        dir.y = 0; // Only horizontal direction
        return dir;
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