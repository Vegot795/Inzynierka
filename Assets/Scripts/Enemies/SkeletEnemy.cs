using UnityEngine;

public class SkeletEnemy : EnemyClass
{
    [Header("Skelet Stats")]
    public int skeletScoreValue = 300;
    public float attackRange = 5f;
    public GameObject skeletProjectile;
    public GameObject rod;
    public GameObject skeletProjectilePrefab;
    public float projectileSpeed;


    public override void Awake()
    {
        base.Awake();
        scoreValue = skeletScoreValue;
        rod = transform.Find("Rod").gameObject;
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

}
