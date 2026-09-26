using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletHelpCaller : MonoBehaviour
{
    private CircleCollider2D col;
    private GameObject parent;
    public List<EnemyClass> zombiesAround = new List<EnemyClass>();

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        parent = transform.parent.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyClass enemyScript = collision.gameObject.GetComponent<EnemyClass>();
        if (enemyScript is ZombieEnemy zombie)
        {
            zombiesAround.Add(zombie);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyClass enemyScript = collision.gameObject.GetComponent<EnemyClass>();
        if (enemyScript is ZombieEnemy zombie)
        {
            zombiesAround.Remove(zombie);
        }
    }
}