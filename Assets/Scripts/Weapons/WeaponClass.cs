using UnityEngine;
using System.Collections;

public class WeaponClass : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    public float baseDamage = 10f;
    public float damageModif = 1;
    public float currentDamage;
    public float baseFireRate = 0.5f;
    public float fireRateModif = 1;
    public float currentFireRate;
    public int baseMaxAmmo;
    public float maxAmmoModif = 1;
    public int currentAmmo;
    public int currentMaxAmmo;
    public float baseReloadTime = 3f;
    public float reloadTimeModif = 1f;
    public float currentReloadTime;
    public float shotDisplayTime = 0.1f;
    public bool isReloading = false;
    public float lastFireTime = 0f;


    private Vector3 muzzlePos;
    private Vector3 spawnPos;
    public WeaponData weaponData;
    public CharacterBase playerController;


    public virtual void Initialize(WeaponData data) { }

    public virtual void Shoot(Vector2 direction) { }

    public bool CanShoot()
    {
        return currentAmmo > 0
            && !isReloading
            && Time.time >= lastFireTime + currentFireRate;
    }

    public virtual IEnumerator DelayReload(float time)
    {
        isReloading = true;
        //Debug.Log($"[RiffleScript] Starting reload for {time} seconds...");
        yield return new WaitForSeconds(time);
        Reload();
        isReloading = false;
    }

    public virtual void Reload()
    {
        currentAmmo = currentMaxAmmo;

        
        //Debug.Log($"[RiffleScript] Reloaded. Current ammo: {currentAmmo}");
    }

    public void OnMagBoofStatusChange()
    {
        currentMaxAmmo = (int)(baseMaxAmmo * maxAmmoModif);
    }

    public void OnDamageBoofStatusChange()
    {
        currentDamage = baseDamage * damageModif;
    }
}
