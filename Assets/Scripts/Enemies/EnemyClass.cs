using System.Threading;
using UnityEngine;

public class EnemyClass : CharacterBase
{
    public int scoreValue;
    public Transform targetCharacter;
    public Vector3? targetLocation;
    public Vector3? lastSpottedPosition;
    public float viewDistance = 6f;
    public float lostIntrestTime = 10f;

    private void Awake()
    {
        base.characterType = CharacterType.Enemy;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
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

    public void StartLostIntrestDelay()
    {
        float timer = lostIntrestTime;
        timer = timer - Time.deltaTime;
        if(timer <= 0)
        {
            lastSpottedPosition = null;
        }

    }
}
