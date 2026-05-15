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
    public float grenadeThrowRange = 2f;

    public WeaponData CurrentWeapon => currentWeapon;
    public GrenadeData CurrentGrenade => currentGrenade;
    public WeaponData startingWeapon;
    public GrenadeData stastingGrenade;
    public RiffleScript RS;
    public ShotgunScript SS;
    public PC_Controller PC;
    public Animator animator;

    private bool isAimingGrenade = false;

    private GrenadeGizmoController gizmoController;
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
        
        gizmoController = gameObject.AddComponent<GrenadeGizmoController>();

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
            gizmoController.UpdateTargetPosition(targetPos);
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
        Debug.Log("Grenade button pressed");
        OnGrenadeHoldStart();
    }

    private void OnGrenadeReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Grenade button released");
        OnGrenadeHoldEnd();
    }

    public void OnGrenadeHoldStart()
    {
        if (currentGrenade == null) return;

        isAimingGrenade = true;
        Vector2 targetPos = GetClampedGrenadeTarget();
        gizmoController.ShowGizmos(grenadeThrowRange, currentGrenade.explosionRadius, targetPos);
        
        Debug.Log($"[Inventory] Started aiming grenade");
    }
    public void OnGrenadeHoldEnd()
    {

        Debug.Log($"OnGrenadeHoldEnd");
        if (!isAimingGrenade || currentGrenade == null) return;

        isAimingGrenade = false;
        gizmoController.HideGizmos();
        
        Debug.Log($"[Inventory] Threw grenade");
        ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        Debug.Log($"[ThrowGrenade] Starting throw...");
        
        if (currentGrenade == null)
        {
            Debug.LogError("[ThrowGrenade] currentGrenade is null!");
            return;
        }

        if (currentGrenade.prefab == null)
        {
            Debug.LogError($"[ThrowGrenade] Grenade prefab is null on {currentGrenade.grenadeName}!");
            return;
        }

        Vector2 targetPos = GetClampedGrenadeTarget();
        Debug.Log($"[ThrowGrenade] Target position: {targetPos}");
        
        GameObject grenadeObj = Instantiate(currentGrenade.prefab, transform.position, Quaternion.identity);
        
        if (grenadeObj == null)
        {
            Debug.LogError("[ThrowGrenade] Failed to instantiate grenade!");
            return;
        }
        
        Debug.Log($"[ThrowGrenade] Grenade instantiated: {grenadeObj.name}");
        
        GrenadeScript grenadeScript = grenadeObj.GetComponent<GrenadeScript>();
        
        if (grenadeScript == null)
        {
            Debug.LogError($"[ThrowGrenade] GrenadeScript component NOT FOUND on prefab {grenadeObj.name}! Add GrenadeScript component to the grenade prefab.");
            Destroy(grenadeObj);
            return;
        }
        
        Debug.Log($"[ThrowGrenade] GrenadeScript found, calling Initialize...");
        grenadeScript.Initialize(currentGrenade, targetPos);

        SpriteRenderer sr = grenadeObj.GetComponent<SpriteRenderer>();
        if (sr != null && currentGrenade.grenadeSprite != null)
        {
            sr.sprite = currentGrenade.grenadeSprite;
        }

        Debug.Log($"[Inventory] Threw grenade to {targetPos}");
        
        currentGrenade = null;
    }

    private Vector2 GetClampedGrenadeTarget()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        
        Vector2 direction = (mouseWorldPos - transform.position);
        float distance = direction.magnitude;
        
        if (distance > grenadeThrowRange)
        {
            direction = direction.normalized * grenadeThrowRange;
        }
        
        return (Vector2)transform.position + direction;
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
            Debug.LogWarning("[PlayerInventory] Weapon instance is null!");
            return;
        }

        if (weapon.weaponType == WeaponType.Rifle)
        {
            RS = weaponInstance.GetComponent<RiffleScript>();
            animator = weaponInstance.GetComponent<Animator>();
            
            if (RS == null)
                Debug.LogWarning("[PlayerInventory] RiffleScript component not found on weapon!");
        }
        else if (weapon.weaponType == WeaponType.Shotgun)
        {
            SS = weaponInstance.GetComponent<ShotgunScript>();
            animator = weaponInstance.GetComponent<Animator>();
            
            if (SS == null)
                Debug.LogWarning("[PlayerInventory] ShotgunScript component not found on weapon!");
        }

        if (animator == null)
        {
            Debug.LogWarning("[PlayerInventory] Animator component not found on weapon. Animations will be skipped.");
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
