using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanCallZombies", story: "Checks if there is enough zombies around to call them to protect the [Agent]", category: "Conditions", id: "d6229007f1a1b2308956fb8d9eccc2c5")]
public partial class CanCallZombiesCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null) 
        { 
            Debug.LogError("Agent is null or not assigned in CanCallZombiesCondition.");
            return false; 
        }

        SkeletEnemy skelet = Agent.Value.GetComponent<SkeletEnemy>();
        if (skelet == null)
        {
            return false;
        }

        return skelet.transform.Find("RangeCallCol").GetComponent<SkeletHelpCaller>().zombiesAround.Count >= skelet.neededMobs;
    }
}
