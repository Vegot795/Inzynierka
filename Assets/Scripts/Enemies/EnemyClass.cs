using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyClass : CharacterBase
{
    [Header("Enemy Class Stats")]
    public bool canAttack = true;
    public int Damage;
    public float attackCooldown = 1f;
    public int scoreValue;
    public float viewDistance = 6f;
    public float repathInterval = 0.5f;
    public int attempts = 3;
    public float minDistance = 1f;

    [Header("Character Components")]
    public Pathfinding pathfinder;
    public FoV fov;
    public Transform targetCharacter;
    public Vector3? targetLocation;
    public Vector3? lastSpottedPosition;
    public List<Vector3> currentPath;
    public int pathIndex;
    public float repathTimer;
    public Rigidbody2D rb;

    public Pathfinding Pathfinder
    {
        get
        {
            if (pathfinder == null)
            {
                pathfinder = Pathfinding.Instance;
            }
            return pathfinder;
        }
    }

    public override void Awake()
    {
        base.characterType = CharacterType.Enemy;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponentInChildren<FoV>(true);
        CurrentHp = MaxHp;
    }
    void Update()
    {
        WalkAccordingToPath();
    }

    public virtual void Attack(CharacterBase target)
    {
        return;
    }

    public IEnumerator<WaitForSeconds> AttackCooldownCoroutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
    }

    protected override void Die(CharacterBase killer)
    {

        Destroy(gameObject);
        killer.GetComponent<ScoreSystem>()?.AddScore(scoreValue);
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
        Debug.Log($"[EnemyClass] - Central Point: {centralPoint}");

        for (int i = 0; i < attempts; i++)
        {
            Vector3 randomPosition = new Vector3(centralPoint.x + Random.Range(-viewDistance, viewDistance), centralPoint.y + Random.Range(-viewDistance, viewDistance), 0);
            //Debug.Log($"[EnemyClass] - Random Position: {randomPosition}");
            Node cell = pathfinder.CellFromWorldPoint(randomPosition);
            //Debug.Log($"[EnemyClass] - Cell: {cell?.worldPosition}, Walkable: {cell?.walkable}");
            if (!cell.walkable || cell == null || Vector3.Distance(centralPoint, randomPosition) < minDistance)
            {
                Debug.Log($"[EnemyClass] - Cell is not walkable or too close to the central point. Attempt {i + 1} of {attempts}.");
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

        if(currentPath == null)
        {
            return;
        }

        if (pathIndex >= currentPath.Count)
        {
            targetLocation = null;
            currentPath = null;
            return;
        }

        Vector3 destination = currentPath[pathIndex];
        rb.MovePositionAndRotation(Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime), Quaternion.LookRotation(Vector3.forward, destination - transform.position));

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
            Debug.Log($"[EnemyClass] - Already moving towards a target. Current target: {targetLocation}");
            return; 
        }

        targetLocation = targetCell;
        repathTimer = 0f;
        Debug.Log($"[EnemyClass] - New target location set: {targetLocation}");
    }

    public void GetCellToGetInRange(Vector3 targetPosition, float minDistance)
    {
        Vector3? cellToGo = FindRandomPointToWalkTo(attempts, minDistance);
        if (cellToGo != null)
        {
            GoToCell(cellToGo);
        }
        else
        {
            Debug.Log($"[EnemyClass] - No valid cell found to get in range of target at {targetPosition}");
        }
    }

}
