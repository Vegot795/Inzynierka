using System.Collections;
using UnityEngine;

public class RiffleScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject muzzle;


    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    public float damage = 10f;
    public float fireRate = 0.1f;
    public int maxAmmo;
    public int currentAmmo;
    public float reloadTime = 3f;

    private Vector3 muzzlePos;
    private Vector3 spawnPos;
    private WeaponData weaponData;
    private float lastFireTime;
    private bool isReloading = false;
    public void Initialize(WeaponData data)
    {
        Debug.Log($"Initializing Riffle with data: {data.weaponName}, Max Ammo: {data.maxAmmoCapacity}");
        weaponData = data;
        
        maxAmmo = weaponData.maxAmmoCapacity;
        reloadTime = weaponData.reloadTime;
        damage = weaponData.damage;
        projectileSpeed = weaponData.projectileSpeed;
        fireRate = weaponData.fireRate;
        projectilePrefab = weaponData.projectilePrefab;

        Reload();
        lastFireTime = -fireRate; // Allow immediate first shot
    }

    public bool CanShoot()
    {
        return currentAmmo > 0 
            && !isReloading 
            && Time.time >= lastFireTime + fireRate;
    }

    public void Shoot(Vector2 direction)
    {
        
        if (!CanShoot())
        {
            return;
        }

        Vector3 spawnPos = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y, 0);

        Vector2 shootDirection = direction.normalized;
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
        
        if (newProjectile == null)
        {
            return;
        }


        GameObject player = transform.parent.parent.gameObject;

        rb.linearVelocity = shootDirection * projectileSpeed;

        currentAmmo--;
        lastFireTime = Time.time;

        if (currentAmmo == 0)
        {
            StartCoroutine(DelayReload(reloadTime));
        }
    }

    public IEnumerator DelayReload(float time)
    {
        isReloading = true;
        Debug.Log($"[RiffleScript] Starting reload for {time} seconds...");
        yield return new WaitForSeconds(time); 
        Reload();
        isReloading = false;
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log($"[RiffleScript] Reloaded. Current ammo: {currentAmmo}");
    }
}
