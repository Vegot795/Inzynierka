using UnityEngine;


public abstract class StatusEffect
{
    public float duration;
    public float timeRemaining;

    public StatusEffect(float duration)
    {
        this.duration = duration;
        this.timeRemaining = duration;
    }

    public abstract void OnApply(CharacterBase character);
    public abstract void OnTick(CharacterBase character);
    public abstract void OnRemove(CharacterBase character);


}
