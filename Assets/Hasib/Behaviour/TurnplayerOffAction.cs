using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TurnplayerOff", story: "Turn Player Off from [GameManager]", category: "Action", id: "8640d983ee0155a366669f885923bff5")]
public partial class TurnplayerOffAction : Action
{
    [SerializeReference] public BlackboardVariable<GameManager> GameManager;

    protected override Status OnStart()
    {
        if (GameManager.Value.IsPlayerOn)
        {
            Debug.Log("Yayyyyyyyyyyyyyyyyyyy");
           GameManager.Value.DisablePlayer();
        }
        else
        {
            Debug.Log("WWWWWWWWWWWWYayyyyyyyyyyyyyyyyyyy");
            GameManager.Value.EnablePlayer();
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

