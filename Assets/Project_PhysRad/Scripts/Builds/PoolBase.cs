using UnityEngine;
using System.Collections;

public abstract class PoolBase : MonoBehaviour, IBuildable
{
    [Header("Основные настройки")]
    [SerializeField] protected int buildCost = 75;
    [SerializeField] protected float lifetime = 10f;
    [SerializeField] protected float effectRadius = 3f;

    [Header("Визуальные эффекты")]
    [SerializeField] protected GameObject spawnEffectPrefab;
    [SerializeField] protected GameObject destroyEffectPrefab;
    [SerializeField] protected Renderer poolRenderer;
    [SerializeField] protected float fadeDuration = 1f;

    [Header("Настройки взаимодействия")]
    [SerializeField] protected LayerMask affectedLayers;
    [SerializeField] protected bool affectStation = false;

    public event System.Action<IBuildable> OnBuildDamaged;
    public event System.Action<IBuildable> OnBuildDestroyed;
    public static event System.Action<IBuildable> OnAnyPoolDestroyed;

    protected BuildCell buildCell;
    protected bool isActive = true;
    protected float currentLifetime;
    protected Collider poolCollider;

    public int BuildCost => buildCost;
    public bool CanBuild => true;
    public BuildCell BuildCell
    {
        get => buildCell;
        set => buildCell = value;
    }

    void Awake()
    {
        poolCollider = GetComponent<Collider>();
        if (poolRenderer == null)
            poolRenderer = GetComponent<Renderer>();
    }

    public void OnBuild(BuildCell cell)
    {
        buildCell = cell;
        currentLifetime = lifetime;
        isActive = true;

        if (spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                spawnEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(effect, 3f);
        }

        UpdateNavigation(false);

        StartCoroutine(LifetimeCountdown());
        StartCoroutine(EffectUpdateLoop());

        GameController.Instance?.BuildManager.RegisterBuilding(this);

        Debug.Log($"Лужа создана на клетке {cell.GridCoordinate}, время жизни: {lifetime}с");
    }

    public void OnDestroyed()
    {
        if (buildCell != null)
        {
            buildCell.ClearOccupation();
        }

        if (destroyEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                destroyEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(effect, 3f);
        }

        OnBuildDestroyed?.Invoke(this);
        OnAnyPoolDestroyed?.Invoke(this);

        UpdateNavigation(true);

        GameController.Instance?.BuildManager.UnregisteredBuilding(this);
        Destroy(gameObject);
    }

    protected virtual IEnumerator EffectUpdateLoop()
    {
        while (isActive)
        {
            ApplyEffectToTargets();
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected virtual IEnumerator LifetimeCountdown()
    {
        while (currentLifetime > 0 && isActive)
        {
            currentLifetime -= Time.deltaTime;

            UpdateVisualLifetimeIndicator();

            yield return null;
        }

        if (isActive)
            OnDestroyed();
    }

    protected virtual void UpdateVisualLifetimeIndicator()
    {
        if (poolRenderer == null) return;

        float lifetimePercent = currentLifetime / lifetime;

        Color color = poolRenderer.material.color;
        color.a = Mathf.Lerp(0.3f, 1f, lifetimePercent);
        poolRenderer.material.color = color;

        if (currentLifetime < 3f)
        {
            float pulse = Mathf.PingPong(Time.time * 0.2f, 0.2f);
            transform.localScale = Vector3.one * (0.2f + pulse);
        }
    }

    protected abstract void ApplyEffectToTargets();
    protected abstract void OnEnemyEnterEffect(Enemy enemy);
    protected abstract void OnEnemyExitEffect(Enemy enemy);

    protected void UpdateNavigation(bool makeWalkable)
    {
        if (poolCollider == null || AstarPath.active == null) return;

        var graphUpdate = new Pathfinding.GraphUpdateObject(poolCollider.bounds);
        graphUpdate.modifyWalkability = true;
        graphUpdate.setWalkability = makeWalkable;
        AstarPath.active.UpdateGraphs(graphUpdate);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (((1 << other.gameObject.layer) & affectedLayers) == 0) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy.IsAlive)
        {
            OnEnemyEnterEffect(enemy);
        }

        if (affectStation && other.CompareTag("Station"))
        {
            IDamageable station = other.GetComponent<IDamageable>();
            if (station != null)
            {
                OnStationEnterEffect(station);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!isActive) return;

        if (((1 << other.gameObject.layer) & affectedLayers) == 0) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            OnEnemyExitEffect(enemy);
        }
    }

    protected virtual void OnStationEnterEffect(IDamageable station)
    {

    }

    void OnDestroy()
    {
        isActive = false;
        StopAllCoroutines();

        // Отладочное сообщение
        if (buildCell != null)
        {
            Debug.LogWarning($"Лужа уничтожается, но клетка не была очищена! Координаты: {buildCell.GridCoordinate}");
        }
        else
        {
            Debug.Log("Лужа уничтожена, клетка очищена");
        }

        OnBuildDamaged = null;
        OnBuildDestroyed = null;
    }

    void OnDrawGizmosSelected()
    {
        if (!isActive) return;

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, effectRadius);

        Gizmos.color = new Color(1, 0.5f, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, effectRadius);
    }
}