using System.Collections;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class SpawnerScript : MonoBehaviour
{
    public GameControllerScript GCS;
    public bool canSpawnEnemies = true;
    public bool isUnlocked = false;
    public int rotation;
    public Direction direction;
    public enum Direction
    {
        facingTop, facingRight, facingBottom, facingLeft
    }

    public void Start()
    {
        rotation = GetRotation();
        GCS = GameControllerScript.GCS;
    }

    public int GetRotation()
    {
        int cusRotation = 0;
        switch (direction)
        {
            case Direction.facingTop:
                rotation = 0;
                break;
            case Direction.facingRight:
                rotation = 90;
                break;
            case Direction.facingBottom:
                rotation = 180;
                break;
            case Direction.facingLeft:
                rotation = 270;
                break;
            default:
                Debug.LogWarning("[SpawnerScript] - Quaternion is screwwwwed");
                return 0 ;
        }

        return cusRotation;
    }

    public IEnumerator SpawnEnemiesInSpawner(GameObject enemyPrefab, float timeBetweenSpawns)
    {
        Debug.Log("[Spawner] - function called");
        canSpawnEnemies = false;
        var enemy = Instantiate(enemyPrefab);
        GCS.enemiesSpawned.Add(enemy.GetComponent<EnemyClass>());
        GCS.enemiesSpawnedDuringRound++;
        enemy.transform.position = this.transform.position;
        enemy.transform.rotation = Quaternion.Euler(0, 0, rotation);
        yield return new WaitForSeconds(timeBetweenSpawns);
        canSpawnEnemies = true;
    }
}
