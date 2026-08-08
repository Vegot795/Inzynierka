using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is player in field of view", story: "Checks if enemy can see [player] in fov", category: "Conditions", id: "1804271ad19fcaa982799d880814522b")]
public partial class IsPlayerInFieldOfViewCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null)
        {
            return false;
        }

        EnemyClass enemy = Agent.Value.GetComponent<EnemyClass>();
        return enemy != null && enemy.targetCharacter != null;
    }
}
