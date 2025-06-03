using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartMiniGame", story: "Start Minigame from [GameManager]", category: "Action", id: "50c2c79f48ce3f74afbbade045372af0")]
public partial class StartMiniGameAction : Action
{
    [SerializeReference] public BlackboardVariable<GameManager> GameManager;

    protected override Status OnStart()
    {

        GameManager.Value.EnableLieGame();
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

