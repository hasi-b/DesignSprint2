using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RangeDetecton", story: "Update [Range] Detector [Target]", category: "Action", id: "c6c6c96836fdcc1dc7cb7e421b070409")]
public partial class RangeDetectonAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetector> Range;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

   

    protected override Status OnUpdate()

    {
 
        Target.Value =  Range.Value.UpdateDetector();
        return Range.Value.UpdateDetector() == null? Status.Failure : Status.Success;
    }

    
}

