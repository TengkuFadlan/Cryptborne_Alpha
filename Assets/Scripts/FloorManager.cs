using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class FloorManager : MonoBehaviour
{
  public float bonusHealthPerFloor = 10f;
  [Header("UI References")]
  public TextMeshProUGUI floorText;
  public TextMeshProUGUI timerText;
  public TextMeshProUGUI floorClearTimeText;
  public TextMeshProUGUI floorClearedText;

  public UnityEngine.UI.Image tipSprite;
  public TextMeshProUGUI tipText;
  public GameObject tipFrame;

  public Animator floorClearedAnimator;
  public GameObject cryptClearedFrame;
  public GameObject gameOverFrame;
  public GameObject pauseFrame;

  public LevelSO LevelData;

  List<Entity> activeEnemyEntities = new();
  GameObject playerGameObject;
  GameObject currentMap;
  int currentFloor = 0;
  int currentWave = 0;

  float timer = 0f;
  bool isTimerRunning = false;
  float floorTimer = 0f;

  // New variable for the Input System actions.
  private PlayerInputActions playerInputActions;

  void Awake()
  {
    // Initialize the input actions asset.
    playerInputActions = new PlayerInputActions();

    // Subscribe to the "EscapeMenu" performed event.
    playerInputActions.Keyboard.EscapeMenu.performed += OnEscapeMenuPerformed;
  }

  void OnEnable()
  {
    // Enable the input actions when the script is enabled.
    playerInputActions.Enable();
  }

  void OnDisable()
  {
    // Disable the input actions when the script is disabled.
    playerInputActions.Disable();
  }

  // The callback method for the EscapeMenu action.
  private void OnEscapeMenuPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
  {
    if (pauseFrame.activeSelf)
    {
      ContinueGame();
    }
    else
    {
      PauseGame();
    }
  }

  void Update()
  {
    if (isTimerRunning)
    {
      floorTimer += Time.deltaTime;
      timer += Time.deltaTime;
      UpdateTimerText();
    }
  }

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
    isTimerRunning = false;

    floorClearedText.text = "Floor " + (currentFloor + 1) + " Cleared";
    TimeSpan timeSpan = TimeSpan.FromSeconds(floorTimer);
    floorClearTimeText.text = "+" + string.Format("{0}:{1:00}:{2:00}.{3:00}",
        timeSpan.Hours,
        timeSpan.Minutes,
        timeSpan.Seconds,
        timeSpan.Milliseconds / 10);
    floorClearedAnimator.SetBool("Open", true);

    if (playerGameObject.TryGetComponent<Entity>(out var playerEntity))
    {
      playerEntity.OnRecieveHeal?.Invoke(bonusHealthPerFloor);
    }

    currentFloor++;

    if (currentFloor < LevelData.floors.Count)
    {
      Debug.Log("End Floor, Starting next floor");
      Invoke("StartFloor", 4f);
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
    isTimerRunning = false;

    if (playerGameObject != null && playerGameObject.TryGetComponent<Entity>(out var playerEntity))
    {
      playerEntity.OnDeath -= FailedGame;
    }

    cryptClearedFrame.SetActive(true);
  }

  void FailedGame()
  {
    Debug.Log("Game over!");
    CancelInvoke();
    isTimerRunning = false;

    if (playerGameObject != null && playerGameObject.TryGetComponent<Entity>(out var playerEntity))
    {
      playerEntity.OnDeath -= FailedGame;
    }

    gameOverFrame.SetActive(true);
  }

  void StartWave()
  {
    Debug.Log("Starting Wave " + currentWave);

    WaveSO waveData = LevelData.floors[currentFloor].waves[currentWave];
    foreach (var enemySpawn in waveData.enemiesToSpawn)
    {
      Transform mapEnemySpawn = currentMap.transform.Find("EnemySpawn").transform;
      GameObject newEnemy = Instantiate(enemySpawn.enemyPrefab, mapEnemySpawn.position + (Vector3)enemySpawn.spawnPosition, Quaternion.identity);

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

    isTimerRunning = true;

    floorText.text = "Floor " + (currentFloor + 1);

    Debug.Log("Starting Floor " + currentFloor);
    currentWave = 0;

    if (LevelData.floors[currentFloor].floorTip != "")
      TipPauseGame(LevelData.floors[currentFloor]);

    Invoke("StartWave", 1f);
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
    timer = 0f;

    StartFloor();
  }

  void Start()
  {
    StartGame();
  }

  // Public UI Functions
  public void PauseGame()
  {
    Time.timeScale = 0f;
    isTimerRunning = false;
    pauseFrame.SetActive(true);
  }

  public void ContinueGame()
  {
    Time.timeScale = 1f;
    isTimerRunning = true;
    pauseFrame.SetActive(false);
    tipFrame.SetActive(false);
  }

  public void TipPauseGame(FloorSO floorInfo)
  {
    Time.timeScale = 0f;
    isTimerRunning = false;
    tipText.text = floorInfo.floorTip;
    tipSprite.sprite = floorInfo.floorImageTip;
    tipFrame.SetActive(true);
  }

  public void RestartGame()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void LeaveGame()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene("MainMenu");
  }
}