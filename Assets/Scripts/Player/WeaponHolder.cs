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
            Debug.LogWarning($"[WeaponHolder] {data.weaponName} has no prefab assigned.");
            return;
        }

        currentWeaponInstance = Instantiate(data.prefab, weaponAttachPoint);
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;
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
