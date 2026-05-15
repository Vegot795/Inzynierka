using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform weaponAttachPoint;
    private GameObject currentWeaponInstance;

    // Expose the instantiated weapon
    public GameObject CurrentWeaponInstance => currentWeaponInstance;

    public void EquipWeapon(WeaponData data)
    {
        UnequipWeapon();
        if (data.prefab == null)
        {
            Debug.LogWarning($"[WeaponHolder] {data.weaponName} prefab nieprzypisany.");
            return;
        }

        currentWeaponInstance = Instantiate(data.prefab, weaponAttachPoint);
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;

        RiffleScript rifle = currentWeaponInstance.GetComponent<RiffleScript>();
        if (rifle != null)
        {
            rifle.Initialize(data);
        }
        
        ShotgunScript shotgun = currentWeaponInstance.GetComponent<ShotgunScript>();
        if (shotgun != null)
        {
            shotgun.Initialize(data);
        }
    }

    public void UnequipWeapon()
    {
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
    }
}
