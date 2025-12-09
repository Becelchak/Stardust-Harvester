using System;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour, IBuildable, IDamageable
{
    [Header("Настройки стены")]
    [SerializeField] private int buildCost = 100;
    [SerializeField] private int maxHealth = 120;
    [SerializeField] private GameObject buildEffectPrefab;
    [SerializeField] private GameObject damageEffectPrefab;
    [SerializeField] private GameObject destroyEffectPrefab;

    [Header("Визуальная обратная связь")]
    [SerializeField] private Renderer wallRenderer;
    [SerializeField] private Material damagedMaterial;
    [SerializeField] private float damageFlashDuration = 0.2f;

    public event Action<IBuildable> OnBuildDamaged;
    public event Action<IBuildable> OnBuildDestroyed;
    public static event Action<IBuildable> OnAnyBuildDestroyed;

    private BuildCell buildCell;
    private Material originalMaterial;
    private int currentHealth;
    private bool isAlive = true;
    private Collider wallCollider;

    public int BuildCost => buildCost;
    public bool CanBuild => true;
    public BuildCell BuildCell
    {
        get => buildCell;
        set => buildCell = value;
    }

    public bool IsAlive => isAlive;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        wallCollider = GetComponent<Collider>();
        if (wallRenderer != null)
            originalMaterial = wallRenderer.material;
    }

    public void OnBuild(BuildCell cell)
    {
        buildCell = cell;

        currentHealth = maxHealth;
        isAlive = true;

        //if (buildEffectPrefab != null)
        //    Instantiate(buildEffectPrefab, transform.position, Quaternion.identity);

        Debug.Log($"Стена построена на клетке {cell.GridCoordinate}, HP: {currentHealth}");

        UpdateNavGraph();

        GameController.Instance?.BuildManager.RegisterBuilding(this);
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

    public void TakeDamage(int damage)
    {
        if(!isAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        StartCoroutine(DamageFlash());

        if (damageEffectPrefab != null)
            Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);

        OnBuildDamaged?.Invoke(this);

        Debug.Log($"Стена получила {damage} урона. Осталось HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator DamageFlash()
    {
        if (wallRenderer != null && damagedMaterial != null)
        {
            wallRenderer.material = damagedMaterial;
            yield return new WaitForSeconds(damageFlashDuration);

            if (originalMaterial != null)
                wallRenderer.material = originalMaterial;
        }
    }

    public void Heal(int amount)
    {
        // Нету
    }

    public void Die()
    {
        if (!isAlive) return;

        isAlive = false;

        OnBuildDestroyed?.Invoke(this);
        OnAnyBuildDestroyed?.Invoke(this);

        if (destroyEffectPrefab != null)
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);

        if (buildCell != null)
        {
            buildCell.ClearOccupation();

            UpdateNavGraph();
        }

        GameController.Instance?.BuildManager.UnregisteredBuilding(this);

        Debug.Log("Стена разрушена!");

        if (wallCollider != null) wallCollider.enabled = false;
        if (wallRenderer != null) wallRenderer.enabled = false;

        Destroy(gameObject, 1f);
    }

    void OnDestroy()
    {
        OnBuildDamaged = null;
        OnBuildDestroyed = null;
        if (buildCell != null && isAlive)
        {
            buildCell.ClearOccupation();
            UpdateNavGraph();
        }
    }
}
