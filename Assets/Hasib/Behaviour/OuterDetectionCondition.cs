using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "OuterDetection", story: "Is [Player] in the [Detection]", category: "Conditions", id: "e68f04efa3f6f44179af11be406381d8")]
public partial class OuterDetectionCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<RangeDetector> Detection;

    public override bool IsTrue()
    {
        if (Detection.Value.UpdateDetector())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
