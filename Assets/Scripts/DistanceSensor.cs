using UnityEngine;
using TMPro;

public class DistanceSensor : MonoBehaviour
{
    public float sensorRange = 10f;
    public TMP_Text distanceText; 

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, sensorRange))
        {
            float distanceToObj = hit.distance;
            distanceText.text = $"Distance: {distanceToObj:F2}";
        }
        else
        {
            distanceText.text = "No object detected";
        }

        Debug.DrawRay(transform.position, transform.forward * sensorRange, Color.red);
    }
}
