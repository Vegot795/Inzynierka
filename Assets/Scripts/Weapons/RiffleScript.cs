using UnityEngine;

public class RiffleScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject muzzle;

    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    public float damage = 10f;

    private Vector3 muzzlePos;
    private Vector3 spawnPos;

    private void Awake()
    {

    }

    public void Shoot(Vector2 direction)
    {
        Vector3 spawnPos = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y, 0);

        Vector2 shootDirection = direction.normalized;
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        newProjectile.GetComponent<ProjectileScript>().damage = damage;
        Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDirection * projectileSpeed;
    }
}
