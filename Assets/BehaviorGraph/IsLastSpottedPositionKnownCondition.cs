using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsLastSpottedPositionKnown", story: "Check if enemy remembers last player position", category: "Conditions", id: "891ac5497d1b4b54967ef458d504fc37")]
public partial class IsLastSpottedPositionKnownCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null)
        {
            return false;
        }
        EnemyClass enemy = Agent.Value.GetComponent<EnemyClass>();

        if (enemy == null)
        {
            return false;
        }

        if (enemy.lastSpottedPosition != null)
        {
            return true;
        }
        return false;
    }
}
