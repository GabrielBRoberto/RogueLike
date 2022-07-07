using System.Collections;
using UnityEngine.AI;
using UnityEngine;

namespace Roguelike.Enemy
{
    public enum IAStates
    {
        Attack,
        Chase
    }

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        [Header("References")]
        private NavMeshAgent agent;
        public Transform player;
        private Animator animator;

        private IAStates states = IAStates.Chase;

        [SerializeField]
        private EnemyStats stats;
        private Material material;

        private float maxHealth;
        private float actualHealth;

        private float maxShield;
        private float actualShield;

        private float resistence;
        [HideInInspector]
        public float damage;

        private bool canTakeDamage;
        private bool isAttacking = false;

        private const string ANIMATOR_ATTACK = "Attack";
        private const string ANIMATOR_CHASE = "Chase";
        private string ANIMATOR_CURRENT_STATE;

        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            maxHealth = stats.maxHealth;
            actualHealth = maxHealth;

            maxShield = stats.maxShield;
            actualShield = maxShield;

            resistence = stats.actualResistence;
            damage = stats.actualDamage;

            material = GetComponent<MeshRenderer>().material;
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            agent.speed = stats.speed;
        }

        private void Update()
        {
            if (actualHealth <= 0)
            {
                canTakeDamage = false;
            }
            else
            {
                canTakeDamage = true;
            }

            ChangeFSM();
        }

        private void ChangeFSM()
        {
            switch (states)
            {
                case IAStates.Attack:
                    Attack();
                    break;
                case IAStates.Chase:
                    Chase();
                    break;
            }

            if (Distance() > 1.5f)
            {
                states = IAStates.Chase;
            }
            else
            {
                states = IAStates.Attack;
            }
        }

        private float Distance()
        {
            return Vector3.Distance(transform.position, player.position);
        }

        private void Attack()
        {
            isAttacking = true;
            agent.isStopped = true;

            ChangeAnimationState(ANIMATOR_ATTACK);

            float attackDelay = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke("AttackComplete", attackDelay);
        }

        private void AttackComplete()
        {
            isAttacking = false;
        }

        private void Chase()
        {
            if (!isAttacking)
            {
                ChangeAnimationState(ANIMATOR_CHASE);

                agent.SetDestination(player.position);

                agent.isStopped = false;
            }
        }

        private void ChangeAnimationState(string NEW_STATE)
        {
            if (ANIMATOR_CURRENT_STATE == NEW_STATE)
            {
                return;
            }

            animator.Play(NEW_STATE);

            ANIMATOR_CURRENT_STATE = NEW_STATE;
        }

        public void takeDamage(float damageValue)
        {
            if (canTakeDamage)
            {
                float damage = damageValue;

                if (actualShield > 0)
                {
                    if (resistence != 0)
                    {
                        damage *= (1 - (resistence / 100));
                    }

                    if (damage > actualShield)
                    {
                        damage -= actualShield;
                        actualShield = 0;

                        actualHealth -= damage;
                    }
                    else
                    {
                        actualShield -= damage;
                    }
                }
                else
                {
                    if (resistence != 0)
                    {
                        damage *= (1 - (resistence / 100));
                    }

                    actualHealth -= damage;

                    if (actualHealth <= 0)
                    {
                        StartCoroutine(Death());
                    }
                }
            }
        }

        private IEnumerator Death()
        {
            agent.isStopped = true;

            float time = 0;

            while (material.GetFloat("_time") <= 1.5f)
            {
                time += 0.05f;

                yield return new WaitForSeconds(0.025F);

                material.SetFloat("_time", time);
            }

            gameObject.SetActive(false);

            yield return null;
        }
    }
}