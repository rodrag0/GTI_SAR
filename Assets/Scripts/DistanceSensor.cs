using UnityEngine;

public class DistanceSensor : MonoBehaviour
{
    public float sensorRange = 10f;  // how far we check

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Cast the ray
        if (Physics.Raycast(ray, out hit, sensorRange))
        {
            // 'hit.distance' is how far away the object is
            float distanceToObj = hit.distance;
            Debug.Log("Object in front at distance: " + distanceToObj);
        }
        else
        {
            Debug.Log("No object in front within " + sensorRange + " units.");
        }

        // (Optional) Draw the ray in the Scene view for debugging
        Debug.DrawRay(transform.position, transform.forward * sensorRange);
    }
}

