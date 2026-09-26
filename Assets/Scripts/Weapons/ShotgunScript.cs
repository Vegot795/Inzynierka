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

        maxAmmo = weaponData.maxAmmoCapacity;
        reloadTime = weaponData.reloadTime;
        damage = weaponData.damage;
        projectileSpeed = weaponData.projectileSpeed;
        fireRate = weaponData.fireRate;
        projectilePrefab = weaponData.projectilePrefab;

        currentAmmo = maxAmmo;
        muzzleSR = muzzle.GetComponent<SpriteRenderer>();
        muzzleSR.enabled = false;
        playerController = GetComponentInParent<CharacterBase>();

    }
    public override void Shoot(Vector2 direction)
    {
        if (!base.CanShoot())
        {
            return;
        }

        StartCoroutine(ShowShot(shotDisplayTime));
        var enemiesInRange = GetComponentInChildren<ShotgunShotScript>().enemiesInRange;

        if (enemiesInRange != null)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                enemy.GetComponent<CharacterBase>().TakeDamage(damage, playerController);
            }
        }
        base.currentAmmo--;
        base.lastFireTime = Time.time;

        if (currentAmmo == 0)
        {
            StartCoroutine(DelayReload(reloadTime));
        }

    }

    private IEnumerator ShowShot(float time)
    {
        muzzleSR.enabled = true;
        yield return new WaitForSeconds(time);
        muzzleSR.enabled = false;
    }
}

