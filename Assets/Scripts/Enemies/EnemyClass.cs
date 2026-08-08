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
    public float repathInterval = 0.5f;
    public int attempts = 3;
    public float minDistance = 1f;
    public float timer;


    public List<Vector3> currentPath;
    public int pathIndex;
    public float repathTimer;

    private void Awake()
    {
        base.characterType = CharacterType.Enemy;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
    void Update()
    {
        WalkAccordingToPath();

        if (targetCharacter == null)
        {
            StartLoseIntrestDelay();
        }
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

    public void StartLoseIntrestDelay()
    {
        if (targetCharacter == null)
        {
            if(lastSpottedPosition == null)
            {
                Debug.Log($"[EnemyClass] - No lastSpottedPosition to go to.");
                return;
            }

            targetCharacter.position = lastSpottedPosition;
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                lastSpottedPosition = null;
            }
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

    public Vector3? FindRandomPointToWalkTo(int attempts, float minDistance)
    {
        if (pathfinder == null)
        {
            Debug.Log($"[EnemyClass] - Pathfinder is null.");
            return null;
        }

        var centralPoint = transform.position;
        
        for (int i = 0; i < attempts; i++)
        {
            Vector3 randomPosition = new Vector3(centralPoint.x + Random.Range(-viewDistance, viewDistance), centralPoint.y + Random.Range(-viewDistance, viewDistance), 0);

            Node cell = pathfinder.CellFromWorldPoint(randomPosition);

            if (cell.walkable || cell == null || Vector3.Distance(centralPoint, randomPosition) < minDistance)
            {
                continue;
            }
            return cell.worldPosition;
        }
        return null;
    }


    public void WalkAccordingToPath()
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            RequestNewPath();
        }

        if (currentPath != null && pathIndex >= currentPath.Count)
        {
            targetLocation = null;
            currentPath = null;
            return;
        }

        Vector3 destination = currentPath[pathIndex];
        transform.position = Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.05f)
        {
            pathIndex++;
        }
    }
    /// <summary>
    /// Moves the enemy to a specified cell or a random cell if no target is provided. If the enemy is already moving towards a target, it will not change its destination.
    /// </summary>
    /// <param name="targetCell"></param>
    public void GoToCell(Vector3? targetCell)
    {
        if (targetLocation != null)
        {
            return; 
        }

        targetLocation = targetCell;
        repathTimer = 0f;
    }

}
