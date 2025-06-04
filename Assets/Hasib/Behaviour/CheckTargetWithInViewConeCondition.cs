using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Target With InViewCone", story: "Check [Target] with [InViewCone]", category: "Conditions", id: "7c06c67ba3ffbfd16d6caf8c55108f3c")]
public partial class CheckTargetWithInViewConeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<LineOfSightDetector> InViewCone;

    public override bool IsTrue()
    {
        return InViewCone.Value.IsTargetInViewCone(Target.Value.transform);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
