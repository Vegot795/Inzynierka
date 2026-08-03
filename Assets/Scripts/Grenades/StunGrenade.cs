using UnityEngine;

public class StunGrenade : GrenadeScript
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
            CharacterBase character = hitCollider.GetComponent<CharacterBase>();
            if (character != null)
            {
                character.ApplyEffect(new Effect_Stun(3f));
            }
        }

        hasExploded = true;
        animator.SetTrigger("toExplode");
    }
}
