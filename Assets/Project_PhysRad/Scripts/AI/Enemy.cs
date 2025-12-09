using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Enemy : MonoBehaviour, IAttacker, IDamageable, ITarget
    {

        [SerializeField]
        protected GameObject m_DeathParticlePrefab;
        [SerializeField]
        protected GameObject m_SpawnParticlePrefab;
        [SerializeField] private EnemyData data;

        private IAstarAI aiAgent;
        private IDamageable targetToAttack;
        private Transform currentTargetTransform;
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

            currentHealth = data.maxHealth;
            aiAgent = GetComponent<IAstarAI>();
            FindStation();

        }

        void Update()
        {

            if (!IsAlive || targetToAttack == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, (currentTargetTransform.position));
            if(distanceToTarget > 45)
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

        public virtual void EnableEnemy()
        {

        }
        public void Attack(IDamageable target)
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
            if (!IsAlive)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
        }

        public void Die()
        {
            OnEnemyDied?.Invoke(this);
            OnThisEnemyDied?.Invoke(this);

            PlayerManager.Instance?.AddScrap(data.scrapReward);

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
            OnThisEnemyDied = null;
        }

        void FindNewTarget()
        {

            IBuildable nearestBuild = BuildManager.Instance?.GetNearestBuilding(transform.position, data.attackRange * 2f);
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

        void SetTarget(IDamageable newTarget, Transform newTargetTransform)
        {
            if (newTarget == targetToAttack) return;

            IDamageable oldTarget = targetToAttack;
            targetToAttack = newTarget;
            currentTargetTransform = newTargetTransform;

            if (aiAgent != null && currentTargetTransform != null)
            {
                aiAgent.destination = currentTargetTransform.position;
                aiAgent.SearchPath();
            }

            OnTargetChanged?.Invoke(this, oldTarget);

            Debug.Log($"Враг сменил цель на: {(newTarget as MonoBehaviour)?.name}");
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Build")
            {
                var damageBuild = collision.gameObject.GetComponent<IDamageable>();
                if (damageBuild == null) return;

                targetToAttack = damageBuild;
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

}