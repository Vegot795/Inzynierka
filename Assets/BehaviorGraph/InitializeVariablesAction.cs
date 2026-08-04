using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InitializeVariables", story: "Initializes all variables and scripts into a blackboard values", category: "Action", id: "8d137416a17961c4548547c0a2cbe1c6")]
public partial class InitializeVariablesAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Pathfinder;

    protected override Status OnStart()
    {
        GameObject gameController = GameObject.FindWithTag("GameController")
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

