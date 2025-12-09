using Shooter.Gameplay;
using System.Collections;
using TMPro;
using UnityEngine;

public class TowerControl : MonoBehaviour
{

    [Header("Основные настройки")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackRate = 1f;

    [Header("Префабы")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Настройки цели")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private string targetTag = "Enemy";

    private IDamageable currentTarget;
    private GameObject currentTargetGameObj;
    private float lastAttackTime;
    private bool isActive = true;

    private void Start()
    {
        if (firePoint == null)
            firePoint = transform;

        StartCoroutine(ScanForTargets());
    }

    private void Update()
    {
        if (!isActive || currentTarget == null) return;

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

    private IEnumerator ScanForTargets()
    {
        while (isActive)
        {
            if (currentTarget == null || !currentTarget.IsAlive)
            {
                FindNearestTarget();
            }
            yield return new WaitForSeconds(0.2f);
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
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag(targetTag)) continue;

            IDamageable potentialTarget = collider.GetComponent<IDamageable>();
            currentTargetGameObj = collider.gameObject;
            if (potentialTarget == null || !potentialTarget.IsAlive) continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = potentialTarget;
            }
        }

        currentTarget = nearestTarget;
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


    private void AttackTarget(IDamageable target)
    {

        var projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        var projectale = projectile.GetComponent<Projectile_Base>();
        projectale.Initialize(currentTargetGameObj.transform.position, firePoint.position);

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
