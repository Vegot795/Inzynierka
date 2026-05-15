using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShotgunScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject muzzle;

    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    public float damage = 10f;
    public float fireRate = 0.5f;
    public int maxAmmo;
    public int currentAmmo;
    public float reloadTime = 3f;
    public float range = 5f;
    public float shotDisplayTime = 0.1f;
    

    private Vector3 muzzlePos;
    private Vector3 spawnPos;
    private WeaponData weaponData;
    public SpriteRenderer muzzleSR;


    public void Initialize(WeaponData data)
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

    }
    public void Shoot(Vector2 direction)
    {
        StartCoroutine(ShowShot(shotDisplayTime));
        var enemiesInRange = GetComponentInChildren<ShotgunShotScript>().enemiesInRange;

        if (enemiesInRange != null)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                enemy.GetComponent<CharacterBase>().TakeDamage(damage);
            }
        }
        currentAmmo--;

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

    public IEnumerator DelayReload(float time)
    {
        yield return new WaitForSeconds(time);
        Reload();
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
    }
}
