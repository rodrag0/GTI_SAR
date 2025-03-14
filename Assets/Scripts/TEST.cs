using UnityEngine;

public class CubeForwardMovement : MonoBehaviour
{
    // Speed at which the cube moves forward.
    public float speed = 5f;
    
    // Reference to the Rigidbody component.
    private Rigidbody rb;

    void Start()
    {
        // Try to get the Rigidbody attached to this GameObject.
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found! Please add a Rigidbody component to the cube.");
        }
    }

    // Use FixedUpdate for physics updates.
    void FixedUpdate()
    {
        if (rb != null)
        {
            // Calculate the new horizontal velocity in the cube's forward direction.
            // Preserve the current vertical velocity (rb.velocity.y) so gravity continues to affect the cube.
            Vector3 newVelocity = transform.forward * speed;
            newVelocity.y = rb.linearVelocity.y;
            
            // Apply the new velocity.
            rb.linearVelocity = newVelocity;
        }
    }
}
