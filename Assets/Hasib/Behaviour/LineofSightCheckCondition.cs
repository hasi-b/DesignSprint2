using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "LIne of sight check", story: "Check [Target] with line of sight [detector]", category: "Conditions", id: "cd4d0c1e62326ab773294facad43d309")]
public partial class LIneOfSightCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<LineOfSightDetector> Detector;

    public override bool IsTrue()
    {
        return Detector.Value.PerformDetection(Target)!= null;
    }

    
}
