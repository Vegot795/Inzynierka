using UnityEngine;
using System.Collections;

public class WeaponClass : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    //Damage
    public int baseDamage = 10;
    public float damageModif = 1;
    public int currentDamage;
    //fire rate
    public float baseFireRate = 0.5f;
    public float fireRateModif = 1;
    public float currentFireRate;
    //ammo
    public float maxAmmoModif = 1;
    public int baseMaxAmmo; //base max ammo
    public int currentMaxAmmo; // max ammo after modifier
    public int currentAmmo; // ammo in mag
    public int maxMagAmmo; //max mag ammo
    public int baseMagAmmo = 25;
    public int currentReserveAmmo; // ammo in reserve
    //reload
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
        var ammoToLoad = Mathf.Min(maxMagAmmo, currentReserveAmmo);
        currentAmmo += ammoToLoad;
        currentReserveAmmo -= ammoToLoad;        
        //Debug.Log($"[RiffleScript] Reloaded. Current ammo: {currentAmmo}");
    }

    public void OnMagBoofStatusChange()
    {
        currentMaxAmmo = (int)(baseMaxAmmo * maxAmmoModif);
        maxMagAmmo = (int)(baseMagAmmo * maxAmmoModif);
    }

    public void OnDamageBoofStatusChange()
    {
        currentDamage = (int)(baseDamage * damageModif);
    }
}
