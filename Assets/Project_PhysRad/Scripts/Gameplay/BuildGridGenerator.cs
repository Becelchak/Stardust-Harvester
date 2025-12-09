using UnityEngine;
using System.Collections.Generic;

public class BuildGridGenerator : MonoBehaviour
{
    [Header("Настройки сетки")]
    [SerializeField] private Transform gridCenter;
    [SerializeField] private int gridSize = 20;
    [SerializeField] private float cellSize = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject cellPrefab;

    [Header("Материалы")]
    [SerializeField] private Material freeCellMaterial;
    [SerializeField] private Material blockedCellMaterial;
    [SerializeField] private Material occupiedCellMaterial;

    private Dictionary<Vector2Int, BuildCell> gridCells = new Dictionary<Vector2Int, BuildCell>();
    //private Dictionary<IBuildable, BuildCell> buildables = new Dictionary<IBuildable, BuildCell>();

    void Start()
    {
        GenerateGrid();
        GameController.Instance?.BuildManager.RegisterGrid(this);
    }

    void GenerateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("BuildGridGenerator: Не назначен префаб клетки!");
            return;
        }

        if (!cellPrefab.GetComponent<BuildCell>())
        {
            Debug.LogError($"BuildGridGenerator: Префаб {cellPrefab.name} не содержит компонент BuildCell!");
            return;
        }

        int halfSize = gridSize / 2;

        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 worldPosition = gridCenter.position +
                    new Vector3(x * cellSize, 0.001f, z * cellSize);

                bool isBlocked = Physics.CheckBox(
                    worldPosition,
                    new Vector3(cellSize * 0.45f, 0.05f, cellSize * 0.45f),
                    Quaternion.identity,
                    obstacleLayer
                );

                Vector2Int gridCoord = new Vector2Int(x + halfSize, z + halfSize);

                GameObject cellObj = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform);
                cellObj.name = $"Cell_{gridCoord.x}_{gridCoord.y}";

                BuildCell cell = cellObj.GetComponent<BuildCell>();

                Material startMaterial = isBlocked ? blockedCellMaterial : freeCellMaterial;
                cell.SetMaterial(startMaterial);

                cell.Initialize(this, gridCoord, !isBlocked);

                gridCells[gridCoord] = cell;

                if (isBlocked)
                    cellObj.GetComponent<Renderer>().material.color = Color.red * 0.3f;
                else
                    cellObj.GetComponent<Renderer>().material.color = Color.green * 0.3f;
            }
        }

        Debug.Log($"Сетка построена: {gridCells.Count} клеток");
    }

    //public bool TryRemoveBuildable(IBuildable buildable)
    //{
    //    if (!buildables.ContainsKey(buildable))
    //        return false;

    //    BuildCell cell = buildables[buildable];
    //    cell.ClearOccupation();
    //    buildables.Remove(buildable);

    //    return true;
    //}

    public bool TryBuildAtCell(BuildCell cell, IBuildable buildablePrefab, out IBuildable builtObject)
    {
        builtObject = null;

        if (cell == null || !cell.IsBuildable || cell.IsOccupied)
        {
            Debug.Log($"Клетка недоступна для строительства: {cell?.GridCoordinate}");
            return false;
        }

        MonoBehaviour buildableMono = buildablePrefab as MonoBehaviour;
        if (buildableMono == null)
        {
            Debug.LogError("BuildablePrefab не является MonoBehaviour!");
            return false;
        }

        GameObject builtObj = Instantiate(
            buildableMono.gameObject,
            cell.WorldPosition + Vector3.up * 0.5f,
            Quaternion.identity
        );

        IBuildable buildable = builtObj.GetComponent<IBuildable>();
        if (buildable == null)
        {
            Debug.LogError("Построенный объект не имеет IBuildable!");
            Destroy(builtObj);
            return false;
        }

        
        cell.SetOccupied(buildable);
        builtObject = buildable;

        builtObject.OnBuild(cell);
        //buildables.Add(builtObject, cell); 

        //Debug.Log($"Построено на клетке {cell.GridCoordinate}");
        return true;
    }
}