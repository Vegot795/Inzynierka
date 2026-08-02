using UnityEngine;
using System.Collections;

public class WeaponClass : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    public float damage = 10f;
    public float fireRate = 0.5f;
    public int maxAmmo;
    public int currentAmmo;
    public float reloadTime = 3f;
    public float range = 5f;
    public float shotDisplayTime = 0.1f;
    public bool isReloading = false;
    public float lastFireTime = 0f;


    private Vector3 muzzlePos;
    private Vector3 spawnPos;
    public WeaponData weaponData;


    public virtual void Initialize(WeaponData data)
    {

    }

    public virtual void Shoot(Vector2 direction)
    {
        return;
    }
    public bool CanShoot()
    {
        return currentAmmo > 0
            && !isReloading
            && Time.time >= lastFireTime + fireRate;
    }

    public virtual IEnumerator DelayReload(float time)
    {
        isReloading = true;
        Debug.Log($"[RiffleScript] Starting reload for {time} seconds...");
        yield return new WaitForSeconds(time);
        Reload();
        isReloading = false;
    }

    public virtual void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log($"[RiffleScript] Reloaded. Current ammo: {currentAmmo}");
    }
}
