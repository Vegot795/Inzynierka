using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set character as target", story: "Action sets a target for enemy to follow", category: "Action", id: "7cf298893f19452ed5299968f36650ce")]
public partial class SetCharacterAsTargetAction : Action
{
    protected override Status OnStart()
    {
        EnemyClass enemy = GameObject.GetComponent<EnemyClass>();
        if (enemy == null)
        {
            return Status.Failure;
        }

        FoV fov = GameObject.GetComponentInChildren<FoV>(true);
        if (fov == null || fov.spotted == null)
        {
            return Status.Failure;
        }

        enemy.targetCharacter = fov.spotted.transform;
        enemy.lastSpottedPosition = fov.spotted.transform.position;

        return Status.Success;
    }
}
