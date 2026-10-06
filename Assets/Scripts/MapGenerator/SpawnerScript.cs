using System.Collections;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class SpawnerScript : MonoBehaviour
{
    public GameControllerScript GCS;
    public bool canSpawnEnemies = true;
    public bool isUnlocked = false;
    public bool openingEnded = false;
    public MapGenerator.Directions direction;
    public GameObject rightWing;
    public GameObject leftWing;
    public float RWr;
    public float RWrb;
    public float LWr;
    public float LWrb;
    public float openedDoorRotation = 120f;
    public float doorRotationSpeed = 90;
    public float totalDoorRotation;

    public void Start()
    {
        GCS = GameControllerScript.GCS;
        RWr = rightWing.transform.localEulerAngles.z;
        RWrb = rightWing.transform.localEulerAngles.z;
        LWr = leftWing.transform.localEulerAngles.z;
        LWrb = leftWing.transform.localEulerAngles.z;
    }
    //zmarnowałem 4 do 6 godzin sprawiając żeby drzwi obracały się jak trzeba...
    public void FixedUpdate()
    {

        if (totalDoorRotation <= openedDoorRotation && isUnlocked)
        {
            totalDoorRotation += doorRotationSpeed * Time.fixedDeltaTime;

        }
        if (isUnlocked && !openingEnded)
        {
            RWr = Mathf.Clamp(RWrb + totalDoorRotation, 0, RWrb + openedDoorRotation);
            LWr = Mathf.Clamp(LWrb + totalDoorRotation, 0, LWrb + openedDoorRotation);
            rightWing.transform.localEulerAngles =  new Vector3(0, 0, RWr);
            leftWing.transform.localEulerAngles = new Vector3(0, 0, -LWr);
        }

        if (RWr >= RWrb + openedDoorRotation &&
            LWr >= LWrb + openedDoorRotation)
        {
            openingEnded = true;
        }

    }

    public int GetRotation()
    {
        int cusRotation = 0;
        switch (direction)
        {
            case MapGenerator.Directions.Up:
                cusRotation = 0;
                break;
            case MapGenerator.Directions.Left:
                cusRotation = 90;
                break;
            case MapGenerator.Directions.Down:
                cusRotation = 180;
                break;
            case MapGenerator.Directions.Right:
                cusRotation = 270;
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
        enemy.transform.rotation = transform.rotation;
        yield return new WaitForSeconds(timeBetweenSpawns);
        canSpawnEnemies = true;
    }
}
