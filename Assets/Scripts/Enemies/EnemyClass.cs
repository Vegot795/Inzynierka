using UnityEngine;

public class EnemyClass : CharacterBase
{
    public int scoreValue;
    private void Awake()
    {
        base.characterType = CharacterType.Enemy;
    }

    public virtual void Attack()
    {
        return;
    }

    protected override void Die(CharacterBase killer)
    {

        Destroy(gameObject);
        killer.GetComponent<ScoreSystem>()?.AddScore(scoreValue);
    }
}
