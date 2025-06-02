using UnityEngine;

public class RangeDetector : MonoBehaviour
{
    [SerializeField]
    float detectionRadius;
    [SerializeField]
    LayerMask detectionMask;
    GameObject detectedTarget;
    [SerializeField]
    bool showDetectVisuals;
    [SerializeField]
    LineOfSightDetector lineOfSightDetector;
    public GameObject DetectedTarget { get => detectedTarget; set => detectedTarget = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       //UpdateDetector();
    }

    public GameObject UpdateDetector()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,detectionRadius,detectionMask);
        if (colliders.Length>0)
        {
            DetectedTarget = colliders[0].gameObject;
           // lineOfSightDetector.PerformDetection(detectedTarget);
        }
        else
        {
            DetectedTarget = null;

        }
        return DetectedTarget;
    }

    void OnDrawGizmos()
    {
        if(!showDetectVisuals) { return; }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (DetectedTarget != null)
        {
           // Debug.Log("Detected");
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            //Gizmos.DrawLine(transform.position, DetectedTarget.transform.position);
            //Gizmos.DrawSphere(DetectedTarget.transform.position, 0.2f);
        }
    }
}
