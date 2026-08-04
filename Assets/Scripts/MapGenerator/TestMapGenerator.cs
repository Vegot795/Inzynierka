using System.Collections.Generic;
using System.ComponentModel;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;



public class TestMapGenerator : MapGenerator
{
    [Header("MapSettings")]
    public bool spawnTestingEnemy;
    public float spawnDistance = 5;
    public int spawnEnemyCount = 1;


    public int height = 10;
    public int width = 10;
    public Vector2 StartPoint;
    public GameObject playerPrefab;
    public GameObject HUD;
    public CinemachineCamera ccCamera;
    public GameObject rifflePrefab;
    public GameObject shotgunPrefab;
    public WeaponType weaponType;
    public GameObject PC;
    public GameObject enemyPrefab;

    public enum WeaponType
    {
        Riffle,
        Shotgun
    }

    private readonly List<GridCell> emptyCells = new List<GridCell>();

    public override void Start()
    {
        GenerateGrid(width, height);
        SpawnPlayer();
        if (spawnTestingEnemy)
        {
            SpawnTestingEnemy(PC.transform.position, spawnDistance, spawnEnemyCount);
        }
    }

    public void GiveStartingGear(WeaponType weapon)
    {
        ccCamera.Target.TrackingTarget = PC.transform;
        WeaponPickup startingWeapon = null;

        switch (weapon)
        {
            case WeaponType.Riffle:
                startingWeapon = rifflePrefab.GetComponent<WeaponPickup>();
                break;
            case WeaponType.Shotgun:
                startingWeapon = shotgunPrefab.GetComponent<WeaponPickup>();
                break;
            default:
                Debug.LogWarning("Unknown weapon type: " + weapon);
                break;
        }

    Instantiate(startingWeapon, PC.transform.position, Quaternion.identity);

    }

    public void SpawnPlayer()
    {
        int startX;
        int startY;

        if (StartPoint == null)
        {
            startX = Random.Range(0, width);
            startY = Random.Range(0, height);
        }
        else
        {
            startX = Mathf.Clamp((int)StartPoint.x, 0, width - 1);
            startY = Mathf.Clamp((int)StartPoint.y, 0, height - 1);
        }

        PC = Instantiate(playerPrefab, new Vector3(startX, startY, 0), Quaternion.identity);

        ccCamera.Target.TrackingTarget = PC.transform;
        Instantiate(HUD, Vector3.zero, Quaternion.identity);
        

    }

    public override void GenerateGrid(int width, int height)
    {
        var mapWidth = width;
        var mapHeight = height;
        grid = new GridCell[width, height];
        emptyCells.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newCellObject = Instantiate(gridCellPref, new Vector3(x, y, 0), Quaternion.identity);
                newCellObject.transform.SetParent(transform);

                GridCell newCell = newCellObject.GetComponent<GridCell>();

                if (newCell == null)
                {
                    Debug.LogError("[MapGenerator] gridCellPref does not contain GridCell component.");
                    continue;
                }

                newCell.x = x;
                newCell.y = y;
                newCell.type = GridCell.CellType.floor;
                newCell.environmentData = roomEnvironment[0];

                if (newCell.sr == null)
                {
                    newCell.sr = newCellObject.GetComponent<SpriteRenderer>();
                }

                if (newCell.sr != null)
                {
                    newCell.sr.sprite = roomEnvironment[0].Floor;
                }

                grid[x, y] = newCell;
                emptyCells.Add(newCell);
            }
        }         

        Debug.Log($"Grid generated with dimensions: {width}x{height}. Cell count: {emptyCells.Count}");
    }

    public void SpawnTestingEnemy(Vector3 targetPosition, float targetDistance, int targetCount)
    {

        if (enemyPrefab == null)
        {
            Debug.LogError("[TestMapGenerator] enemyPrefab is not assigned in the Inspector.");
            return;
        }

        Pathfinding sharedPathfinder = GetComponent<Pathfinding>();
        if (sharedPathfinder == null)
        {
            Debug.LogError("[TestMapGenerator] No Pathfinding component on the generator object.", this);
            return;
        }

        for (int i = 0; i < targetCount; i++)
        {
            Vector3 spawnPosition = targetPosition + new Vector3(Random.Range(-targetDistance, targetDistance), Random.Range(-targetDistance, targetDistance), 0);
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            ZombieEnemy zombie = newEnemy.GetComponent<ZombieEnemy>();
            if (zombie != null)
            {
                zombie.pathfinder = sharedPathfinder;
            }
        }
    }

}
