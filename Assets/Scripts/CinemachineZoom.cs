using UnityEngine;
using Cinemachine;

public class CinemachineZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;

    public float zoomSpeed = 5f;
    public float minZoom = 20f;
    public float maxZoom = 200f;

    void Update()
    {
        if (vcam == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            float fov = vcam.m_Lens.FieldOfView;
            fov = Mathf.Clamp(fov - scroll * zoomSpeed, minZoom, maxZoom);
            vcam.m_Lens.FieldOfView = fov;
        }
    }
}
