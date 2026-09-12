using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is player in field of view", story: "Checks if [Agent] can see [targetCharacter] in fov", category: "Conditions", id: "1804271ad19fcaa982799d880814522b")]
public partial class IsPlayerInFieldOfViewCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> targetCharacter;


    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null)
        {
            return false;
        }

        EnemyClass enemy = Agent.Value.GetComponent<EnemyClass>();
        //Debug.Log($"Enemy: {enemy}, TargetCharacter: {enemy?.targetCharacter}");
        if (enemy == null || enemy.targetCharacter == null)
        {
            return false;
        }
        
        if (enemy != null && enemy.targetCharacter != null)
        {
            targetCharacter.Value = enemy.targetCharacter.gameObject;
            return true;
        }
        return false;
    }
}
