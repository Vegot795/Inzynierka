using UnityEngine;

public class Effect_Flash : StatusEffect
{
public Effect_Flash(float duration) : base(duration)
    {

    }

    public override void OnApply(CharacterBase character)
    {
        character.isFlashed = true;
    }
    public override void OnTick(CharacterBase character)
    {
      
    }
    public override void OnRemove(CharacterBase character)
    {
        character.isFlashed = false;
    }
}
