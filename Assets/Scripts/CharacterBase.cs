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
        Debug.Log($"[{gameObject.name}] Took {damage} damage. Current HP: {CurrentHp}");
        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
