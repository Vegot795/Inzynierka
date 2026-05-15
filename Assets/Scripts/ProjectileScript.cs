using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float SDT = 3f;
    public float damage;
    public float time;

    private CharacterBase cb;
    private WeaponData HeldWeapon;

    private void Start()
    {
        cb = GetComponent<CharacterBase>();
        HeldWeapon = transform.parent.GetComponent<WeaponData>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log($"[Projectile] Hit enemy: {collision.gameObject.name}");
            cb.TakeDamage(HeldWeapon.damage);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= SDT) 
        {
            Destroy(gameObject);
        }
    }
}
