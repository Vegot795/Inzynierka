using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : EnemyClass
{
    public int zombieScoreValue = 100;


    public float repathInterval = 0.5f;



    void Start()
    {
        MaxHp = 100f;
        CurrentHp = MaxHp;
        MoveSpeed = 2f;
        scoreValue = zombieScoreValue;
        fov = GetComponentInChildren<FoV>(true);
    }

    
}
