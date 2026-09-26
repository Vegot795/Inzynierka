using UnityEngine;

public class EnemyController : CharacterBase
{
    public int scoreValue = 100;
    protected override void Die(CharacterBase killer)
    {

        Destroy(gameObject);
        killer.GetComponent<PlayerInventory>()?.AddScore(scoreValue);
    }
}
