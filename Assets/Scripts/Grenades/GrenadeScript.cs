using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public float damage;
    public float explosionRadius;
    public float moveSpeed;
    public bool isThrown = false;

    private Vector2 targetPosition;
    private bool hasExploded = false;
    private GrenadeData grenadeData;
    private GrenadeType grenadeType;


    public void Initialize(GrenadeData data, Vector2 target)
    {
        grenadeData = data;

        damage = grenadeData.damage;
        explosionRadius = grenadeData.explosionRadius;
        moveSpeed = grenadeData.moveSpeed;
        grenadeType = grenadeData.grenadeType;
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
        switch (grenadeType)
        {
            case GrenadeType.Frag:
                Frag();
                break;
            case GrenadeType.Flash:
                Flash();
                break;
            case GrenadeType.Kasket:
                Kasket();
                break;
            default:
                Debug.LogWarning($"Unknown grenade type: {grenadeType}");
                break;
        }
    }

    private void Frag()
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

    private void Flash() 
    {
        if (hasExploded)
        {
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            CharacterBase character = hitCollider.GetComponent<CharacterBase>();
            if (character != null)
            {
                //character.Flash();
                Debug.Log($"Object {hitCollider.name} is within flash radius and is flashed.");
            }
        }
    }

    private void Kasket()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
