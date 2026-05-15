using System.Collections.Generic;
using UnityEngine;

public class ShotgunShotScript : MonoBehaviour
{
    public Collider2D col;
    public List<GameObject> enemiesInRange = new List<GameObject>();

    public void Start()
    {
        col = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Enemy"))
        {
            enemiesInRange.Add(target.gameObject);
            Debug.Log($"[ShotgunScript] Enemy {target.gameObject.name} entered range. Total enemies in range: {enemiesInRange.Count}");
        }
    }

    private void OnTriggerExit2D(Collider2D target)
    {
        if (target.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(target.gameObject);
            Debug.Log($"[ShotgunScript] Enemy {target.gameObject.name} exited range. Total enemies in range: {enemiesInRange.Count}");
        }
    }
}
