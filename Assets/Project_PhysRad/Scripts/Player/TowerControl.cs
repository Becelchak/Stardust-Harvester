using Shooter.Gameplay;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class TowerControl : MonoBehaviour
{

    [Header("Основные настройки")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Префабы")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Настройки цели")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private string targetTag = "Enemy";

    private IDamageable currentTarget;
    private Transform currentTargetTransform;
    private float lastAttackTime;
    private bool isActive = true;

    private void Start()
    {
        if (firePoint == null)
            firePoint = transform;

        Enemy.OnEnemyDied += OnEnemyDied;
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(ScanForTargets());
    }

    private void Update()
    {
        if (!isActive || currentTarget == null) return;

        if (!IsTargetValid(currentTarget))
        {
            ClearTarget();
            return;
        }

        if (!currentTarget.IsAlive || !IsTargetInRange(currentTarget))
        {
            currentTarget = null;
            return;
        }

        if (Time.time >= lastAttackTime + 1f / attackRate)
        {
            AttackTarget(currentTarget);
            lastAttackTime = Time.time;
        }
    }

    private bool IsTargetValid(IDamageable target)
    {
        if (target == null) return false;

        if (!target.IsAlive) return false;

        MonoBehaviour targetMono = target as MonoBehaviour;
        if (targetMono == null || targetMono.transform == null) return false;

        return IsTargetInRange(target);
    }

    private IEnumerator ScanForTargets()
    {
        while (isActive)
        {
            if (currentTarget == null || !currentTarget.IsAlive)
            {
                FindNearestTarget();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            attackRange,
            targetLayer
        );

        IDamageable nearestTarget = null;
        Transform nearestTransform = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag(targetTag)) continue;

            IDamageable potentialTarget = collider.GetComponent<IDamageable>();
            currentTargetTransform = collider.gameObject.transform;
            if (potentialTarget == null || !potentialTarget.IsAlive) continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = potentialTarget;
                nearestTransform = collider.transform;
            }
        }

        currentTarget = nearestTarget;
        currentTargetTransform = nearestTransform;
    }

    private bool IsTargetInRange(IDamageable target)
    {
        if (target == null) return false;

        MonoBehaviour targetMono = target as MonoBehaviour;
        if (targetMono == null) return false;

        float distance = Vector3.Distance(transform.position, targetMono.transform.position);
        var newRotataion = Quaternion.LookRotation(targetMono.transform.position - transform.position);
        transform.rotation = new Quaternion(transform.rotation.x, newRotataion.y , transform.rotation.z, transform.rotation.w);
        return distance <= attackRange;
    }

    private void OnEnemyDied(Enemy deadEnemy)
    {
        if (currentTargetTransform != null && deadEnemy.transform == currentTargetTransform)
        {
            ClearTarget();

            StopCoroutine(nameof(ScanForTargets));
            StartCoroutine(ScanForTargets());
        }
    }

    private void ClearTarget()
    {
        currentTarget = null;
        currentTargetTransform = null;
    }

    private void AttackTarget(IDamageable target)
    {

        var projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        var projectale = projectile.GetComponent<Projectile_Base>();
        projectale.Initialize(currentTargetTransform.transform.position, firePoint.position);
        audioSource.PlayOneShot(shootSound);

        Debug.Log($"Башня выстрелила по цели");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (currentTarget != null)
        {
            MonoBehaviour targetMono = currentTarget as MonoBehaviour;
            if (targetMono != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetMono.transform.position);
            }
        }
    }

    public void Activate() => isActive = true;
    public void Deactivate() => isActive = false;
    public void SetAttackRate(float newRate) => attackRate = newRate;
    public void SetRange(float newRange) => attackRange = newRange;
}
