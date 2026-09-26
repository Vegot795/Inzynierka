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

        List<GameObject> kasketObjects = new List<GameObject>();

        if (grenadeType == GrenadeType.Kasket)
        {
            GameObject kasketBasket = new GameObject("KasketBasket");

            for (int i = 0; i < miniGrenCount; i++)
            {
                GameObject miniGrenade = Instantiate(miniGrenadePrefab, transform.position, Quaternion.identity);
                if (miniGrenade == null)
                {
                    Debug.LogWarning($"Failed to instantiate mini grenade.");
                }
                miniGrenade.AddComponent<GrenadeScript>();
                kasketObjects.Add(miniGrenade);
                miniGrenade.transform.SetParent(kasketBasket.transform);
            }
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D hitCollider in hitColliders)
        {
            CharacterBase character = hitCollider.GetComponent<CharacterBase>();
            if (character != null)
            {
                hitCollider.GetComponent<CharacterBase>()?.TakeDamage(damage, playerController);
                // Debug.Log($"Object {hitCollider.name} is within stun radius and is stunned.");
            }
        }

        foreach (GameObject miniGrenade in kasketObjects)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 targetPos = (Vector2)transform.position + randomDirection * explosionRadius;

            GrenadeScript CS = miniGrenade.GetComponent<GrenadeScript>();
            if (CS != null)
            {
                GrenadeData miniGren = new GrenadeData()
                {
                    grenadeName = $"{grenadeData.grenadeName}_Mini",
                    damage = this.damage / 2,
                    explosionRadius = this.explosionRadius / kasketObjects.Count,
                    moveSpeed = this.moveSpeed / 3f,
                    prefab = grenadeData.miniGrenadePrefab,
                    grenadeType = GrenadeType.Mini
                };

                CS.Initialize(miniGren, targetPos);
            }
        }

        hasExploded = true;
        animator.SetTrigger("toExplode");
    }
}