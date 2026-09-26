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
    public bool hasExploded { get; protected set; } = false;

    public Vector2 targetPosition;
    public GrenadeData grenadeData;
    public GrenadeType grenadeType;
    public CharacterBase playerController;



    public virtual void Initialize(GrenadeData data, Vector2 target)
    {
        grenadeData = data;

        damage = data.damage;
        explosionRadius = data.explosionRadius;
        moveSpeed = data.moveSpeed;
        grenadeType = data.grenadeType;
        targetPosition = target;
        playerController = GetComponentInParent<CharacterBase>();


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

    public virtual void Explode()
    {
        return;
    }

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
