using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Current Slots (runtime, readonly)")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GrenadeData currentGrenade;

    [Header("References")]
    public WeaponHolder weaponHolder;

    [Header("Drop Settings")]
    public float dropDistance = 0.8f;

    [Header("Constants")]
    public float grenadeThrowRange = 20f;

    public WeaponData CurrentWeapon => currentWeapon;
    public GrenadeData CurrentGrenade => currentGrenade;
    public WeaponData startingWeapon;
    public GrenadeData stastingGrenade;
    public RiffleScript RS;
    public ShotgunScript SS;
    public PC_Controller PC;
    public Animator animator;
    public GrenadeGizmoController indicator;

    private bool isAimingGrenade = false;
    private PlayerInput playerInput;
    private InputAction grenadeAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            grenadeAction = playerInput.actions["ThrowGrenade"];
            if (grenadeAction != null)
            {
                grenadeAction.started += OnGrenadePressed;
                grenadeAction.canceled += OnGrenadeReleased;
            }
        }
        indicator = GetComponent<GrenadeGizmoController>();
    }

    private void OnDestroy()
    {
        if (grenadeAction != null)
        {
            grenadeAction.started -= OnGrenadePressed;
            grenadeAction.canceled -= OnGrenadeReleased;
        }
    }

    private void Start()
    {
        PC = GetComponent<PC_Controller>();
        
        indicator = gameObject.GetComponent<GrenadeGizmoController>();

        if (startingWeapon != null)
        {
            currentWeapon = startingWeapon;
            weaponHolder.EquipWeapon(currentWeapon);
            SetWeaponScriptReferences(currentWeapon);
        }

        if (stastingGrenade != null)
        {
            currentGrenade = stastingGrenade;
        }
    }

    private void Update()
    {
        if (isAimingGrenade && currentGrenade != null)
        {
            Vector2 targetPos = GetClampedGrenadeTarget();
        }


    }

    #region --- Pickup/Drop ---
    public void PickupWeapon(WeaponPickup pickup)
    {
        if (currentWeapon != null)
        {
            DropWeapon();
        }

        currentWeapon = pickup.weaponData;
        weaponHolder.EquipWeapon(currentWeapon);
        SetWeaponScriptReferences(currentWeapon);
        Destroy(pickup.gameObject);

        Debug.Log($"[Inventory] Equipped weapon: {currentWeapon.weaponName}");
    }

    public void PickupGrenade(GrenadePickup pickup)
    {
        if (currentGrenade != null)
        {
            DropGrenade();
        }
        currentGrenade = pickup.grenadeData;
        Destroy(pickup.gameObject);

        Debug.Log($"[Inventory] Picked up grenade: {currentGrenade.grenadeName}");
    }

    private void DropWeapon()
    {
        Vector2 dropPos = (Vector2)transform.position + GetDropOffset();
        var weaponPickup = WeaponPickup.SpawnFromData(currentWeapon, dropPos);
        currentWeapon = null;
        weaponHolder.UnequipWeapon();
    }

    private void DropGrenade()
    {
        Vector2 dropPos = (Vector2)transform.position + GetDropOffset();
        GrenadePickup.SpawnFromData(currentGrenade, dropPos);
        currentGrenade = null;
    }
    #endregion-----------------------------------------------
    

    
    #region --- Combat ---
    public void OnAttack()
    {
        if (currentWeapon == null) return;

        if (currentWeapon.weaponType == WeaponType.Rifle)
        {
            if (RS != null && RS.CanShoot())
            {
                RS.Shoot(PC.mousePos);
                PlayAnimation("ShootingRiffle");
            }
            else if (RS != null && RS.currentAmmo <= 0)
            {
                RS.StartCoroutine(RS.DelayReload(RS.reloadTime));
            }
        }
        else if (currentWeapon.weaponType == WeaponType.Shotgun)
        {
            if (SS != null && SS.currentAmmo > 0)
            { 
                SS.Shoot(PC.mousePos);
                PlayAnimation("Shoot");
            }
            else if (SS != null && SS.currentAmmo <= 0)
            {
                SS.StartCoroutine(SS.DelayReload(SS.reloadTime));
            }
        }
    }

    private void OnGrenadePressed(InputAction.CallbackContext context)
    {
        //Debug.Log("Grenade button pressed");
        OnGrenadeHoldStart();
        indicator.ShowIndicator();

    }

    private void OnGrenadeReleased(InputAction.CallbackContext context)
    {
        //Debug.Log("Grenade button released");
        OnGrenadeHoldEnd();
        indicator.HideIndicator();
    }

    public void OnGrenadeHoldStart()
    {
        if (currentGrenade == null) return;

        isAimingGrenade = true;
        Vector2 targetPos = GetClampedGrenadeTarget();
        
        
        Debug.Log($"[Inventory] Started aiming grenade");
    }
    public void OnGrenadeHoldEnd()
    {

        if (!isAimingGrenade || currentGrenade == null) return;

        isAimingGrenade = false;
        
        ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        
        if (currentGrenade == null)
        {
            return;
        }

        if (currentGrenade.prefab == null)
        {
            return;
        }

        Vector2 targetPos = GetClampedGrenadeTarget();
        
        GameObject grenadeObj = Instantiate(currentGrenade.prefab, transform.position, Quaternion.identity);        
        if (grenadeObj == null)
        {
            return;
        }
        
        
        GrenadeScript grenadeScript = grenadeObj.GetComponent<GrenadeScript>();
        
        if (grenadeScript == null)
        {
            Destroy(grenadeObj);
            return;
        }
        
        grenadeScript.Initialize(currentGrenade, targetPos);

        SpriteRenderer sr = grenadeObj.GetComponent<SpriteRenderer>();
        if (sr != null && currentGrenade.grenadeSprite != null)
        {
            sr.sprite = currentGrenade.grenadeSprite;
        }
        
        currentGrenade = null;
    }

    private Vector2 GetClampedGrenadeTarget()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        
        Vector2 playerPos = transform.position;
        Vector2 mousePos = mouseWorldPos;
        
        Vector2 direction = mousePos - playerPos;
        float distance = direction.magnitude;
        
        Vector2 targetPos;
        
        if (distance > grenadeThrowRange)
        {
            targetPos = playerPos + direction.normalized * grenadeThrowRange;
        }
        else
        {
            targetPos = mousePos;
        }


        return targetPos;
    }
    #endregion-----------------------------------------------

    private void SetWeaponScriptReferences(WeaponData weapon)
    {
        RS = null;
        SS = null;
        animator = null;
        
        GameObject weaponInstance = weaponHolder.CurrentWeaponInstance;
        if (weaponInstance == null)
        {
            return;
        }

        if (weapon.weaponType == WeaponType.Rifle)
        {
            RS = weaponInstance.GetComponent<RiffleScript>();
            animator = weaponInstance.GetComponent<Animator>();
        }
        else if (weapon.weaponType == WeaponType.Shotgun)
        {
            SS = weaponInstance.GetComponent<ShotgunScript>();
            animator = weaponInstance.GetComponent<Animator>();
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.SetTrigger(animationName);
        }
    }

    private Vector2 GetDropOffset()
    {
        return Vector2.left * dropDistance;
    }
}
