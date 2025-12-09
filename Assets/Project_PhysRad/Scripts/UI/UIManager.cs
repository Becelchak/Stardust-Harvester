using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text scrapText;
    [SerializeField] private Image healthSlider;
    [SerializeField] private Text healthText;
    [SerializeField] private GameObject lowHealthWarning;

    private PlayerStationControl stationControl;

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

        RefreshUI();

        Debug.Log("UIManager initialized");
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

        if (healthText != null)
            healthText.text = $"{stationControl.CurrentHealth}/{stationControl.MaxHealth}";

        if (lowHealthWarning != null)
        {
            float healthPercent = (float)stationControl.CurrentHealth / stationControl.MaxHealth;
            lowHealthWarning.SetActive(healthPercent < 0.3f);
        }
    }

    void HandleScrapChanged(int oldScrap, int newScrap)
    {
        RefreshUI();

        if (scrapText != null)
        {
            //scrapText.GetComponent<Animator>()?.SetTrigger("Pulse");
        }
    }

    void HandleHealthChanged(int newHealth)
    {
        RefreshUI();
    }

    void HandleStationDestroyed()
    {
        Debug.Log("Show Game Over UI");
    }

    public void ShowGameOverScreen(bool isWinStatus)
    {
        
    }

    void OnDestroy()
    {

        if (stationControl != null)
        {
            stationControl.OnScrapChanged -= HandleScrapChanged;
            stationControl.OnHealthChanged -= HandleHealthChanged;
            stationControl.OnStationDestroyed -= HandleStationDestroyed;
        }
    }
}