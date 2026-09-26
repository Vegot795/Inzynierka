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
        //Debug.Log("PatrolAreaAction: OnStart called for " + Agent.Value.name);
        Vector3? randomPoint = null;

        if (randomPoint == null || randomPoint == Vector3.zero)
        {
            randomPoint = MobScript.FindRandomPointToWalkTo(MobScript.attempts, MobScript.minDistance);
            //Debug.Log("PatrolAreaAction: Random point to walk to: " + randomPoint);

        }
        MobScript.GoToCell(randomPoint);

        return MobScript.targetLocation != null ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        EnemyClass MobScript = Agent.Value.GetComponent<EnemyClass>();
        Debug.Log("PatrolAreaAction: OnUpdate called for " + Agent.Value.name);

        if (MobScript == null)
        {
            //Debug.LogError("PatrolAreaAction: MobScript is null for " + Agent.Value.name);
            return Status.Failure; 
        }

        if (MobScript.targetCharacter != null)
        {
            return Status.Failure;
        }

        //Debug.Log("PatrolAreaAction: Current target location: " + MobScript.targetLocation);
        return MobScript.targetLocation != null ? Status.Running : Status.Success;
    }
}

