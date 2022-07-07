using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;

namespace gNox.BehaviorTreeVisualizer
{
    public enum canSeePlayer
    {
        canSee,
        cannotSee
    }
    public enum canAttackPlayer
    {
        canAttack,
        cannotAttack
    }
    public enum takingDamage
    {
        takingDamage,
        notTakingDamage
    }

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class MeleeBasico : MonoBehaviour, IBehaviorTree
    {
        public NodeBase BehaviorTree { get; set; }
        public canSeePlayer canSeePlayer { get; set; }
        public canAttackPlayer canAttackPlayer { get; set; }
        public takingDamage takingDamage { get; set; }

        [Header("References")]
        [SerializeField]
        private GameObject player;
        [SerializeField]
        private EnemyStats enemyStats;

        private float maxHealth;
        [SerializeField]
        private float actualHealth;

        private float healthCheck;

        private float maxShield;
        private float actualShield;

        private float resistence;

        [HideInInspector]
        public float damage;

        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public NavMeshAgent agent;

        private bool canTakeDamage = true;
        public bool isAttacking = false;
        [HideInInspector]
        public bool isTakingDamage = false;

        private const string ANIMATOR_IDLE = "Sword + Shield Idle";
        private const string ANIMATOR_MOVING = "Sword + Shield Move";
        private const string ANIMATOR_ATTACK = "Sword + Shield Attack";
        private const string ANIMATOR_DEFENDING = "Sword + Shield Defend";

        private string ANIMATOR_CURRENT;

        private float delay;

        private void Awake()
        {
            Setup();
        }
        private void Update()
        {
            Checks();
        }

        #region BehaviorTree

        private void GenerateBehaviorTree()
        {
            BehaviorTree =
                new Selector("Root",
                    new Sequence("AttackArea",
                        new canSeePlayerCheck(canSeePlayer.canSee, this),
                        new Selector("Selector",
                            new Sequence("Attack",
                               new canAttackPlayerCheck(canAttackPlayer.canAttack, this),
                               new AttackNode(this, ANIMATOR_ATTACK, player.transform)),
                            new Sequence("Defend",
                                new takingDamageCheck(takingDamage.takingDamage, this),
                                new DefendNode(this, ANIMATOR_DEFENDING, player.transform)))));
        }

        private Coroutine m_BehaviourTreeRoutine;
        private YieldInstruction m_WaitTime = new WaitForSeconds(.01f);

        private IEnumerator RunBehaviorTree()
        {
            while (enabled)
            {
                if (BehaviorTree == null)
                {
                    $"{this.GetType().Name} is missing BehaviorTree. Did you set the BehaviorTree property??".BTDebugLog();
                    continue;
                }

                (BehaviorTree as Node).Run();

                yield return m_WaitTime;
            }
        }

        private void OnDestroy()
        {
            if (m_BehaviourTreeRoutine != null)
            {
                StopCoroutine(m_BehaviourTreeRoutine);
            }
        }

        #endregion

        private void Setup()
        {
            maxHealth = enemyStats.maxHealth;
            actualHealth = maxHealth;

            healthCheck = actualHealth;

            maxShield = enemyStats.maxShield;
            actualShield = maxShield;

            resistence = enemyStats.actualResistence;
            damage = enemyStats.actualDamage;

            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            canAttackPlayer = canAttackPlayer.cannotAttack;

            GenerateBehaviorTree();

            if (m_BehaviourTreeRoutine == null && BehaviorTree != null)
            {
                m_BehaviourTreeRoutine = StartCoroutine(RunBehaviorTree());
            }
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
                        //StartCoroutine(Death());
                    }
                }
            }
        }

        private float Distance()
        {
            return Vector3.Distance(transform.position, player.transform.position);
        }

        public void ChangeAnimationState(string NEW_STATE)
        {
            if (ANIMATOR_CURRENT == NEW_STATE)
            {
                return;
            }

            animator.Play(NEW_STATE);

            ANIMATOR_CURRENT = NEW_STATE;
        }

        private void Checks()
        {
            if (canAttackPlayer == canAttackPlayer.cannotAttack)
            {
                if (takingDamage == takingDamage.notTakingDamage)
                {
                    if (!isAttacking)
                    {
                        ChangeAnimationState(ANIMATOR_IDLE);
                    }
                }
            }

            if (Distance() > 2f)
            {
                canAttackPlayer = canAttackPlayer.cannotAttack;
            }
            else
            {
                canAttackPlayer = canAttackPlayer.canAttack;
            }

            if (isTakingDamage)
            {
                takingDamage = takingDamage.takingDamage;
            }
            else
            {
                takingDamage = takingDamage.notTakingDamage;
            }

            if (isAttacking)
            {
                delay = animator.GetCurrentAnimatorStateInfo(0).length;

                Invoke("AttackComplete", delay);
            }

            if (healthCheck != actualHealth)
            {
                isTakingDamage = true;

                Invoke("DamageComplete", 2f);
            }
        }

        private void AttackComplete()
        {
            isAttacking = false;
        }

        private void DamageComplete()
        {
            isTakingDamage = false;

            healthCheck = actualHealth;
        }
    }
}