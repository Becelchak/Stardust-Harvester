using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))] // Обязательно!
[RequireComponent(typeof(MeshRenderer))] // Для отображения
public class BuildCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Визуальные настройки")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material blockedMaterial;
    [SerializeField] private Material occupiedMaterial;

    private BuildGridGenerator gridGenerator;
    private Vector2Int gridCoordinate;
    private bool isBuildable = true;
    private bool isOccupied = false;
    private IBuildable currentBuildable;
    private MeshRenderer meshRenderer;

    public Vector2Int GridCoordinate => gridCoordinate;
    public Vector3 WorldPosition => transform.position;
    public bool IsBuildable => isBuildable && !isOccupied;
    public bool IsOccupied => isOccupied;
    public IBuildable CurrentBuildable => currentBuildable;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError($"BuildCell {name}: нет MeshRenderer!");
        }
    }

    public void Initialize(BuildGridGenerator generator, Vector2Int coord, bool buildable)
    {
        gridGenerator = generator;
        gridCoordinate = coord;
        isBuildable = buildable;

        SetMaterial(buildable ? defaultMaterial : blockedMaterial);

        SetupCollider();
    }

    private void SetupCollider()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null) collider = gameObject.AddComponent<BoxCollider>();

        collider.isTrigger = false;
        collider.size = new Vector3(1, 0.1f, 1);

        collider.enabled = isBuildable;
    }

    public void SetMaterial(Material material)
    {
        if (meshRenderer != null && material != null)
            meshRenderer.material = material;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isBuildable || isOccupied) return;

        SetMaterial(hoverMaterial);
        BuildManager.Instance?.OnCellHoverEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isBuildable || isOccupied) return;

        SetMaterial(defaultMaterial);
        BuildManager.Instance?.OnCellHoverExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isBuildable || isOccupied) return;

        BuildManager.Instance?.OnCellClicked(this);
        Debug.Log($"Клик по клетке {gridCoordinate}");
    }

    public void SetOccupied(IBuildable buildable)
    {
        isOccupied = true;
        currentBuildable = buildable;
        SetMaterial(occupiedMaterial);

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null) collider.enabled = false;
    }

    public void ClearOccupation()
    {
        isOccupied = false;
        currentBuildable = null;
        SetMaterial(defaultMaterial);

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null) collider.enabled = true;
    }

    public void SetHovered(bool isHovered)
    {
        if (!isBuildable || isOccupied) return;
        SetMaterial(isHovered ? hoverMaterial : defaultMaterial);
    }
}