using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum CharacterType
{
    Player,
    Enemy
}

public class CharacterBase : MonoBehaviour
{
    [Header("Character Stats")]
    public CharacterType characterType;
    public float MaxHp;
    public float CurrentHp;
    public float MoveSpeed;

    [Header("Character status")]
    public bool isStunned;
    public bool isSlowed;

    public CircleCollider2D col;

    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    private void Update()
    {
        TickEffect();
    }

    protected virtual void Start()
    {
        col = GetComponent<CircleCollider2D>();
        CurrentHp = MaxHp;
        DetermineCharacterType();
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

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private void DetermineCharacterType()
    {
        if (GetComponent<PC_Controller>() != null)
        {
            characterType = CharacterType.Player;
        }
        else if (GetComponent<EnemyController>() != null)
        {
            characterType = CharacterType.Enemy;
        }
    }

# region --- Status Effects ---

    public void ApplyEffect(StatusEffect effect)
    {
        activeEffects.RemoveAll(e => e.GetType() == effect.GetType());
        activeEffects.Add(effect);
        effect.OnApply(this);
    }

    void TickEffect()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].timeRemaining -= Time.deltaTime;
            activeEffects[i].OnTick(this);

            if (activeEffects[i].timeRemaining <= 0)
            {
                activeEffects[i].OnRemove(this);
                activeEffects.RemoveAt(i);
            }
        }
    }

# endregion
}