using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsCalledBySkeleton", story: "Is [self] called by skeleton to protect", category: "Conditions", id: "10dd2671044f6b6a3d0388d06ad1dd71")]
public partial class IsCalledBySkeletonCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null)
        {
            Debug.Log("Zombie - IsCalledBySkeleton - Agent is null");
        }
        EnemyClass self = Agent.Value.GetComponent<EnemyClass>();

        if (self is ZombieEnemy zombie)
        {
            if (zombie.calledbySkeleton)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log("Zombie - IsCalledBySkeleton - Agent is not a ZombieEnemy");
            return false;
        }
    }
}
