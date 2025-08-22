using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure to add this namespace!
using System;

public class FloorManager : MonoBehaviour
{
  // These are the new variables for the UI elements.
  [Header("UI References")]
  public TextMeshProUGUI floorText;
  public TextMeshProUGUI timerText;
  public TextMeshProUGUI floorClearTimeText;
  public TextMeshProUGUI floorClearedText;
  public Animator floorClearedAnimator;

  public LevelSO LevelData;

  List<Entity> activeEnemyEntities = new();
  GameObject playerGameObject;
  GameObject currentMap;
  int currentFloor = 0;
  int currentWave = 0;

  // These are the new variables for the timer.
  float timer = 0f;
  bool isTimerRunning = false;
  float floorTimer = 0f;

  // This method is new and will be called every frame to update the timer.
  void Update()
  {
    if (isTimerRunning)
    {
      floorTimer += Time.deltaTime;
      timer += Time.deltaTime;
      UpdateTimerText();
    }
  }

  // This new method updates the timer display.
  void UpdateTimerText()
  {
    TimeSpan timeSpan = TimeSpan.FromSeconds(timer);
    timerText.text = string.Format("{0}:{1:00}:{2:00}.{3:00}",
        timeSpan.Hours,
        timeSpan.Minutes,
        timeSpan.Seconds,
        timeSpan.Milliseconds / 10);
  }

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
      Invoke("StartWave", 2f);
    }
    else
    {
      Debug.Log("End Wave, reached final wave. Starting ending floor");
      EndFloor();
    }
  }

  void EndFloor()
  {
    // Pause the timer when the floor ends.
    isTimerRunning = false;

    floorClearedText.text = "Floor " + (currentFloor + 1) + " Cleared";
    TimeSpan timeSpan = TimeSpan.FromSeconds(floorTimer);
    floorClearTimeText.text = "+" + string.Format("{0}:{1:00}:{2:00}.{3:00}",
        timeSpan.Hours,
        timeSpan.Minutes,
        timeSpan.Seconds,
        timeSpan.Milliseconds / 10);
    floorClearedAnimator.SetBool("Open", true);

    currentFloor++;

    if (currentFloor < LevelData.floors.Count)
    {
      Debug.Log("End Floor, Starting next floor");
      Invoke("StartFloor", 5f);
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
    // Pause the timer when the game ends.
    isTimerRunning = false;

    if (playerGameObject != null && playerGameObject.TryGetComponent<Entity>(out var playerEntity))
    {
      playerEntity.OnDeath -= FailedGame;
    }
  }

  void FailedGame()
  {
    Debug.Log("Game over!");
    CancelInvoke();
    // Pause the timer when the game fails.
    isTimerRunning = false;

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

    floorTimer = 0f;
    floorClearedAnimator.SetBool("Open", false);

    // Start the timer when the floor begins.
    isTimerRunning = true;

    floorText.text = "Floor " + (currentFloor + 1);

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
    timer = 0f; // Reset the timer at the start of the game.

    StartFloor();
  }

  void Start()
  {
    StartGame();
  }
}