using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public int damage;
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
    public AudioSource AS;
    public AudioClip explosionAudio;
    public GameSettings GS;
    public Rigidbody2D rb;
    float timeToExplode;
    bool cdStarted = false;


    public virtual void Initialize(GrenadeData data, Vector2 target)
    {
        grenadeData = data;

        damage = data.damage;
        explosionRadius = data.explosionRadius;
        moveSpeed = data.moveSpeed;
        grenadeType = data.grenadeType;
        targetPosition = target;
        playerController = GetComponentInParent<CharacterBase>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        var distance = Vector2.Distance(targetPosition, (Vector2)transform.position);

        timeToExplode = distance / (moveSpeed);
        Debug.Log($"Distance: {distance} | Speed: {moveSpeed} | time: {timeToExplode}");

        //Debug.Log($"Grenade initialized with target position: {targetPosition}, damage: {damage}, explosion radius: {explosionRadius}, move speed: {moveSpeed}");
    }

    /*private void FixedUpdate()
    {
        if (timeToExplode > 0)
        {
            timeToExplode -= Time.fixedDeltaTime;
        }

        if (timeToExplode <= 0)
        {
            Explode();
        }
    }*/
    private void FixedUpdate()
    {
        if (hasExploded)
        {
            return;
        }

        if (GS == null)
        {
            GS = GameSettings.GS;
        }

        AS.volume = GS.audioFloat;

        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPosition - currentPos).normalized;

        float step = moveSpeed * Time.deltaTime;
        if (rb != null)
        {
            rb.MovePosition(Vector2.MoveTowards(currentPos, targetPosition, step));
        }

        if (!cdStarted)
        {
            StartCoroutine(WaitToExplode(timeToExplode));
        }
    }

    public virtual void Explode()
    {
        AS.PlayOneShot(explosionAudio);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    public virtual void DestroyGrenade()
    {
        Destroy(gameObject); 
    }

    public void TurnOffSR()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public IEnumerator WaitToDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public IEnumerator WaitToExplode(float time)
    {
        cdStarted = true;
        yield return new WaitForSeconds(time);
        Explode();
    }
}
