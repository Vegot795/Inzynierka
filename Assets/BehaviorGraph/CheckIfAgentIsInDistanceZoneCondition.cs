using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckIfAgentIsInDistanceZone", story: "Check if [Target] is in [Agent]'s [DistanceZone] ", category: "Conditions", id: "1e9c21b5c2ef90b54d03720c287581e1")]
public partial class CheckIfAgentIsInDistanceZoneCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> DistanceZone;

    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null || Target == null || Target.Value == null)
        {
            Debug.LogError("Not all blackboard values are set");
            return false;
        }
        SkeletEnemy skelet = Agent.Value.GetComponent<SkeletEnemy>();

        DistanceZone.Value = skelet.attackRange;

        float distance = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);

        if(distance > DistanceZone.Value || distance < DistanceZone.Value/2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
