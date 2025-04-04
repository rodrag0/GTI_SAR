using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class LidarScannerURP : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public float scanInterval = 0.05f;
    public int laserChannels = 64;
    public float verticalFOV = 25f;
    public float maxDistance = 60f;

    public Material pointMaterial;
    private List<Vector3> pointList = new List<Vector3>();
    private ComputeBuffer pointBuffer;

    private float timer = 0f;
    private float angleStep;

    void Start()
    {
        angleStep = verticalFOV / (laserChannels - 1);
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        if (timer >= scanInterval)
        {
            Scan();
            timer = 0f;
        }

        if (pointList.Count == 0) return;

        if (pointBuffer != null)
        {
            pointBuffer.Release();
        }

        pointBuffer = new ComputeBuffer(pointList.Count, sizeof(float) * 3);
        pointBuffer.SetData(pointList.ToArray());

        pointMaterial.SetBuffer("_Positions", pointBuffer);
        pointMaterial.SetColor("_Color", Color.cyan);
        Graphics.DrawProcedural(pointMaterial, new Bounds(Vector3.zero, Vector3.one * 500f), MeshTopology.Points, pointList.Count);
    }

    void Scan()
    {
        Vector3 origin = transform.position;

        for (int i = 0; i < laserChannels; i++)
        {
            float verticalAngle = -verticalFOV / 2f + i * angleStep;
            Quaternion rot = Quaternion.Euler(verticalAngle, transform.eulerAngles.y, 0);
            Vector3 direction = rot * Vector3.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
            {
                pointList.Add(hit.point);
            }
        }

        // Debug opcional
        // Debug.Log("Points scanned: " + pointList.Count);
    }

    void OnDestroy()
    {
        if (pointBuffer != null)
            pointBuffer.Release();
    }
}
