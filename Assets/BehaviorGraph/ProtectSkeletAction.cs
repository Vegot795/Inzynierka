using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ProtectSkelet", story: "Agent will become a shield for a skelet", category: "Action", id: "4304b7aa48e7c095f060b47719935d32")]
public partial class ProtectSkeletAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
        {
            return Status.Failure;
        }

        EnemyClass mobScript = Agent.Value.GetComponent<EnemyClass>();
        if (mobScript == null || mobScript is not ZombieEnemy zombie)
        {
            return Status.Failure;
        }
        else
        {
            mobScript.targetLocation = zombie.spotLocation.transform.position;
        }

        mobScript.targetCharacter = null;
        return mobScript.targetLocation != null ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        ZombieEnemy MobScript = Agent.Value.GetComponent<ZombieEnemy>();

        if(MobScript == null)
        {
            return Status.Failure;
        }

        if (MobScript.skeletonCaller == null)
        {
            return Status.Failure;
        }
        MobScript.targetLocation = MobScript.spotLocation.transform.position;
        MobScript.repathTimer = 0;

        Vector3 lookDirection = (MobScript.skeletonCaller.transform.position - MobScript.transform.position).normalized;

        return Status.Running;
    }
}

