using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyClass : CharacterBase
{
    public int scoreValue;
    public Transform targetCharacter;
    public Vector3? targetLocation;
    public Vector3? lastSpottedPosition;
    public float viewDistance = 6f;
    public float lostIntrestTime = 10f;
    public Pathfinding pathfinder;
    public FoV fov;

    public List<Vector3> currentPath;
    public int pathIndex;
    public float repathTimer;

    private void Awake()
    {
        base.characterType = CharacterType.Enemy;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public virtual void Attack()
    {
        return;
    }

    protected override void Die(CharacterBase killer)
    {

        Destroy(gameObject);
        killer.GetComponent<ScoreSystem>()?.AddScore(scoreValue);
    }

    public void StartLostIntrestDelay()
    {
        float timer = lostIntrestTime;
        timer = timer - Time.deltaTime;
        if(timer <= 0)
        {
            lastSpottedPosition = null;
        }

    }

    void RequestNewPath()
    {
        repathTimer = repathInterval;

        if (pathfinder == null)
        {
            return;
        }

        if (targetLocation == null)
        {
            currentPath = null;
            return;
        }

        currentPath = pathfinder.FindPath(transform.position, targetLocation.Value);
        pathIndex = 0;
    }

    public Vector3 FindRandomPointToWalkTo()
    {
        var centralPoint = transform.position;
        /*
        var distance = base.viewDistance;

        Node cell;
        
        Vector3 randomPosition = new Vector3(centralPoint.x + Random.Range(-distance, distance), centralPoint.y + Random.Range(-distance, distance), 0);
        var destination = pathfinder.CellFromWorldPoint(randomPosition);

        if (destination == null)
        {
            return centralPoint;
        }

        cell = pathfinder.CellFromWorldPoint(randomPosition);

        if (cell.walkable)
        {
            return cell.worldPosition;
        }*/

        return centralPoint;
    }

    void Update()
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            RequestNewPath();
        }

        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            if (targetCharacter == null)
            {
                targetLocation = null;
                lastSpottedPosition = null;
            }
            return;
        }

        Vector3 destination = currentPath[pathIndex];
        transform.position = Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.05f)
        {
            pathIndex++;
        }
    }

}
