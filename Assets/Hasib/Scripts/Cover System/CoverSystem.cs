using StarterAssets;
using UnityEngine;

public class CoverSystem : MonoBehaviour
{
    [SerializeField]
    float maxDistanceFromCover;
    [SerializeField]
    LayerMask coverLayer;
    [SerializeField]
    Transform highCoverDetectionThreshold;
    [SerializeField]
    float initialRaycastHeight;
    Vector3 coverLocation;
    ICharacterPlacer characterPlacer;
    
    private void OnEnable()
    {
        StarterAssetsInputs.OnCoverButtonPressedAction += OnCoverButtonPressed;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.OnCoverButtonPressedAction -= OnCoverButtonPressed;
    }

    private void Start()
    {
        characterPlacer = GetComponent<ICharacterPlacer>();
    }

    void OnCoverButtonPressed()
    {
        Debug.Log("Cover search");

        if(!IsNearCover()) return;
        IsHighCover();
        MoveCharacterToCover();

        //Debug.Log(IsHighCover() ? "High cover" : "Low cover");

    }

    private bool IsNearCover()
    {
        RaycastHit hitInfo;
        
         bool hitDecision = Physics.Raycast(transform.position+new Vector3(0,initialRaycastHeight,0),transform.forward,out hitInfo,maxDistanceFromCover,coverLayer);
        coverLocation = hitInfo.point;
        return hitDecision;

    }

    private bool IsHighCover()
    {
        RaycastHit hitInfo;

        bool hitDecision = Physics.Raycast(highCoverDetectionThreshold.position, transform.forward, out hitInfo, maxDistanceFromCover, coverLayer);
        if(hitDecision)coverLocation= hitInfo.point;
        return hitDecision;
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward.normalized * maxDistanceFromCover);
    }

    void MoveCharacterToCover()
    {
        characterPlacer.cover = true;
        characterPlacer.BeginMoveToCover(coverLocation);
    }
}
