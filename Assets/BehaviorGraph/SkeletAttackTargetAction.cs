using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SkeletAttackTarget", story: "[Skelet] attacks [target] if in [DistanceZone]", category: "Action", id: "3f496a660aa6aeadc1c5897af3b370fb")]
public partial class SkeletAttackTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Skelet;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> DistanceZone;


    protected override Status OnStart()
    {
        if (Skelet == null || Skelet.Value == null || Target == null || Target.Value == null)
        {
            return Status.Failure;
        }

        SkeletEnemy skelet = Skelet.Value.GetComponent<SkeletEnemy>();
        if (skelet == null)
        {
            return Status.Failure;
        }

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        SkeletEnemy skelet = Skelet.Value.GetComponent<SkeletEnemy>();
        if (Target.Value == null || Vector3.Distance(skelet.transform.position, Target.Value.transform.position) > skelet.attackRange)
        {
            return Status.Failure;
        }

        Vector3 direction = Target.Value.transform.position - skelet.transform.position;
        skelet.fov.SetAimDirection(direction);
        skelet.ShootProjectile(direction); ///// This piece of code is to be changed, for now there is no animation ~Ogun

        return Status.Success;
    }
}

