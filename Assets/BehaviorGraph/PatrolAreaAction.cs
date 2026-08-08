using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol area", story: "Enemy will patrol area until it sees player", category: "Action", id: "f043c4cce82b9f675cfb43893f3b0457")]
public partial class PatrolAreaAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;


    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
        {
            return Status.Failure;
        }

        EnemyClass MobScript = Agent.Value.GetComponent<EnemyClass>();

        if (MobScript == null || MobScript is not EnemyClass)
        {
            return Status.Failure;
        }

        var randomPoint = MobScript.FindRandomPointToWalkTo(MobScript.attempts, MobScript.minDistance);
        MobScript.GoToCell(randomPoint);

        return MobScript.targetLocation != null ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        EnemyClass MobScript = Agent.Value.GetComponent<EnemyClass>();

        if (MobScript == null)
        {
            return Status.Failure; 
        }


        return MobScript.targetLocation != null ? Status.Running : Status.Success;
    }
}

