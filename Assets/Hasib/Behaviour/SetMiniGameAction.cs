using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetMiniGame", story: "Enable minigame from the [LieGame]", category: "Action", id: "2a6f97293937c43f01df71eb400f6724")]
public partial class SetMiniGameAction : Action
{
    [SerializeReference] public BlackboardVariable<GameManager> LieGame;

    protected override Status OnStart()
    {
        Debug.Log("MiniGame");
        LieGame.Value.EnableLieGame();
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

