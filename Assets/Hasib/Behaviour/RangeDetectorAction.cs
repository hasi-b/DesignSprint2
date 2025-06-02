using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RangeDetector", story: "Update [Range_Detector] and Assign [Target]", category: "Action", id: "ef2dfc211674b8b0ddf49c72212994f2")]
public partial class RangeDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetector> Range_Detector;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

   

    protected override Status OnUpdate()
    {
        Target.Value = Range_Detector.Value.UpdateDetector();
        return Range_Detector.Value.UpdateDetector() == null ? Status.Success : Status.Failure;
    }

    
}

