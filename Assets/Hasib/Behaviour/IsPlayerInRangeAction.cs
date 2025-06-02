using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsPlayerInRange", story: "Is [Player] in the [Detection]", category: "Action", id: "09b6c967040c137bf911a77925f9c7cd")]
public partial class IsPlayerInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<RangeDetector> Detection;

   

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    
}

