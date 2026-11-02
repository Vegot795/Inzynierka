using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RiffleScript : WeaponClass
{
    public GameObject projectilePrefab;
    public GameObject muzzle;
    public LayerMask layerToIgnore;

    public override void Initialize(WeaponData data)
    {
        //Debug.Log($"Initializing Riffle with data: {data.weaponName}, Max Ammo: {data.maxAmmoCapacity}");
        weaponData = data;
        
        baseMaxAmmo = weaponData.maxAmmoCapacity;
        baseMagAmmo = weaponData.maxMagAmmo;
        currentMaxAmmo = (int)(baseMaxAmmo * maxAmmoModif);
        currentReserveAmmo = currentMaxAmmo + maxMagAmmo;
        maxMagAmmo = (int)(baseMagAmmo * maxAmmoModif);
        currentAmmo = maxMagAmmo;
        baseReloadTime = weaponData.reloadTime;
        currentReloadTime = baseReloadTime; 
        baseDamage = weaponData.damage;
        currentDamage = baseDamage;
        projectileSpeed = weaponData.projectileSpeed;
        baseFireRate = weaponData.fireRate;
        currentFireRate = baseFireRate;
        projectilePrefab = weaponData.projectilePrefab;
        playerController = GetComponentInParent<CharacterBase>();


        Reload();
        base.lastFireTime = -currentFireRate;
    }

    public override void Shoot(Vector2 direction)
    {
        if (!base.CanShoot())
        {
            return;
        }

        base.Shoot(direction);
        Vector3 spawnPos = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y, 0);

        Vector2 shootDirection = direction.normalized;

        RaycastHit2D hit = Physics2D.Raycast(muzzle.transform.position, direction, 1000f, ~layerToIgnore);
        
        CharacterBase character = hit.collider.GetComponent<CharacterBase>();
        Debug.DrawLine(muzzle.transform.position, hit.transform.position);

        base.currentAmmo--;
        base.lastFireTime = Time.time;

        if (character != null && character is EnemyClass)
        {
            character.TakeDamage(currentDamage, playerController);
        }

        if (currentAmmo == 0)
        {
            StartCoroutine(DelayReload(currentReloadTime));
        }
    }



}
