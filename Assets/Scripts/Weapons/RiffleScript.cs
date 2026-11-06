using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RiffleScript : WeaponClass
{
    [Header("RiffleScript")]

    public GameObject projectilePrefab;
    public LayerMask layerToIgnore;
    public LineRenderer LR;
    [SerializeField] private float range = 100f;
    private Transform startPos;
    private Transform endPos;
    Color lineColor = new Color(1f, 1f, 0, 1f);


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

        startPos = gameObject.transform;
        SetupLR();
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
        StartCoroutine(ShootPath(0.01f, direction));

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

    private void ShowShootPath(Vector2 direction)
    {
        RaycastHit2D hit;

        if (Physics2D.Raycast(muzzle.transform.position, transform.right))
        {
            hit = Physics2D.Raycast(muzzle.transform.position, direction, 1000f, ~layerToIgnore);
            DrawLine(muzzle.transform.position, hit.point);
        }
        else
        {
            //hit = Physics2D.Raycast(muzzle.transform.position, direction, 10000f, ~layerToIgnore);
            DrawLine(muzzle.transform.position, muzzle.transform.right * 1000f);
        }

        //yield return new WaitForSeconds(time);
        //HideLine();
    }

    private void DrawLine(Vector2 startPos, Vector2 endPos)
    {

        LR.SetPosition(0, startPos);
        LR.SetPosition(1, endPos);
    }

    private void HideLine()
    {
        LR.SetPosition(0, Vector2.zero);
        LR.SetPosition(1, Vector2.zero);
    }

    private IEnumerator ShootPath(float time, Vector2 direction)
    {
        ShowShootPath(direction);
        yield return new WaitForSeconds(time);
        HideLine();
    }

    private void SetupLR()
    {
        LR.startColor = lineColor;
        LR.endColor = lineColor;
        LR.startWidth = 0.02f;
        LR.endWidth = LR.startWidth;
    }

}
