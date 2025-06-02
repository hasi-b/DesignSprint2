using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "LineofSightCheck", story: "Check [Player] With Line Of Sight [Detector]", category: "Conditions", id: "a097dbfc05d0bb9074ca9e34fe3bf6c9")]
public partial class LineofSightCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<LineOfSightDetector> Detector;

    public override bool IsTrue()
    {
        return Detector.Value.PerformDetection(Player.Value)!= null;
    }

   
}
