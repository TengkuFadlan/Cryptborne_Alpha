using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
  public LevelSO LevelData;

  List<Entity> activeEnemyEntities = new();
  GameObject playerGameObject;
  GameObject currentMap;
  int currentFloor = 0;
  int currentWave = 0;

  void OnEnemyDeath(Entity enemy)
  {
    enemy.OnDeath -= () => OnEnemyDeath(enemy);

    if (activeEnemyEntities.Contains(enemy))
    {
      activeEnemyEntities.Remove(enemy);
    }

    if (activeEnemyEntities.Count == 0)
    {
      Debug.Log("All enemies in wave defeated.");
      EndWave();
    }
  }

  void EndWave()
  {
    currentWave++;

    FloorSO floorData = LevelData.floors[currentFloor];
    if (currentWave < floorData.waves.Count)
    {
      Debug.Log("End Wave, Starting next wave");
      Invoke("StartWave", 3f);
    }
    else
    {
      Debug.Log("End Wave, reached final wave. Starting ending floor");
      EndFloor();
    }
  }

  void EndFloor()
  {
    currentFloor++;

    if (currentFloor < LevelData.floors.Count)
    {
      Debug.Log("End Floor, Starting next floor");
      Invoke("StartFloor", 3f);
    }
    else
    {
      Debug.Log("End Floor, reached final floor. Ending game");
      EndGame();
    }
  }

  void EndGame()
  {
    Debug.Log("End Game reached");
    if (playerGameObject != null && playerGameObject.TryGetComponent<Entity>(out var playerEntity))
    {
      playerEntity.OnDeath -= FailedGame;
    }
  }

  void FailedGame()
  {
    Debug.Log("Game over!");
    CancelInvoke();
    if (playerGameObject != null && playerGameObject.TryGetComponent<Entity>(out var playerEntity))
    {
      playerEntity.OnDeath -= FailedGame;
    }
  }

  void StartWave()
  {
    Debug.Log("Starting Wave " + currentWave);

    WaveSO waveData = LevelData.floors[currentFloor].waves[currentWave];
    foreach (var enemySpawn in waveData.enemiesToSpawn)
    {
      Transform mapEnemySpawn = currentMap.transform.Find("EnemySpawn").transform;
      GameObject newEnemy = Instantiate(enemySpawn.enemyPrefab, mapEnemySpawn.position + (Vector3)enemySpawn.spawnPosition, Quaternion.identity);

      // Listen for the enemy's death event
      if (newEnemy.TryGetComponent<Entity>(out var enemyEntity))
      {
        activeEnemyEntities.Add(enemyEntity);
        enemyEntity.OnDeath += () => OnEnemyDeath(enemyEntity);
      }
    }
  }

  void StartFloor()
  {
    if (currentMap != null)
      Destroy(currentMap);

    currentMap = Instantiate(LevelData.floors[currentFloor].map);
    Transform mapPlayerSpawn = currentMap.transform.Find("PlayerSpawn").transform;
    playerGameObject.transform.position = mapPlayerSpawn.transform.position;

    Debug.Log("Starting Floor " + currentFloor);
    currentWave = 0;
    StartWave();
  }

  void StartGame()
  {
    Debug.Log("Starting new game");
    playerGameObject = Instantiate(LevelData.playerCharacter);
    if (playerGameObject.TryGetComponent<Entity>(out var playerEntity))
    {
      playerEntity.OnDeath += FailedGame;
    }
    currentFloor = 0;
    StartFloor();
  }

  void Start()
  {
    StartGame();
  }
}