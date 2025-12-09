using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Enemy : MonoBehaviour, IAttacker, IDamageable, ITarget
    {
        protected DamageControl m_DamageControl;
        [SerializeField]
        protected GameObject m_DeathParticlePrefab;
        [SerializeField]
        protected GameObject m_SpawnParticlePrefab;
        [SerializeField] private EnemyData data;

        private IAstarAI aiAgent;
        private IDamageable targetToAttack;
        private float lastAttackTime;

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

        public Transform targetCenter => transform;


        public int currentHealth;
        void Start()
        {

            GameObject obj = Instantiate(m_SpawnParticlePrefab);
            obj.transform.position = transform.position;
            Destroy(obj, 3);

            currentHealth = data.maxHealth;
            aiAgent = GetComponent<IAstarAI>();
            GameObject stationObj = GameObject.FindGameObjectWithTag("Station");
            if (stationObj != null)
            {
                targetToAttack = stationObj.GetComponent<IDamageable>();

                if (aiAgent != null)
                    aiAgent.destination = stationObj.transform.position;
            }

        }

        void Update()
        {

            if (!IsAlive || targetToAttack == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, ((MonoBehaviour)targetToAttack).transform.position);
            if (distanceToTarget <= data.attackRange)
            {
                if (aiAgent != null)
                    aiAgent.isStopped = true;

                if (Time.time >= lastAttackTime + 1f / data.attackRate)
                {
                    Attack(targetToAttack);
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                if (aiAgent != null)
                    aiAgent.isStopped = false;
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
                Debug.Log($"Атакую станцию! У нее осталось {target.CurrentHealth}");
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
            PlayerManager.Instance?.AddScrap(data.scrapReward);

            if (aiAgent != null)
                aiAgent.isStopped = true;

            Destroy(gameObject, 0.1f);
        }
    }

}