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
    [SerializeField] public BlackboardVariable<GameObject> Agent;
    [SerializeField] public BlackboardVariable<GameObject> Pathfinder;


    protected override Status OnStart()
    {
        EnemyClass MobScript = Agent.Value.GetComponent<EnemyClass>();

        if (Agent == null || Agent.Value == null)
        {
            return Status.Failure;
        }

        if (Pathfinder == null || Pathfinder.Value == null)
        {
            return Status.Failure;
        }

        if (MobScript == null || MobScript is not )
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        EnemyClass MobScript = Agent.Value.GetComponent<EnemyClass>();
        MobScript.PatrolArea(Pathfinder.Value.transform.position, Pathfinder.Value.GetComponent<PatrolPath>().GetPatrolPoints());


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

