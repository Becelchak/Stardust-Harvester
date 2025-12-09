using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Настройки строительства")]
    [SerializeField] private GameObject buildPreviewPrefab;
    [SerializeField] private LayerMask buildGridLayer;

    private IBuildable selectedBuildablePrefab;
    private GameObject currentPreview;
    private BuildCell hoveredCell;
    private BuildGridGenerator currentGrid;

    private List<IBuildable> activeBuildings = new List<IBuildable>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        UpdateBuildPreview();

        if (Input.GetKeyDown(KeyCode.Escape))
            ClearSelection();

        if (Input.GetMouseButtonDown(1))
            ClearSelection();
    }

    public void RegisterBuilding(IBuildable buildable)
    {
        if (!activeBuildings.Contains(buildable))
        {
            activeBuildings.Add(buildable);

            buildable.OnBuildDestroyed += OnSpecificWallDestroyed;
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

    public void UnregisterBuilding(IBuildable buildable)
    {
        if (activeBuildings.Contains(buildable))
        {
            activeBuildings.Remove(buildable);

            buildable.OnBuildDestroyed -= OnSpecificWallDestroyed;
        }
    }

    void OnSpecificWallDestroyed(IBuildable buildable)
    {
        UnregisterBuilding(buildable);
    }

    public void SelectBuildable(IBuildable buildablePrefab)
    {
        selectedBuildablePrefab = buildablePrefab;

        if (buildPreviewPrefab != null && selectedBuildablePrefab != null)
        {
            if (currentPreview != null)
                Destroy(currentPreview);

            currentPreview = Instantiate(buildPreviewPrefab);
            currentPreview.SetActive(false);

            SetPreviewTransparency(currentPreview, 0.5f);
        }

        Debug.Log($"Выбрано для постройки: {((MonoBehaviour)selectedBuildablePrefab).name}");
    }

    void UpdateBuildPreview()
    {
        if (selectedBuildablePrefab == null || currentPreview == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, buildGridLayer))
        {
            BuildCell cell = hit.collider.GetComponent<BuildCell>();
            if (cell != null && cell != hoveredCell)
            {
                OnCellHoverEnter(cell);
            }

            if (cell != null)
            {
                currentPreview.transform.position = cell.WorldPosition + Vector3.up * 0.5f;
                currentPreview.SetActive(true);

                UpdatePreviewColor(cell.IsBuildable && !cell.IsOccupied);
            }
            else
            {
                currentPreview.SetActive(false);
            }
        }
        else
        {
            currentPreview.SetActive(false);
            if (hoveredCell != null)
                OnCellHoverExit(hoveredCell);
        }


        if (Input.GetMouseButtonDown(0) && hoveredCell != null)
        {
            TryBuildAtCell(hoveredCell);
        }
    }

    public void OnCellHoverEnter(BuildCell cell)
    {
        hoveredCell = cell;

    }

    public void OnCellHoverExit(BuildCell cell)
    {
        if (hoveredCell == cell)
            hoveredCell = null;
    }

    public void OnCellClicked(BuildCell cell)
    {
        if (selectedBuildablePrefab != null)
        {
            TryBuildAtCell(cell);
        }
        else
        {
            Debug.Log("Не выбран объект для строительства");
        }
    }

    void TryBuildAtCell(BuildCell cell)
    {
        if (selectedBuildablePrefab == null || !cell.IsBuildable || cell.IsOccupied)
            return;

        if (!CanAffordBuild(selectedBuildablePrefab.BuildCost))
        {
            Debug.Log("Недостаточно ресурсов для постройки");
            return;
        }

        if (currentGrid != null)
        {
            IBuildable builtObject;
            if (currentGrid.TryBuildAtCell(cell, selectedBuildablePrefab, out builtObject))
            {
                SpendResources(selectedBuildablePrefab.BuildCost);

                Debug.Log($"Построено: {((MonoBehaviour)builtObject).name} на клетке {cell.GridCoordinate}");
                builtObject.OnBuild(cell);
                Destroy(currentPreview);

                ClearSelection();
            }
        }
    }

    void ClearSelection()
    {
        selectedBuildablePrefab = null;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        hoveredCell = null;
    }

    void SetPreviewTransparency(GameObject preview, float alpha)
    {
        Renderer[] renderers = preview.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;

            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }

    void UpdatePreviewColor(bool canBuild)
    {
        if (currentPreview == null) return;

        Color previewColor = canBuild ? Color.green : Color.red;
        previewColor.a = 0.5f;

        Renderer[] renderers = currentPreview.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = previewColor;
        }
    }

    bool CanAffordBuild(int cost)
    {
        return true; // Временно всегда true
    }

    void SpendResources(int amount)
    {
        // Реализуйте списание ресурсов
        Debug.Log($"Списано {amount} ресурсов");
    }

    public void RegisterGrid(BuildGridGenerator grid)
    {
        currentGrid = grid;
    }
}