using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour, IBuildable
{
    [Header("Настройки башни")]
    [SerializeField] private int buildCost = 100;
    [SerializeField] private GameObject buildEffectPrefab;

    private BuildCell buildCell;

    public int BuildCost => buildCost;
    public bool CanBuild => true;
    public BuildCell BuildCell
    {
        get => buildCell;
        set => buildCell = value;
    }

    public void OnBuild(BuildCell cell)
    {
        buildCell = cell;

        if (buildEffectPrefab != null)
            Instantiate(buildEffectPrefab, transform.position, Quaternion.identity);

        Debug.Log($"Башня построена на клетке {cell.GridCoordinate}");

        UpdateNavGraph();
    }

    public void OnDestroyed()
    {
        if (buildCell != null)
        {
            BuildGridGenerator generator = buildCell.GetComponentInParent<BuildGridGenerator>();
            if (generator != null)
                generator.TryRemoveBuildable(this);
        }

        Destroy(gameObject);
    }

    void UpdateNavGraph()
    {
        var graphUpdate = new Pathfinding.GraphUpdateObject(GetComponent<Collider>().bounds);
        graphUpdate.modifyWalkability = true;
        graphUpdate.setWalkability = false;
    }

    void OnDestroy()
    {
        if (buildCell != null)
            buildCell.ClearOccupation();
    }
}
