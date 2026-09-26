using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum CharacterType
{
    Player,
    Enemy,
    Neutral
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
    public bool isFlashed;

    public CircleCollider2D col;

    private List<StatusEffect> activeEffects = new List<StatusEffect>();
    private List<CharacterBase> attackers = new List<CharacterBase>();
    private CharacterBase lastAttacker;
    private float timeToResetAttackers = 30f;


    private void Update()
    {
        TickEffect();
    }

    public virtual void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        CurrentHp = MaxHp;
        DetermineCharacterType();
    }

    public void TakeDamage(float damage, CharacterBase attacker)
    {
        CurrentHp -= damage;

        if (attacker != null && !attackers.Contains(attacker))
        {
            attackers.Add(attacker);
            StartCoroutine(ResetAttackers());
        }
        lastAttacker = attacker;

        if (CurrentHp <= 0)
        {
            Die(attacker);
        }
    }
    public IEnumerator ResetAttackers()
    {
        yield return new WaitForSeconds(timeToResetAttackers);
        attackers.Clear();
    }

    protected virtual void Die(CharacterBase killer)
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