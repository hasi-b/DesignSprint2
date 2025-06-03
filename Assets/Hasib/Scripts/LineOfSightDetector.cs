using UnityEngine;

public class LineOfSightDetector : MonoBehaviour
{


    [SerializeField]
    float detectionHeight = 3.0f;
    [SerializeField]
    float detectionRange = 10.0f;
    [SerializeField]
    LayerMask playerMask;
    [SerializeField]
    bool showDetectVisuals;
    [SerializeField]
    GameObject target;
    public float viewAngle = 60f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject PerformDetection(GameObject potentialTarget)
    {

        RaycastHit hit;
        Vector3 direction =  target.transform.position - transform.position;
        Physics.Raycast(transform.position + Vector3.up * detectionHeight, direction,out hit,detectionRange,playerMask);
        
        if (hit.collider!= null && hit.collider.gameObject.layer == target.layer && IsTargetInViewCone(target.transform))
        {

            Debug.Log("Hit "+ hit.collider.name);
            if (showDetectVisuals)
            {
                Debug.DrawLine(transform.position + Vector3.up * detectionHeight,potentialTarget.transform.position,Color.green);
            }
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    bool IsTargetInViewCone(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        return angleToTarget <= viewAngle / 2f;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Set Gizmo color for the view cone
        Gizmos.color = Color.yellow;

        // Origin point at eye level (considering detection height)
        Vector3 origin = transform.position + Vector3.up * detectionHeight;

        // Forward direction
        Vector3 forward = transform.forward;

        // Draw detection range line
        Gizmos.DrawLine(origin, origin + forward * detectionRange);

        // Calculate the two side directions of the view cone
        float halfAngle = viewAngle / 2f;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        // Draw view cone boundaries
        Gizmos.DrawLine(origin, origin + leftRayDirection * detectionRange);
        Gizmos.DrawLine(origin, origin + rightRayDirection * detectionRange);

        // Optional: draw arc to visualize the cone (requires Handles, only works in Editor)
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(1, 1, 0, 0.2f);
        UnityEditor.Handles.DrawSolidArc(origin, Vector3.up, leftRayDirection, viewAngle, detectionRange);
#endif
    }

}
