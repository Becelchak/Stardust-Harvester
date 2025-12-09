using UnityEngine;
using System.Collections.Generic;

public class BuildGridGenerator : MonoBehaviour
{
    [Header("Настройки сетки")]
    [SerializeField] private Transform gridCenter;
    [SerializeField] private int gridSize = 10;
    [SerializeField] private float cellSize = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject cellPrefab;

    [Header("Визуальные настройки")]
    [SerializeField] private Material freeCellMaterial;
    [SerializeField] private Material occupiedCellMaterial;
    [SerializeField] private Material blockedCellMaterial;

    private Dictionary<Vector2Int, BuildCell> gridCells = new Dictionary<Vector2Int, BuildCell>();
    private Dictionary<IBuildable, BuildCell> buildables = new Dictionary<IBuildable, BuildCell>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        if (cellPrefab == null || gridCenter == null)
        {
            Debug.LogError("BuildGridGenerator: Не назначены префаб клетки или центр сетки!");
            return;
        }

        int halfSize = gridSize / 2;

        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 worldPosition = gridCenter.position +
                    new Vector3(x * cellSize, 1f, z * cellSize);

                bool isBlocked = Physics.CheckSphere(
                    worldPosition,
                    cellSize * 0.4f,
                    obstacleLayer
                );

                Vector2Int gridCoord = new Vector2Int(x + halfSize, z + halfSize);

                GameObject cellObj = Instantiate(
                    cellPrefab,
                    worldPosition,
                    Quaternion.identity,
                    transform
                );

                cellObj.name = $"Cell_{gridCoord.x}_{gridCoord.y}";

                BuildCell cell = cellObj.GetComponent<BuildCell>();
                if (cell == null) cell = cellObj.AddComponent<BuildCell>();

                cell.Initialize(this, gridCoord, worldPosition, !isBlocked);

                cell.SetMaterial(isBlocked ? blockedCellMaterial : freeCellMaterial);

                gridCells[gridCoord] = cell;
            }
        }

        Debug.Log($"Сетка построена: {gridCells.Count} клеток");
    }

    public bool TryBuildAtCell(BuildCell cell, IBuildable buildablePrefab, out IBuildable builtObject)
    {
        builtObject = null;

        if (cell == null || !cell.IsBuildable || cell.IsOccupied)
            return false;

        GameObject builtObj = Instantiate(
            (buildablePrefab as MonoBehaviour).gameObject,
            cell.WorldPosition + Vector3.up * 0.5f,
            Quaternion.identity
        );

        UpdateGridAroundPosition(cell.transform.position, 1);

        IBuildable buildable = builtObj.GetComponent<IBuildable>();
        if (buildable == null)
        {
            Destroy(builtObj);
            return false;
        }

        cell.SetOccupied(buildable, occupiedCellMaterial);
        buildables[buildable] = cell;

        buildable.OnBuild(cell);

        builtObject = buildable;
        return true;
    }

    public bool TryRemoveBuildable(IBuildable buildable)
    {
        if (!buildables.ContainsKey(buildable))
            return false;

        BuildCell cell = buildables[buildable];
        cell.ClearOccupation();
        buildables.Remove(buildable);

        return true;
    }

    public BuildCell GetCellAtPosition(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - gridCenter.position;
        int x = Mathf.RoundToInt(localPos.x / cellSize) + gridSize / 2;
        int z = Mathf.RoundToInt(localPos.z / cellSize) + gridSize / 2;

        Vector2Int coord = new Vector2Int(x, z);
        return gridCells.ContainsKey(coord) ? gridCells[coord] : null;
    }

    void OnDrawGizmosSelected()
    {
        if (gridCenter == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        float halfCell = cellSize * 0.5f;
        int halfSize = gridSize / 2;

        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 center = gridCenter.position +
                    new Vector3(x * cellSize, 0.1f, z * cellSize);

                Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }

    public void UpdateGridAroundPosition(Vector3 position, float radius)
    {
        int halfSize = gridSize / 2;

        foreach (var cell in gridCells.Values)
        {
            float distance = Vector3.Distance(cell.WorldPosition, position);
            if (distance <= radius)
            {
                bool isBlocked = Physics.CheckSphere(
                    cell.WorldPosition,
                    cellSize * 0.4f,
                    obstacleLayer
                );

                cell.SetMaterial(isBlocked ? blockedCellMaterial :
                    cell.IsOccupied ? occupiedCellMaterial : freeCellMaterial);
            }
        }
    }
}