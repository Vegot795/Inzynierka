using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : EnemyClass
{
    public int zombieScoreValue = 100;
    public Pathfinding pathfinder;
    public FoV fov;

    public float repathInterval = 0.5f;

    private List<Vector3> currentPath;
    private int pathIndex;
    private float repathTimer;

    void Start()
    {
        MaxHp = 100f;
        CurrentHp = MaxHp;
        MoveSpeed = 2f;
        scoreValue = zombieScoreValue;
        fov = GetComponentInChildren<FoV>(true);
    }

    public void InitializeBlackboardValues()
    {
        BlackboardValu
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
        var distance = base.viewDistance;
        var destination = null;

        Node cell;

        while (!cell.walkable)
        {
            Vector3 randomPosition = new Vector3(centralPoint.x + Random.Range(-distance, distance), centralPoint.y + Random.Range(-distance, distance), 0);
            destination = pathfinder.CellFromWorldPoint(randomPosition);
        }

        return destination;
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
