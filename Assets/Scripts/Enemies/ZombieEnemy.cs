using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : EnemyClass
{
    [Header("Zombie Stats")]
    public int zombieScoreValue = 100;

    private Collider2D attackCol;
    public bool calledbySkeleton = false;
    public SkeletEnemy skeletonCaller;
    public GameObject spotLocation;
    CharacterBase character;
    public float baseAttackCD;

    public override void Awake()
    {
        base.Awake();
        scoreValue = zombieScoreValue;
        attackCol = transform.Find("AttackCollider").GetComponent<Collider2D>();
    }

    public override void Attack(CharacterBase target)
    {
        if (base.canAttack)
        {
            base.canAttack = false;
            target.TakeDamage(base.Damage, this);
            StartCoroutine(AttackCooldownCoroutine(base.attackCooldown));
        }
    }

    public override void Update()
    {
        base.Update();

        if (character is PC_Controller)
        {
            Attack(character);
        }

    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        character = collision.gameObject.GetComponent<CharacterBase>();
        //Debug.Log("Zombie collided with: " + collision.gameObject.name);

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        character = null;
    }

    protected override void Die(CharacterBase killer)
    {
        base.Die(killer);
        if (skeletonCaller != null)
        {
            skeletonCaller.zombiesCalled.Remove(this);
        }
        var GCS = GameControllerScript.GCS;
        
        foreach (var enemy in GCS.enemiesSpawned)
        {
            if (enemy is SkeletEnemy skelet)
            {
                if (skelet.CallRangeObject.GetComponent<SkeletHelpCaller>().zombiesAround.Contains(this))
                {
                    skelet.CallRangeObject.GetComponent<SkeletHelpCaller>().zombiesAround.Remove(this);
                }

                if (skelet.zombiesCalled.Contains(this))
                {
                    skelet.zombiesCalled.Remove(this);
                }
            }
        }
    }

    public override void KillObject()
    {
        if (skeletonCaller != null)
        {
            skeletonCaller.zombiesCalled.Remove(this);
        }
        var GCS = GameControllerScript.GCS;

        foreach (var enemy in GCS.enemiesSpawned)
        {
            if (enemy is SkeletEnemy skelet)
            {
                if (skelet.CallRangeObject.GetComponent<SkeletHelpCaller>().zombiesAround.Contains(this))
                {
                    skelet.CallRangeObject.GetComponent<SkeletHelpCaller>().zombiesAround.Remove(this);
                }

                if (skelet.zombiesCalled.Contains(this))
                {
                    skelet.zombiesCalled.Remove(this);
                }
            }
        }
        base.KillObject();
    }

}
