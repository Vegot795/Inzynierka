using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : EnemyClass
{
    public int zombieScoreValue = 100;





    void Start()
    {
        MaxHp = 100f;
        CurrentHp = MaxHp;
        MoveSpeed = 2f;
        scoreValue = zombieScoreValue;
        fov = GetComponentInChildren<FoV>(true);
    }

    
}
