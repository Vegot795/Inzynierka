using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : EnemyClass
{
    [Header("Zombie Stats")]
    public int zombieScoreValue = 100;

    private Collider2D attackCol;



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


    public void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterBase character = collision.gameObject.GetComponent<CharacterBase>();
        Debug.Log("Zombie collided with: " + collision.gameObject.name);

        if (character is PC_Controller)
        {
            Debug.Log("Zombie is attacking: " + character.gameObject.name);
            Attack(character);
            Debug.Log($"{character.name} has been attacked, current HP: {character.CurrentHp}");
        }
    }

   

}
