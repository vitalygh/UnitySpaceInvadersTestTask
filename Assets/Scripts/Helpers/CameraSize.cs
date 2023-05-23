using UnityEngine;

public class CameraSize : MonoBehaviour
{
    public float size = 11.0f;
    private Camera cam = null;
    private float aspect = -1.0f;
    private void UpdateCameraSize()
    {
        if (cam == null)
            return;
        if (Mathf.Approximately(cam.aspect, aspect))
            return;
        aspect = cam.aspect;
        cam.orthographicSize = (cam.aspect < 1.0f) ? size / cam.aspect : size;
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateCameraSize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraSize();
    }
}
