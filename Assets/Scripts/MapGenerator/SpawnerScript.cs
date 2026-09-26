using System.Collections;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class SpawnerScript : MonoBehaviour
{
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
        canSpawnEnemies = false;
        var enemy = Instantiate(enemyPrefab);
        enemy.transform.position = transform.position;
        enemy.transform.rotation = Quaternion.Euler(0, 0, rotation);
        yield return new WaitForSeconds(timeBetweenSpawns);
        canSpawnEnemies = true;
    }
}
