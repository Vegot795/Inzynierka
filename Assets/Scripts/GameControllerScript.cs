using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    [Header("Scripts")]
    public static GameControllerScript GCS { get; private set; }
    public MapGenerator mapGenerator;
    public PC_Controller pcController;
    public BoofManager BM;
    public PlayerData playerData;
    public GameSettings GS;

    [Header("Kids")]
    public GameObject EnemiesHolder;


    [Header("Prefabs")]
    public GameObject PlayerPrefab;
    public GameObject SkeletPrefab;
    public GameObject ZombiePrefab;
    public GameObject GSprefab;

    [Header("Objects")]
    public GameObject MainCamera;
    public GameObject CinemachineCamera;
    public GameObject playerObj;
    public PlayerData PD;

    [Header("Counters")]
    public int WaveNumber = 1;
    public int BaseMaxEnemies;
    public int MaxEnemies;
    public int AddMaxEnemiesPerWave;
    public int AllEnemiesKilled = 0;
    public int ZombiesKilled = 0;
    public int SkeletsKilled = 0;
    public int enemiesSpawnedDuringRound;
    public int EnemiesKilledDuringRound;
    public int TotalScore = 0;

    [Header("Specs")]
    public bool canSpawnEnemies;
    public float timeBetweenSpawns = 3f;
    public int doorPrice = 300;
    public int doorPricePerUnlockedDoors = 50;
    public int startScore = 0;
    public bool readyToSpawn = true;
    public int? zombiesSpawned;
    public int? skeletsSpawned;
    public float? enemyRatio;

    [Header("Lists")]
    public List<EnemyClass> enemiesSpawned = new List<EnemyClass>();
    public List<SpawnerScript> allSpawners = new List<SpawnerScript>();


   // [Header("Settings")]


    public void Awake()
    {
        SetupGC();
        
    }

    public void Update()
    {
        if (CanGameSpawnEnemies() && canSpawnEnemies)
        {
            Debug.Log("[GCS] - spawn enemies called");
            readyToSpawn = false;
            StartCoroutine(SpawnEnemies());
        }
        if (mapGenerator is not TestMapGenerator)
        {
            WaveController();

        }
    }

    #region ------ Game Setup ------
    public void SetupGame(Room startRoom)
    {
        WaveNumber = 1;
        RectInt SRBounds = startRoom.bounds;

        Vector2 startCellCoord = new Vector2(Mathf.RoundToInt(SRBounds.center.x), Mathf.RoundToInt(SRBounds.center.y));
        GridCell startCell = mapGenerator.cellsList
            .Where(a => a.transform.position.x == startCellCoord.x && a.transform.position.y == startCellCoord.y)
            .FirstOrDefault();

        playerObj = Instantiate(PlayerPrefab);
        pcController = playerObj.GetComponent<PC_Controller>();
        playerObj.transform.position = SRBounds.center;
        GS.player = pcController;

        var CineCamComp = CinemachineCamera.GetComponent<CinemachineCamera>();
        CineCamComp.Target.TrackingTarget = playerObj.transform;
        allSpawners = mapGenerator.spawnerList;
        startRoom.isStartingRoom = true;
        startRoom.UnlockRoom();
        MaxEnemies = BaseMaxEnemies;
        //GivePlayerStartScore(startScore);
        Time.timeScale = 1;
        StartCoroutine(GS.TurnOffLoadingScreen(null));
    }

    public void SetupGC()
    {
        GCS = this;
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        GS = GameSettings.GS;
        if (GS == null)
        {
            GameObject devGameSettings = Instantiate(GSprefab);
            GS = devGameSettings.GetComponent<GameSettings>();
        }
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
        PD = PlayerData.Instance;
    }

    #endregion

    #region ------ Enemy Handler ------
    public bool CanGameSpawnEnemies()
    {
        return ((enemiesSpawnedDuringRound < MaxEnemies) && readyToSpawn);
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
        Debug.Log("[GCS] - spawn enemies called");

        List<SpawnerScript> spawnersToUse = GetAvailableSpawners();

        var currentSpawner = spawnersToUse.OrderBy(_ => Random.value)
                                          .FirstOrDefault();

        GameObject enemyToSpawn;
        int enemySpawnChance = Random.Range(0, 10);

        zombiesSpawned = enemiesSpawned.Where(x => x.GetComponent<ZombieEnemy>()).Count();
        skeletsSpawned = enemiesSpawned.Where(x => x.GetComponent<SkeletEnemy>()).Count();

        if (zombiesSpawned > 0 && skeletsSpawned > 0)
        {
            enemyRatio = zombiesSpawned / skeletsSpawned;
        }
        else if (zombiesSpawned > 0)
        {
            enemyRatio = zombiesSpawned;
        }
        else if(skeletsSpawned > 0)
        {
            enemyRatio = skeletsSpawned;
        }
        else
        {
            enemyRatio = 0;
        }
        


        if ((enemySpawnChance == 0 || enemyRatio >= 5)) //&& WaveNumber >= 4
        {
            enemyToSpawn = SkeletPrefab;

        }
        else
        {
            enemyToSpawn = ZombiePrefab;
        }

        StartCoroutine(currentSpawner.SpawnEnemiesInSpawner(enemyToSpawn, 2f));

        yield return new WaitForSeconds(0.5f);
        readyToSpawn = true;

    }

    public void EnemyKilled(EnemyClass enemy)
    {
        AllEnemiesKilled++;
        EnemiesKilledDuringRound++;

        if (enemy is ZombieEnemy zombie)
        {
            ZombiesKilled++;
        }
        else if (enemy is SkeletEnemy skelet)
        {
            SkeletsKilled++;
        }
        enemiesSpawned.Remove(enemy);
    }
    #endregion

    #region ----- Wave Handler ------
    public void WaveController()
    {
        if (EnemiesKilledDuringRound == MaxEnemies)
        {
            EnemiesKilledDuringRound = 0;
            enemiesSpawnedDuringRound = 0;
            WaveNumber++;
            ChangeValuesWithRound();
        }
    }

    public void ChangeValuesWithRound()
    {
        var zombie = ZombiePrefab.GetComponent<EnemyClass>();
        var skelet = SkeletPrefab.GetComponent<EnemyClass>();

        zombie.healthModif += 0.05f;
        zombie.damageModif += 0.05f;
        zombie.moveSpeedModif += 0.05f;

        skelet.healthModif += 0.1f;
        skelet.damageModif += 0.1f;
        skelet.moveSpeedModif += 0.05f;

        MaxEnemies += AddMaxEnemiesPerWave;
    }
    #endregion

    private void GivePlayerStartScore(int startScore)
    {
        var PCC = pcController;
        if (PCC == null)
        {
            Debug.Log("[GCC] - Player Controller is null");

        }
        pcController.GetComponent<PlayerInventory>().AddScore(startScore);
    }

    public void UpdatePlayerData()
    {
        PD = PlayerData.Instance;
        if (PD == null)
        {
            PD = new PlayerData();
        }

        new ScoreEntry()
        {
            playerNickname = PD.playerNickname,
            totalEnemiesKilled = PD.TotalEnemiesKilled
        };
        PD.TotalScore = TotalScore;
        PD.TotalEnemiesKilled = AllEnemiesKilled;
        PD.Date = System.DateTime.UtcNow.ToString("dd-MM-yyyy");
    }

    public void ExitToScoreboard()
    {
        GS.exitToScoreboard = true;
        StartCoroutine(GS.TurnOnLoadingScreen(LoadMainMenu));
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
