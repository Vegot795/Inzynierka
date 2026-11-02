using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShotgunScript : WeaponClass
{
    public GameObject projectilePrefab;
    public GameObject muzzle;


    public SpriteRenderer muzzleSR;


    public override void Initialize(WeaponData data)
    {
        weaponData = data;

        baseMaxAmmo = weaponData.maxAmmoCapacity;
        currentMaxAmmo = (int)(baseMaxAmmo * maxAmmoModif);
        currentReserveAmmo = currentMaxAmmo;
        baseMagAmmo = weaponData.maxMagAmmo;
        maxMagAmmo = (int)(baseMagAmmo * maxAmmoModif);
        baseReloadTime = weaponData.reloadTime;
        currentReloadTime = baseReloadTime;
        baseDamage = weaponData.damage;
        currentDamage = baseDamage;
        baseFireRate = weaponData.fireRate;
        currentFireRate = baseFireRate;
        projectilePrefab = weaponData.projectilePrefab;

        muzzleSR = muzzle.GetComponent<SpriteRenderer>();
        muzzleSR.enabled = false;
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
        StartCoroutine(ShowShot(shotDisplayTime));
        List<GameObject> enemiesInRange = new List<GameObject>();
        enemiesInRange.AddRange(GetComponentInChildren<ShotgunShotScript>().enemiesInRange);

        base.currentAmmo--;
        base.lastFireTime = Time.time;

        if (enemiesInRange != null)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<CharacterBase>().TakeDamage(currentDamage, playerController);
                }
            }
        }

        if (currentAmmo == 0)
        {
            StartCoroutine(DelayReload(currentReloadTime));
        }

    }

    private IEnumerator ShowShot(float time)
    {
        muzzleSR.enabled = true;
        yield return new WaitForSeconds(time);
        muzzleSR.enabled = false;
    }
}

