using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float SDT = 3f;
    public float damage;
    public float time;
    public Collider2D col;
    public CharacterBase shooter { get; private set; }

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public void Initialize(float weaponDamage, CharacterBase shooterCharacter)
    {
        damage = weaponDamage;
        shooter = shooterCharacter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Projectile] OnTriggerEnter2D called. Hit: {collision.gameObject.name}");
        if (shooter != null && collision.gameObject == shooter)
        {
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log($"[Projectile] Hit enemy: {collision.gameObject.name}");
            CharacterBase character = collision.GetComponent<CharacterBase>();
            if (character != null)
            {
                character.TakeDamage(damage, shooter);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogWarning($"[Projectile] OnCollisionEnter2D called (should be trigger!). Hit: {collision.gameObject.name}");
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= SDT) 
        {
            Destroy(gameObject);
        }
    }
}
