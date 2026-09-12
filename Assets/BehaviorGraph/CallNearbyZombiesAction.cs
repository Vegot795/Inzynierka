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
            return Status.Failure;
        }
        
        SkeletEnemy skelet = Agent.Value.GetComponent<SkeletEnemy>();
        if (skelet == null)
        {
            return Status.Failure;
        }   
        SkeletHelpCaller callRangeScript = skelet.transform.Find("RangeCallCol").GetComponent<SkeletHelpCaller>();

        if (callRangeScript == null || callRangeScript.zombiesAround.Count < skelet.neededMobs)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        SkeletEnemy skelet = Agent.Value.GetComponent<SkeletEnemy>();
        skelet.CallForHelp();

        if (skelet.zombiesCalled.Count >= skelet.neededMobs)
        {
            return Status.Success;
        }

        if (skelet.zombiesCalled.Count == 0)
        {
            skelet.CancelTheCall();
            Debug.Log("Skelet - CallNearbyZombiesAction - No zombies available to protect the skelet.");
            return Status.Failure;
        }

        return Status.Running;
    }
}

