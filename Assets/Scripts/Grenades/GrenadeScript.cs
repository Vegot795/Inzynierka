using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public float damage;
    public float explosionRadius;
    public float moveSpeed;
    public bool isThrown = false;
    public int miniGrenCount = 8;
    public Animator animator;

    private Vector2 targetPosition;
    private bool hasExploded = false;
    private GrenadeData grenadeData;
    private GrenadeType grenadeType;
    public GameObject miniGrenadePrefab;


    public void Initialize(GrenadeData data, Vector2 target)
    {
        grenadeData = data;

        damage = data.damage;
        explosionRadius = data.explosionRadius;
        moveSpeed = data.moveSpeed;
        grenadeType = data.grenadeType;
        targetPosition = target;
        miniGrenadePrefab = data.miniGrenadePrefab;

        animator = GetComponent<Animator>();


        Debug.Log($"Grenade initialized with target position: {targetPosition}, damage: {damage}, explosion radius: {explosionRadius}, move speed: {moveSpeed}");
    }
    private void Update()
    {
        if (hasExploded)
        {
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPosition - currentPos).normalized;

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(currentPos, targetPosition, step);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        switch (grenadeType)
        {
            case GrenadeType.Mini:
                Frag();
                break;
            case GrenadeType.Frag:
                Frag();
                break;
            case GrenadeType.Stun:
                Stun();
                break;
            case GrenadeType.Kasket:
                Kasket();
                break;
            default:
                Debug.LogWarning($"Unknown grenade type: {grenadeType}");
                break;
        }
    }

    private void Frag()
    {
        if (hasExploded)
        {
            return;
        }
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            hitCollider.GetComponent<CharacterBase>()?.TakeDamage(damage);
            //Debug.Log($"Object {hitCollider.name} is within explosion radius and takes {damage} damage.");
        }

        hasExploded = true;
        animator.SetTrigger("toExplode");
    }
    private void Stun() 
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
#region --- Kasket Script ---
    private void Kasket()
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
                hitCollider.GetComponent<CharacterBase>()?.TakeDamage(damage);
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

#endregion ----------------------------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    public void DestroyGrenade()
    {
        Destroy(gameObject); 
    }

    public void TurnOffSR()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }
}
