using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text scrapText;
    [SerializeField] private Image healthSlider;
    [SerializeField] private CanvasGroup GUI;
    [SerializeField] private CanvasGroup DefeatePanel;
    [SerializeField] private CanvasGroup PausePanel;
    [SerializeField] private CanvasGroup WinPanel;
    [SerializeField] private CanvasGroup HelpPanel;

    [Header("Pause Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button closeHelpButton;

    private PlayerStationControl stationControl;
    private bool isPaused = false;

    public bool IsPaused => isPaused;
    private Coroutine returnToMenuCoroutine;

    public void Initialize(PlayerStationControl station)
    {
        if (station == null)
        {
            Debug.LogError("UIManager: No station provided!");
            return;
        }

        stationControl = station;

        stationControl.OnScrapChanged += HandleScrapChanged;
        stationControl.OnHealthChanged += HandleHealthChanged;
        stationControl.OnStationDestroyed += HandleStationDestroyed;

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);

        if (helpButton != null)
            helpButton.onClick.AddListener(ShowHelp);

        if (closeHelpButton != null)
            closeHelpButton.onClick.AddListener(HideHelp);

        HideAllPanels();

        RefreshUI();

        Debug.Log("UIManager initialized");
    }

    private void HideAllPanels()
    {
        SetCanvasGroup(PausePanel, false);
        SetCanvasGroup(HelpPanel, false);
        SetCanvasGroup(DefeatePanel, false);
        SetCanvasGroup(WinPanel, false);
        SetCanvasGroup(GUI, true);
    }

    private void SetCanvasGroup(CanvasGroup group, bool show)
    {
        if (group == null) return;

        group.alpha = show ? 1.0f : 0.0f;
        group.blocksRaycasts = show;
        group.interactable = show;
    }

    public void RefreshUI()
    {
        if (stationControl == null) return;

        if (scrapText != null)
            scrapText.text = $"SCRAP: {stationControl.CurrentScrap}";

        if (healthSlider != null)
        {
            healthSlider.fillAmount = Mathf.Clamp01((float)stationControl.CurrentHealth / stationControl.MaxHealth);
        }
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
        if (isPaused) return;

        isPaused = true;
        SetCanvasGroup(PausePanel, true);
        SetCanvasGroup(GUI, false);

        GameController.Instance.PauseGame();
    }

    public void ContinueGame()
    {
        if (!isPaused) return;

        isPaused = false;
        SetCanvasGroup(PausePanel, false);
        SetCanvasGroup(HelpPanel, false);
        SetCanvasGroup(GUI, true);

        GameController.Instance.ContinueGame();
    }

    public void ShowHelp()
    {
        SetCanvasGroup(HelpPanel, true);
        SetCanvasGroup(PausePanel, false);
    }

    public void HideHelp()
    {
        SetCanvasGroup(HelpPanel, false);
        SetCanvasGroup(PausePanel, true);
    }

    void HandleScrapChanged(int oldScrap, int newScrap)
    {
        RefreshUI();
    }

    void HandleHealthChanged(int newHealth)
    {
        RefreshUI();
    }

    void HandleStationDestroyed()
    {
        Debug.Log("Show Game Over UI");

        SetCanvasGroup(DefeatePanel, true);
        SetCanvasGroup(GUI, false);
        SetCanvasGroup(PausePanel, false);

        StartAutoReturnToMenu(false);
    }

    public void ShowGameOverScreen(bool isWinStatus)
    {
        SetCanvasGroup(isWinStatus ? WinPanel : DefeatePanel, true);
        SetCanvasGroup(GUI, false);
        SetCanvasGroup(PausePanel, false);

        Time.timeScale = isWinStatus ? 1 : 0;
        StartAutoReturnToMenu(false);

    }

    private void StartAutoReturnToMenu(bool isWin)
    {
        // Останавливаем предыдущую корутину, если она есть
        if (returnToMenuCoroutine != null)
        {
            StopCoroutine(returnToMenuCoroutine);
        }

        // Запускаем новую корутину
        returnToMenuCoroutine = StartCoroutine(AutoReturnToMenuCoroutine(isWin));
    }

    private IEnumerator AutoReturnToMenuCoroutine(bool isWin)
    {
        float timer = 2;

        yield return new WaitForSecondsRealtime(timer);

        GameController.Instance.ReturnToMenu();
    }

    public void RestartLevel()
    {
        GameController.Instance.RestartLevel();
    }

    public void ReturnToMenu()
    {
        GameController.Instance.ReturnToMenu();
    }

    void OnDestroy()
    {
        if (stationControl != null)
        {
            stationControl.OnScrapChanged -= HandleScrapChanged;
            stationControl.OnHealthChanged -= HandleHealthChanged;
            stationControl.OnStationDestroyed -= HandleStationDestroyed;
        }

        if (continueButton != null)
            continueButton.onClick.RemoveAllListeners();

        if (restartButton != null)
            restartButton.onClick.RemoveAllListeners();

        if (menuButton != null)
            menuButton.onClick.RemoveAllListeners();

        if (helpButton != null)
            helpButton.onClick.RemoveAllListeners();

        if (closeHelpButton != null)
            closeHelpButton.onClick.RemoveAllListeners();
    }
}