using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sets target location based on lastSpottedLocation", story: "Sets new targetLocation for enemy to follow", category: "Action", id: "444f6d2f2fd57b2074305b90297bf971")]
public partial class SetNewTargetLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Agent;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
        {
            return Status.Failure;
        }

        EnemyClass enemy = Agent.Value.GetComponent<EnemyClass>();

        if (enemy == null)
        {
            return Status.Failure;
        }

        if (enemy.lastSpottedPosition == null)
        {
            return Status.Failure;
        }


        enemy.GoToCell(enemy.lastSpottedPosition);
        return enemy.targetLocation != null ? Status.Running : Status.Failure;

    }

    protected override Status OnUpdate()
    {
        EnemyClass enemy = Agent.Value.GetComponent<EnemyClass>();

        if (enemy == null)
        {
            return Status.Failure;
        }


        return enemy.targetLocation != null ? Status.Running : Status.Success;
    }
}
