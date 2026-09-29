using System.Collections;
using UnityEngine;

public class RiffleScript : WeaponClass
{
    public GameObject projectilePrefab;
    public GameObject muzzle;
    

    public override void Initialize(WeaponData data)
    {
        //Debug.Log($"Initializing Riffle with data: {data.weaponName}, Max Ammo: {data.maxAmmoCapacity}");
        weaponData = data;
        
        currentMaxAmmo = weaponData.maxAmmoCapacity;
        currentReloadTime = weaponData.reloadTime;
        currentDamage = weaponData.damage;
        projectileSpeed = weaponData.projectileSpeed;
        currentFireRate = weaponData.fireRate;
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

        Vector3 spawnPos = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y, 0);

        Vector2 shootDirection = direction.normalized;
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        newProjectile.GetComponent<ProjectileScript>().Initialize(currentDamage, playerController);
        Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
        
        if (newProjectile == null)
        {
            return;
        }


        GameObject player = transform.parent.parent.gameObject;

        rb.linearVelocity = shootDirection * projectileSpeed;

        base.currentAmmo--;
        base.lastFireTime = Time.time;

        if (currentAmmo == 0)
        {
            StartCoroutine(DelayReload(currentReloadTime));
        }
    }



}
