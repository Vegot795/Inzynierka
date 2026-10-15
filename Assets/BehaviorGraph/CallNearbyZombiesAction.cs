using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CallNearbyZombies", story: "Orders zombies from arount to protect the skelet", category: "Action", id: "1ca7e16788386cf44043d7e0f80d96be")]
public partial class CallNearbyZombiesAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
        {
            Debug.Log("SKelet failed at Agent assign");
            return Status.Failure;
        }
        
        SkeletEnemy skelet = Agent.Value.GetComponent<SkeletEnemy>();
        if (skelet == null)
        {
            Debug.Log("SKelet failed at script assign");
            return Status.Failure;
        }   
        SkeletHelpCaller callRangeScript = skelet.transform.Find("RangeCallCol").GetComponent<SkeletHelpCaller>();

        if (callRangeScript == null || callRangeScript.zombiesAround.Count < skelet.neededMobs)
        {
            Debug.Log("SKelet failed at rangeColS script assign");
            return Status.Failure;
        }
        skelet.CallForHelp();


        return Status.Success;
    }
}

