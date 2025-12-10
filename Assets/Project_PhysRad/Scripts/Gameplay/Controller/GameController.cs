using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameController");
                    _instance = singletonObject.AddComponent<GameController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    [Header("Core Systems")]
    [SerializeField] private PlayerStationControl stationControl;
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WaveManager waveManager;

    [Header("Optional Systems")]
    [SerializeField] private List<MonoBehaviour> systemsToInitialize;

    public PlayerStationControl Station => stationControl;
    public BuildManager BuildManager => buildManager;
    public UIManager UI => uiManager;

    private bool isGameActive = true;
    private bool isPaused = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            //return;
        }

        _instance = this;
        //DontDestroyOnLoad(gameObject);

        InitializeAllSystems();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            TogglePause();
        }
    }

    void InitializeAllSystems()
    {
        stationControl.Initialize(uiManager);

        if (buildManager != null)
            buildManager.Initialize(stationControl);

        if (uiManager != null)
            uiManager.Initialize(stationControl);

        if (waveManager != null)
            waveManager.Initialize(stationControl);

        foreach (var system in systemsToInitialize)
        {
            if (system is IGameSystem gameSystem)
                gameSystem.Initialize(stationControl);
        }

        Debug.Log("All game systems initialized");
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!isGameActive || isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (uiManager != null)
            uiManager.PauseGame();

        Debug.Log("Game Paused");
    }

    public void ContinueGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (uiManager != null)
            uiManager.ContinueGame();

        Debug.Log("Game Continued");
    }

    public PlayerStationControl GetStation() => stationControl;

    /// <summary>Быстрый доступ к скрапу станции</summary>
    public int GetCurrentScrap()
    {
        return stationControl != null ? stationControl.CurrentScrap : 0;
    }

    /// <summary>Попытаться потратить скрап</summary>
    public bool TrySpendScrap(int amount)
    {
        return stationControl != null && stationControl.TrySpendScrap(amount);
    }

    /// <summary>Добавить скрап</summary>
    public void AddScrap(int amount)
    {
        stationControl?.AddScrap(amount);
    }

    /// <summary>Завершение игры</summary>
    public void GameOver(bool isWin = false)
    {
        isGameActive = false;
        isPaused = false;

        Debug.Log(isWin ? "VICTORY!" : "GAME OVER");

        uiManager?.ShowGameOverScreen(isWin);
    }

    /// <summary>Перезапуск уровня</summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        isPaused = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>Выход в меню</summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        isPaused = false;

        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

public interface IGameSystem
{
    void Initialize(PlayerStationControl station);
}