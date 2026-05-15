using UnityEngine;

public class FragGrenadeScript : MonoBehaviour
{
    private float explosionRadius;
    private float damage;

    private void Instantiate(GrenadeData data)
    {
        explosionRadius = data.explosionRadius;
        damage = data.damage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
