using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // ===== ПРАВИЛЬНАЯ РЕАЛИЗАЦИЯ СИНГЛТОНА =====
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

    void Awake()
    {
        // Защита от дублирования синглтона при загрузке новой сцены
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAllSystems();
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

    public PlayerStationControl GetStation() => stationControl;

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }
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
        Debug.Log(isWin ? "VICTORY!" : "GAME OVER");

        // Останавливаем все системы
        Time.timeScale = 0f;

        // Показываем соответствующее UI
        uiManager?.ShowGameOverScreen(isWin);

        // Можно добавить статистику, сохранение прогресса и т.д.
    }

    /// <summary>Перезапуск уровня</summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    /// <summary>Выход в меню</summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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