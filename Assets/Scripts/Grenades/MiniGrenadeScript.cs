using UnityEngine;

public class MiniGrenadeScript : FragGrenade
{
    public override void DestroyGrenade()
    {
        var KasketKids = transform.parent.childCount;
        if (KasketKids <= 1)
        {
            Destroy(transform.parent.gameObject);
        }
        base.DestroyGrenade();
    }
}
