using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    [Tooltip("Gravity multiplier. 1 = default physics gravity, 2 = double, etc.")]
    public float gravityScale = 1f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        // We’ll apply gravity ourselves, so turn off built-in gravity.
        _rb.useGravity = false;  
    }

    private void FixedUpdate()
    {
        // Apply our own gravity each physics step.
        // Physics.gravity is typically (0, -9.81, 0).
        Vector3 customGravity = Physics.gravity * gravityScale;
        _rb.AddForce(customGravity, ForceMode.Acceleration);
    }
}
