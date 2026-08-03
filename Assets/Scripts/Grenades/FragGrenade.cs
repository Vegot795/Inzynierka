using UnityEngine;

public class FragGrenade : GrenadeScript
{
    public override void Explode()
    {
        if (hasExploded)
        {
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            hitCollider.GetComponent<CharacterBase>()?.TakeDamage(damage, playerController);
            //Debug.Log($"Object {hitCollider.name} is within explosion radius and takes {damage} damage.");
        }

        hasExploded = true;
        animator.SetTrigger("toExplode");
    }
}