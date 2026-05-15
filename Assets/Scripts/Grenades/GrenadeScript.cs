using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public float damage;
    public float explosionRadius;
    public float moveSpeed;

    private Vector2 targetPosition;
    private bool hasExploded = false;


    public void Initialize(GrenadeData data, Vector2 target)
    {
        damage = data.damage;
        explosionRadius = data.explosionRadius;
        moveSpeed = data.moveSpeed;
        targetPosition = target;

        Debug.Log($"Grenade initialized with target position: {targetPosition}, damage: {damage}, explosion radius: {explosionRadius}, move speed: {moveSpeed}");
    }
    private void Update()
    {
        if (hasExploded)
        {
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPosition - currentPos).normalized;

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(currentPos, targetPosition, step);
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded)
        {
            return;
        }
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            hitCollider.GetComponent<CharacterBase>()?.TakeDamage(damage);
            Debug.Log($"Object {hitCollider.name} is within explosion radius and takes {damage} damage.");
        }

        hasExploded = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
