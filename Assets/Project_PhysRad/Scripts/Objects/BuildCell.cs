using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class BuildCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Визуальные настройки")]
    [SerializeField] private MeshRenderer cellRenderer;
    [SerializeField] private Material hoverMaterial;

    private BuildGridGenerator gridGenerator;
    private Vector2Int gridCoordinate;
    private Vector3 worldPosition;
    private bool isBuildable = true;
    private bool isOccupied = false;
    private IBuildable currentBuildable;
    private Material defaultMaterial;

    public Vector2Int GridCoordinate => gridCoordinate;
    public Vector3 WorldPosition => worldPosition;
    public bool IsBuildable => isBuildable;
    public bool IsOccupied => isOccupied;
    public IBuildable CurrentBuildable => currentBuildable;

    public void Initialize(BuildGridGenerator generator, Vector2Int coord, Vector3 position, bool buildable)
    {
        gridGenerator = generator;
        gridCoordinate = coord;
        worldPosition = position;
        isBuildable = buildable;

        if (cellRenderer != null)
            defaultMaterial = cellRenderer.material;

        if (!buildable)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
        }
    }

    public void SetMaterial(Material material)
    {
        if (cellRenderer != null)
        {
            cellRenderer.material = material;
            defaultMaterial = material;
        }
    }

    public void SetOccupied(IBuildable buildable, Material occupiedMaterial = null)
    {
        isOccupied = true;
        currentBuildable = buildable;

        if (occupiedMaterial != null && cellRenderer != null)
            cellRenderer.material = occupiedMaterial;
    }

    public void ClearOccupation()
    {
        isOccupied = false;
        currentBuildable = null;

        if (cellRenderer != null && defaultMaterial != null)
            cellRenderer.material = defaultMaterial;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isBuildable || isOccupied) return;

        if (cellRenderer != null && hoverMaterial != null)
            cellRenderer.material = hoverMaterial;

        BuildManager.Instance?.OnCellHoverEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isBuildable || isOccupied) return;

        if (cellRenderer != null && defaultMaterial != null)
            cellRenderer.material = defaultMaterial;

        BuildManager.Instance?.OnCellHoverExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isBuildable || isOccupied) return;

        BuildManager.Instance?.OnCellClicked(this);
    }
}