using System.Collections.Generic;
using UnityEngine;

public class SkeletEnemy : EnemyClass
{
    [Header("Skelet Stats")]
    public int skeletScoreValue = 300;
    public float attackRange = 5f;
    public float callRange = 10f;
    public int neededMobs = 3;
    public GameObject skeletProjectile;
    public GameObject rod;
    public GameObject skeletProjectilePrefab;
    public float projectileSpeed;
    public GameObject CallRangeObject;
    CircleCollider2D RangeCallCol;
    public GameObject WallSpotObject;
    public GameObject WallSpots;
    public Vector3 playerTargetLocation;
    public List<ZombieEnemy> zombiesCalled;

    public bool isBeingProtected = false;


    public override void Awake()
    {
        base.Awake();

        if (CallRangeObject == null)
        {
            CallRangeObject = transform.Find("RangeCallCol").gameObject;
        }
        if (WallSpots == null)
        { 
            WallSpots = transform.Find("WallSpots").gameObject;
        }
        scoreValue = skeletScoreValue;
        rod = transform.Find("Rod").gameObject;
        RangeCallCol = CallRangeObject.GetComponent<CircleCollider2D>();
        RangeCallCol.radius = callRange;
        RangeCallCol.isTrigger = true;
    }

    public override void Attack(CharacterBase target)
    {
        Vector2 direction = target.transform.position - transform.position;
        PlayShootAnimation(direction);
    }

    public void ShootProjectile(Vector2 direction)
    {
        if (base.targetCharacter == null || Vector3.Distance(transform.position, base.targetCharacter.position) > viewDistance)
        {
            return;
        }
        
        Vector3 spawnPos = new Vector3(rod.transform.position.x, rod.transform.position.y, 0);
        Vector2 shootDirection = direction.normalized;

        GameObject newSkeletProjectile = Instantiate(skeletProjectilePrefab, spawnPos, Quaternion.identity);
        newSkeletProjectile.GetComponent<ProjectileScript>().Initialize(base.Damage, this);
        Rigidbody2D rb = newSkeletProjectile.GetComponent<Rigidbody2D>();

        if (newSkeletProjectile == null)
        {
            return;
        }

        rb.linearVelocity = shootDirection * projectileSpeed;

    }

    public void PlayShootAnimation(Vector2 direction)
    {

    }

    public Vector3? FindPositionToAttackPlayer(int attempts, float minDistance)
    {
        if(base.pathfinder == null || base.targetCharacter == null)
        {
            Debug.Log("Finding cell to attack player...");
            return null;
        }

        var playerPosition = base.targetCharacter.position;
        Vector3? bestPoint = null;
        float bestDistance = float.MaxValue;

        float maxSqr = viewDistance * viewDistance;
        float minSqr = (0.6f * viewDistance) * (0.6f * viewDistance);

        for (int i = 0; i < attempts; i++)
        {
            float angel = Random.Range(0f, Mathf.PI * 2f);
            float radius = Mathf.Sqrt(Random.Range(minSqr, maxSqr));

            Vector3 proposedVec = playerPosition + new Vector3(Mathf.Cos(angel) * radius, Mathf.Sin(angel) * radius, 0);
            Node cell = pathfinder.CellFromWorldPoint(proposedVec);
            
            if (cell == null || !cell.walkable)
            {
                continue;
            }

            float distance = Vector3.Distance(proposedVec, playerPosition);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPoint = cell.worldPosition;
            }
        }

        return bestPoint;
    }

    /* skelet musi znaleść pozycje między 60% a 100% odległości od gracza, aby móc go zaatakować.
    * skelet może atakować gracza tylko jeśli jest w zasięgu ataku, który jest mniejszy niż viewDistance.
    * 
    * Umiejętność bariera: Skelet może stworzyć barierę, która ogranicza ruch gracza. Gracz musi wejść do bariery, aby zaatakować szkielet.
    */

    public void CallForHelp()
    {
        var zombiesAround = CallRangeObject.GetComponent<SkeletHelpCaller>().zombiesAround;
        var closestZombie = new ZombieEnemy[neededMobs];
        List<(EnemyClass, float)> zombieDistances = new List<(EnemyClass, float)>();

        foreach (var zombie in zombiesAround)
        {
            float distance = Vector3.Distance(zombie.transform.position, transform.position);
            zombieDistances.Add((zombie, distance));
        }

        zombieDistances.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        for (int i = 0; i < neededMobs-1; i++)
        {
            closestZombie[i] = (ZombieEnemy)zombieDistances[i].Item1;
        }
        var spawnSpots = CreateShieldSpots();

        for (int i =0; i < spawnSpots.Count; i++)
        {
            closestZombie[i].spotLocation = spawnSpots[i].transform.position;
            closestZombie[i].calledbySkeleton = true;
            closestZombie[i].skeletonCaller = this;
            zombiesCalled.Add(closestZombie[i]);
        }
        isBeingProtected = true;
    }

    public void CancelTheCall()
    {
        foreach (var zombie in zombiesCalled)
        {
            if (zombie != null)
            {
                zombie.calledbySkeleton = false;
                zombie.skeletonCaller = null;
                zombie.spotLocation = Vector3.zero;
            }
        }

        zombiesCalled.Clear();
        isBeingProtected = false;
    }

    private List<GameObject> CreateShieldSpots()
    {
        List<GameObject> shieldSpots = new List<GameObject>();
        for (int i = 0; i < neededMobs; i++)
        {
            Vector3 spawnPosition = new Vector3(2, -(neededMobs / 2) + i + 0.5f, 0);
            var spawnSpot = Instantiate(WallSpotObject);
            spawnSpot.transform.localPosition = spawnPosition;
            spawnSpot.transform.parent = WallSpots.transform;
            shieldSpots.Add(spawnSpot);
        }
        return shieldSpots;
    }

    
}
