using System.Collections.Generic;
using UnityEngine;

public class KasketGrenade : GrenadeScript
{
    public GameObject miniGrenadePrefab;
    public override void Initialize(GrenadeData data, Vector2 target)
    {
        base.Initialize(data, target);
        miniGrenadePrefab = data.miniGrenadePrefab;
    
    }

    public override void Explode()
    {
        if (hasExploded)
        {
            return;
        }
        base.Explode();
        List<GameObject> kasketObjects = new List<GameObject>();

        if (grenadeType == GrenadeType.Kasket)
        {

            for (int i = 0; i < miniGrenCount; i++)
            {
                GameObject miniGrenade = Instantiate(miniGrenadePrefab, transform.position, Quaternion.identity);
                if (miniGrenade == null)
                {
                    //Debug.LogWarning($"Failed to instantiate mini grenade.");
                }
                kasketObjects.Add(miniGrenade);
            }
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D hitCollider in hitColliders)
        {
           hitCollider.GetComponent<CharacterBase>()?.TakeDamage(damage, playerController);
        }

        GrenadeData miniGren = ScriptableObject.CreateInstance<GrenadeData>();

        miniGren.grenadeName = $"{grenadeData.grenadeName}_Mini";
        miniGren.damage = this.damage / 2;
        miniGren.explosionRadius = this.explosionRadius / kasketObjects.Count;
        miniGren.moveSpeed = this.moveSpeed / 3f;
        miniGren.prefab = grenadeData.miniGrenadePrefab;
        miniGren.grenadeType = GrenadeType.Mini;

        foreach (GameObject miniGrenade in kasketObjects)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 targetPos = (Vector2)transform.position + randomDirection * explosionRadius;

            FragGrenade GS = miniGrenade.GetComponent<FragGrenade>();
            if (GS != null)
            {
                GS.Initialize(miniGren, targetPos);
                GS.playerController = playerController;
            }
        }

        Destroy(miniGren);
        hasExploded = true;
        animator.SetTrigger("toExplode");
    }

}