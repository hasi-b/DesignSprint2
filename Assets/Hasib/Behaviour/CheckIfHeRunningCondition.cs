using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check if he Running", story: "Check if its [running]", category: "Conditions", id: "aa7c0905bf2b0eaf8feba2faafd1b626")]
public partial class CheckIfHeRunningCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameManager> Running;

    public override bool IsTrue()
    {
        return Running.Value.IsSprinting();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
