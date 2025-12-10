using Pathfinding;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IAttacker, IDamageable, ITarget
{

    [SerializeField]
    protected GameObject m_DeathParticlePrefab;
    [SerializeField]
    protected GameObject m_SpawnParticlePrefab;
    [SerializeField] protected EnemyData data;

    protected PlayerStationControl stationControl;

    private IAstarAI aiAgent;
    private IDamageable targetToAttack;
    private Transform currentTargetTransform;
    private IBuildable currentBuildableTarget;
    private float lastAttackTime;
    private bool canSwitchTarget = true;

    public int AttackDamage => data.attackDamage;
    public float AttackRange => data.attackRange;
    public float AttackRate => data.attackRate;
    public bool CanAttack => false;
    public  bool IsAlive => currentHealth > 0;
    int IDamageable.CurrentHealth
    {
        get => currentHealth;
    }
    public int MaxHealth => data.maxHealth;

    public Transform enemyCenter => transform;

    public static event Action<Enemy> OnEnemyDied;
    public event Action<Enemy> OnThisEnemyDied;
    public event Action<Enemy, IDamageable> OnTargetChanged;

    public int currentHealth;

    void Start()
    {

        GameObject obj = Instantiate(m_SpawnParticlePrefab);
        obj.transform.position = transform.position;
        Destroy(obj, 3);

        stationControl = GameController.Instance?.Station;

        currentHealth = data.maxHealth;
        aiAgent = GetComponent<IAstarAI>();
        Wall.OnAnyBuildDestroyed += OnAnyBuildDestroyed;
        FindStation();

    }

    void Update()
    {
        if (!IsAlive) return;

        if (!IsTargetValid(targetToAttack))
        {
            FindNewTarget();
            return;
        }

        if (currentTargetTransform == null)
        {
            FindNewTarget();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTargetTransform.position);

        if (distanceToTarget > 45)
            Die();

        if (distanceToTarget <= data.attackRange)
        {
            if (aiAgent != null)
                aiAgent.isStopped = true;

            if (Time.time >= lastAttackTime + 1f / data.attackRate)
            {
                Attack(targetToAttack);
                lastAttackTime = Time.time;
            }
            canSwitchTarget = false;
        }
        else
        {
            if (aiAgent != null)
                aiAgent.isStopped = false;
            canSwitchTarget = true;
        }

        if (canSwitchTarget)
        {
            FindNewTarget();
        }
    }

    private bool IsTargetValid(IDamageable target)
    {
        if (target == null) return false;
        if (!target.IsAlive) return false;

        MonoBehaviour targetMono = target as MonoBehaviour;
        if (targetMono == null) return false;
        if (targetMono.transform == null) return false;

        return true;
    }

    public virtual void EnableEnemy()
    {

    }
    public virtual void Attack(IDamageable target)
    {
        if (target != null)
        {
            target.TakeDamage(data.attackDamage);
            Debug.Log($"Атакую! У цели осталось {target.CurrentHealth}");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("ПОЛУЧИЛ УРОН ОТ ЯДА");
        if (!IsAlive)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
    }

    public virtual void Die()
    {
        OnEnemyDied?.Invoke(this);
        OnThisEnemyDied?.Invoke(this);

        if (stationControl != null)
            stationControl.AddScrap(data.scrapReward);


        if (aiAgent != null)
            aiAgent.isStopped = true;

        if (m_DeathParticlePrefab != null)
        {
            GameObject deathParticles = Instantiate(
                m_DeathParticlePrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(deathParticles, 2f);
        }

        OnThisEnemyDied = null;

        Destroy(gameObject, 0.1f);
    }

    void OnDestroy()
    {
        Wall.OnAnyBuildDestroyed -= OnAnyBuildDestroyed;
        UnsubscribeFromBuildableEvents();
        OnThisEnemyDied = null;
    }

    void FindNewTarget()
    {

        IBuildable nearestBuild = GameController.Instance?.BuildManager.GetNearestBuilding(transform.position, data.attackRange * 2f);
        if (nearestBuild != null)
        {
            var damageblaBuild = ((MonoBehaviour)nearestBuild).GetComponent<IDamageable>();
            if (damageblaBuild != null)
            {
                SetTarget(damageblaBuild, ((MonoBehaviour)nearestBuild).transform);
            }
            return;
        }

        FindStation();
    }

    void SetTarget(IDamageable newTarget, Transform newTargetTransform, IBuildable newBuildable = null)
    {
        if (newTarget == targetToAttack) return;

        UnsubscribeFromBuildableEvents();

        IDamageable oldTarget = targetToAttack;
        targetToAttack = newTarget;
        currentTargetTransform = newTargetTransform;
        currentBuildableTarget = newBuildable;

        if (currentBuildableTarget != null)
        {
            currentBuildableTarget.OnBuildDestroyed += OnCurrentBuildDestroyed;
        }

        if (aiAgent != null && currentTargetTransform != null)
        {
            aiAgent.destination = currentTargetTransform.position;
            aiAgent.SearchPath();
        }

        OnTargetChanged?.Invoke(this, oldTarget);
        Debug.Log($"Враг сменил цель на: {(newTarget as MonoBehaviour)?.name}");
    }

    protected void OnAnyBuildDestroyed(IBuildable destroyedBuild)
    {
        if (currentBuildableTarget != null && currentBuildableTarget == destroyedBuild)
        {
            Debug.Log($"Текущая цель врага разрушена, ищем новую цель");
            ForceFindNewTarget();
        }
    }

    private void OnCurrentBuildDestroyed(IBuildable destroyedBuild)
    {
        if (currentBuildableTarget == destroyedBuild)
        {
            Debug.Log($"Текущая постройка уничтожена, ищем новую цель");
            ForceFindNewTarget();
        }
    }

    private void ForceFindNewTarget()
    {
        targetToAttack = null;
        currentTargetTransform = null;

        UnsubscribeFromBuildableEvents();
        currentBuildableTarget = null;

        FindNewTarget();
    }

    private void UnsubscribeFromBuildableEvents()
    {
        if (currentBuildableTarget != null)
        {
            currentBuildableTarget.OnBuildDestroyed -= OnCurrentBuildDestroyed;
            currentBuildableTarget = null;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Build"))
        {
            var damageBuild = collision.gameObject.GetComponent<IDamageable>();
            var buildable = collision.gameObject.GetComponent<IBuildable>();

            if (damageBuild != null && damageBuild.IsAlive)
            {
                SetTarget(damageBuild, collision.transform, buildable);
            }
        }
    }

    public void FindStation()
    {
        GameObject stationObj = GameObject.FindGameObjectWithTag("Station");
        if (stationObj != null)
        {
            targetToAttack = stationObj.GetComponent<IDamageable>();

            if (aiAgent != null)
                aiAgent.destination = stationObj.transform.position;
        }
        currentTargetTransform = ((MonoBehaviour)targetToAttack).transform;
    }
}