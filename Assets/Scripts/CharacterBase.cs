using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public CircleCollider2D col;

    public float MaxHp;
    public float CurrentHp;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();

        CurrentHp = MaxHp;
    }

    public void TakeDamage(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            ProjectileScript projectile = collision.GetComponent<ProjectileScript>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
                Destroy(collision.gameObject);
            }
        }

    }

}
