using UnityEngine;

public class ZombieEnemy : EnemyClass
{

    void Start()
    {
        MaxHp = 100f;
        CurrentHp = MaxHp;
        MoveSpeed = 2f;
    }
}
