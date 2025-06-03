using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ViewConeLineRenderer : MonoBehaviour
{
    public float viewAngle = 90f;
    public float detectionRange = 5f;
    public float detectionHeight = 1.5f;
    public int resolution = 30;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Set LineRenderer properties
        lineRenderer.positionCount = resolution + 2;
        lineRenderer.loop = false;
        lineRenderer.useWorldSpace = false;

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Use a simple material (make sure you have this shader)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
    }

    void Update()
    {
        DrawViewCone();
    }

    void DrawViewCone()
    {
        Vector3 origin = Vector3.up * detectionHeight;
        float halfAngle = viewAngle / 2f;

        lineRenderer.SetPosition(0, origin);

        for (int i = 0; i <= resolution; i++)
        {
            float angle = -halfAngle + (viewAngle * i / resolution);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * Vector3.forward;
            Vector3 point = origin + direction * detectionRange;
            lineRenderer.SetPosition(i + 1, point);
        }
    }
}
