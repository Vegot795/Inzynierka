using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [Header("Scripts")]
    public static GameControllerScript GCS { get; private set; }
    public MapGenerator mapGenerator;
    public PC_Controller pcController;

    [Header("Prefabs")]
    public GameObject PlayerPrefab;
    public GameObject SkeletPrefab;
    public GameObject ZombiePrefab;

    [Header("Objects")]
    public GameObject MainCamera;
    public GameObject CinemachineCamera;
    public GameObject playerObj;

    [Header("Counters")]
    public int WaveNumber;
    public int MaxEnemies;
    public int AddMaxEnemiesPerWave;

    [Header("Specs")]
    public bool canSpawnEnemies;
    public float timeBetweenSpawns = 3f;

    [Header("Lists")]
    public List<EnemyClass> enemiesSpawned = new List<EnemyClass>();
    public List<SpawnerScript> allSpawners;


   // [Header("Settings")]

    private void SetupGC()
    {

        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();

        if (GCS == null)
        {
            GCS = this;
        }

        if (mapGenerator == null)
        {
            Debug.LogWarning("MapGenerator has not been assigned");
        }

        if (PlayerPrefab == null)
        {
            Debug.LogWarning("PlayerPrefab has not been assigned");
        }

        if (SkeletPrefab == null)
        {
            Debug.LogWarning("SkeletPrefab has not been assigned");
        }

        if (ZombiePrefab == null)
        {
            Debug.LogWarning("ZombiePrefab has not been assigned");
        }
    }

    public void Awake()
    {
        SetupGC();
    }

    public void Update()
    {
        if (CanGameSpawnEnemies())
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    public void SetupGame(Room startRoom)
    {
        WaveNumber = 1;
        RectInt SRBounds = startRoom.bounds;

        Vector2 startCellCoord = new Vector2(Mathf.RoundToInt(SRBounds.center.x), Mathf.RoundToInt(SRBounds.center.y));
        GridCell startCell = mapGenerator.cellsList
            .Where(a => a.transform.position.x == startCellCoord.x && a.transform.position.y == startCellCoord.y)
            .FirstOrDefault();

        GameObject playerObj = Instantiate(PlayerPrefab);
        playerObj.transform.position = SRBounds.center;
        var CineCamComp = CinemachineCamera.GetComponent<CinemachineCamera>();
        CineCamComp.Target.TrackingTarget = playerObj.transform;

    }

    public bool CanGameSpawnEnemies()
    {
        return (enemiesSpawned.Count < MaxEnemies);
    }

    private List<SpawnerScript> GetAvailableSpawners()
    {
        List<SpawnerScript> availalbeSpawners = allSpawners
                                                    .Where(x => x.isUnlocked == true)
                                                    .ToList();

        return availalbeSpawners;
    }

    public IEnumerator SpawnEnemies()
    {
        List<SpawnerScript> spawnersToUse = GetAvailableSpawners();

        var currentSpawner = spawnersToUse.OrderBy(_ => Random.value)
                                          .FirstOrDefault();

        GameObject enemyToSpawn;
        int enemySpawnChance = Random.Range(0, 10);
        if (enemySpawnChance == 10)
        {
            enemyToSpawn = SkeletPrefab;
        }
        else
        {
            enemyToSpawn = ZombiePrefab;
        }
        currentSpawner.StartCoroutine(currentSpawner.SpawnEnemiesInSpawner(enemyToSpawn, 0.5f));

        yield return new WaitForSeconds(0.2f);

    }


}
