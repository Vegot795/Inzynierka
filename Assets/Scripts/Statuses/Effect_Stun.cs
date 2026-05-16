using UnityEngine;

public class Effect_Stun : StatusEffect
{
    public Effect_Stun(float duration) : base(duration)
    {

    }

    public override void OnApply(CharacterBase character)
    {
        character.isStunned = true;
    }
    public override void OnTick(CharacterBase character)
    {
     
    }

    public override void OnRemove(CharacterBase character)
    {
        character.isStunned = false;
    }
}
