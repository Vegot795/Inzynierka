using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GoToCellInRange", story: "Makes [Agent] go to cell closer to the [Player]", category: "Action", id: "e93c4719040ad9540535bded45658bee")]
public partial class GoToCellInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    private Vector3 destination;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null || Player == null || Player.Value == null)
        {
            return Status.Failure;
        }

        EnemyClass enemyScript = Agent.Value.GetComponent<EnemyClass>();
        if (enemyScript == null)
        {
            return Status.Failure;
        }

        Vector3 distance = Player.Value.transform.position - Agent.Value.transform.position;
        if (enemyScript is SkeletEnemy skeletEnemy)
        {
            if (distance.magnitude < skeletEnemy.viewDistance && distance.magnitude > skeletEnemy.viewDistance/2)
            {
                return Status.Success;
            }
            else if(distance.magnitude > skeletEnemy.viewDistance || distance.magnitude < skeletEnemy.viewDistance / 2)
            {
                destination = skeletEnemy.GetCellBetweenThisAndTarget().transform.position;
                skeletEnemy.GoToCell(destination);
            }
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        EnemyClass enemyScript = Agent.Value.GetComponent<EnemyClass>();

        Vector3 distance = Player.Value.transform.position - Agent.Value.transform.position;
        if (enemyScript is SkeletEnemy skeletEnemy)
        {

            if (distance.magnitude < skeletEnemy.viewDistance && distance.magnitude > skeletEnemy.viewDistance / 2)
            {
                return Status.Success;
            }

        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

