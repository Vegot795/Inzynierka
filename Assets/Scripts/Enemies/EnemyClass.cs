using UnityEngine;

public class EnemyClass : CharacterBase
{
    private void Awake()
    {
        base.characterType = CharacterType.Enemy;
    }

    public virtual void Attack()
    {
        return;
    }
}
