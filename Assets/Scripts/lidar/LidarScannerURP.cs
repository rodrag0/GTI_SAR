using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class LidarScannerURP : MonoBehaviour
{
    public float rotationSpeed = 1300f;
    public float scanInterval = 0.02f; // 50Hz
    public int laserChannels = 180;
    public float verticalFOV = 165f;
    public float maxDistance = 32f;
    public float refreshrate = 11f; // tiempo de vida de los puntos en segundos
    public float positionResetThreshold = 0.5f;

    public Material pointMaterial;

    private List<Vector3> pointList = new();
    private ComputeBuffer pointBuffer;

    private HashSet<Vector3> seenPoints = new();
    private float timer = 0f;
    private float angleStep;
    private Vector3 lastScannerPosition;

    // 🧠 Punto rastreado con temporizador acumulado
    private class TrackedPoint
    {
        public GameObject sphere;
        public Vector3 position;
        public float age;

        public TrackedPoint(GameObject s, Vector3 pos)
        {
            sphere = s;
            position = pos;
            age = 0f;
        }

        public void UpdateAge(float delta)
        {
            age += delta;
        }
    }

    private List<TrackedPoint> trackedPoints = new();

    void Start()
    {
        angleStep = verticalFOV / (laserChannels - 1);
        lastScannerPosition = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Reset if moved
        if (Vector3.Distance(transform.position, lastScannerPosition) > positionResetThreshold)
        {
            ResetScan();
            lastScannerPosition = transform.position;
        }

        if (timer >= scanInterval)
        {
            Scan();
            timer = 0f;
        }

        // 🧹 Limpiar puntos viejos
        for (int i = trackedPoints.Count - 1; i >= 0; i--)
        {
            trackedPoints[i].UpdateAge(Time.deltaTime);

            if (trackedPoints[i].age > refreshrate)
            {
                if (trackedPoints[i].sphere != null)
                    Destroy(trackedPoints[i].sphere);

                seenPoints.Remove(trackedPoints[i].position);
                trackedPoints.RemoveAt(i);
            }
        }

        if (pointList.Count == 0) return;

        if (pointBuffer != null)
            pointBuffer.Release();

        pointBuffer = new ComputeBuffer(pointList.Count, sizeof(float) * 3);
        pointBuffer.SetData(pointList.ToArray());

        pointMaterial.SetBuffer("_Positions", pointBuffer);
        pointMaterial.SetColor("_Color", Color.cyan);

        Graphics.DrawProcedural(
            pointMaterial,
            new Bounds(transform.position, Vector3.one * 1000f),
            MeshTopology.Points,
            pointList.Count
        );
    }

    void Scan()
    {
        Vector3 origin = transform.position;
        pointList.Clear();

        int layerMask = ~(1 << 6); // Ignora la capa 6 ("Lidar Vision")

        for (int i = 0; i < laserChannels; i++)
        {
            float verticalAngle = -verticalFOV / 2f + i * angleStep;
            Quaternion rot = Quaternion.Euler(verticalAngle, transform.eulerAngles.y, 0);
            Vector3 direction = rot * Vector3.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask))
            {
                pointList.Add(hit.point);

                if (!seenPoints.Contains(hit.point))
                {
                    seenPoints.Add(hit.point);

                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = hit.point;
                    sphere.transform.localScale = Vector3.one * 0.015f;
                    sphere.layer = 6; // Lidar Vision

                    var renderer = sphere.GetComponent<Renderer>();
                    if (renderer != null)
                        renderer.material.color = Color.red;

                    trackedPoints.Add(new TrackedPoint(sphere, hit.point));
                }
            }
        }
    }

    void ResetScan()
    {
        seenPoints.Clear();

        foreach (var tracked in trackedPoints)
        {
            if (tracked.sphere != null)
                Destroy(tracked.sphere);
        }

        trackedPoints.Clear();
        pointList.Clear();
    }

    void OnDestroy()
    {
        if (pointBuffer != null)
            pointBuffer.Release();
    }
}
