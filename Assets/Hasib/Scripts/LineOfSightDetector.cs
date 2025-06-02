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
        
        if (hit.collider!= null && hit.collider.gameObject.layer == target.layer)
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
}
