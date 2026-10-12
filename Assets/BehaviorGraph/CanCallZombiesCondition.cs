using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanCallZombies", story: "Checks if there is enough zombies around to call them to protect the [Agent], or if skelet is already protected", category: "Conditions", id: "d6229007f1a1b2308956fb8d9eccc2c5")]
public partial class CanCallZombiesCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null) 
        { 
            //Debug.LogError("Agent is null or not assigned in CanCallZombiesCondition.");
            return false; 
        }

        SkeletEnemy skelet = Agent.Value.GetComponent<SkeletEnemy>();
        if (skelet == null)
        {
            return false;
        }

        if (skelet.isBeingProtected)
        {
            return false;
        }

        List<EnemyClass> zombiesToCall = skelet.transform.Find("RangeCallCol").GetComponent<SkeletHelpCaller>().zombiesAround
            .Where(zombie => zombie.GetComponent<ZombieEnemy>().calledbySkeleton == false &&
                   zombie.GetComponent<ZombieEnemy>().skeletonCaller == null)
            .ToList();

        return zombiesToCall.Count >= skelet.neededMobs;
    }
}
