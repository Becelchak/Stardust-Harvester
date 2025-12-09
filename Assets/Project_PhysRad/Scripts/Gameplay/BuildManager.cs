using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour, IGameSystem
{
    [Header("Settings")]
    [SerializeField] private GameObject buildPreviewPrefab;
    [SerializeField] private LayerMask buildGridLayer;

    private PlayerStationControl stationControl;
    private BuildGridGenerator currentGrid;
    private IBuildable selectedBuildable;
    private GameObject currentPreview;
    private List<IBuildable> activeBuildings = new List<IBuildable>();

    public void Initialize(PlayerStationControl station)
    {
        if (station == null)
        {
            Debug.LogError("BuildManager: No station provided!");
            return;
        }

        stationControl = station;
        Debug.Log("BuildManager initialized with station reference");
    }

    void Update()
    {
        UpdateBuildPreview();

        if (Input.GetKeyDown(KeyCode.Escape))
            ClearSelection();
    }

    public void SelectBuildable(IBuildable buildable)
    {
        selectedBuildable = buildable;

        if (buildPreviewPrefab != null && selectedBuildable != null)
        {
            if (currentPreview != null)
                Destroy(currentPreview);

            currentPreview = Instantiate(buildPreviewPrefab);
            currentPreview.SetActive(false);
        }
    }

    void UpdateBuildPreview()
    {
        if (selectedBuildable == null || currentPreview == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, buildGridLayer))
        {
            BuildCell cell = hit.collider.GetComponent<BuildCell>();
            if (cell != null && cell.IsBuildable && !cell.IsOccupied)
            {
                currentPreview.transform.position = cell.WorldPosition + Vector3.up * 0.5f;
                currentPreview.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                    TryBuildAtCell(cell);
            }
            else
            {
                currentPreview.SetActive(false);
            }
        }
    }

    public void RegisterBuilding(IBuildable buildable)
    {
        if (!activeBuildings.Contains(buildable))
        {
            activeBuildings.Add(buildable);

            buildable.OnBuildDestroyed += OnSpecificWallDestroyed;
        }
    }

    public void UnregisteredBuilding(IBuildable buildable)
    {
        if (activeBuildings.Contains(buildable))
        {
            activeBuildings.Remove(buildable);

            buildable.OnBuildDestroyed -= OnSpecificWallDestroyed;
        }
    }

    void OnSpecificWallDestroyed(IBuildable buildable)
    {
        UnregisteredBuilding(buildable);
    }


    void TryBuildAtCell(BuildCell cell)
    {
        if (selectedBuildable == null || stationControl == null) return;

        if (!stationControl.HasEnoughScrap(selectedBuildable.BuildCost))
        {
            Debug.Log("Not enough scrap!");
            return;
        }

        if (!stationControl.TrySpendScrap(selectedBuildable.BuildCost))
            return;

        if (currentGrid != null)
        {
            IBuildable builtObject;
            if (currentGrid.TryBuildAtCell(cell, selectedBuildable, out builtObject))
            {
                Debug.Log($"Built {((MonoBehaviour)builtObject).name}");
                ClearSelection();
            }
        }
    }

    public IBuildable GetNearestBuilding(Vector3 position, float maxRange = float.MaxValue)
    {
        IBuildable nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var building in activeBuildings)
        {
            if (building == null) continue;

            float distance = Vector3.Distance(position, ((MonoBehaviour)building).transform.position);
            if (distance <= maxRange && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = building;
            }
        }

        return nearest;
    }

    void ClearSelection()
    {
        selectedBuildable = null;
        if (currentPreview != null)
            Destroy(currentPreview);
    }

    public void RegisterGrid(BuildGridGenerator grid) => currentGrid = grid;
}