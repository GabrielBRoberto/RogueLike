using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Roguelike.Player.Lifecycle
{
    public class PlayerLifecycle : MonoBehaviour
    {
        private PlayerManager manager;

        private float epsilon = .01f;
        private bool canRegen;

        private void Start()
        {
            manager = GetComponent<PlayerManager>();

            manager.stats.actualHealth = manager.stats.maxHealth;
            manager.stats.actualShield = manager.stats.maxShield;
        }

        private void Update()
        {
            if (manager.stats.regenHealthRate != 0)
            {
                healthRecovery();
            }
            if (manager.stats.actualRegenShieldRate != 0)
            {
                shieldRecovery();
            }
        }

        private void shieldRecovery()
        {
            if (!canRegen)
            {
                return;
            }
            if (manager.stats.actualRegenShieldRate == 0)
            {
                return;
            }
            if (Time.frameCount % manager.stats.actualRegenShieldRate == 0 && Mathf.Abs(manager.stats.actualShield - manager.stats.maxShield) > epsilon)
            {
                if (manager.stats.actualShield < manager.stats.maxShield)
                {
                    if (!canRegen)
                    {
                        return;
                    }

                    manager.stats.actualShield += 1;
                }
                else
                {
                    manager.stats.actualShield = manager.stats.maxShield;
                }
            }
        }

        private void healthRecovery()
        {
            if (manager.stats.regenHealthRate == 0)
            {
                return;
            }
            if (Time.frameCount % manager.stats.regenHealthRate == 0 && Mathf.Abs(manager.stats.actualHealth - manager.stats.maxHealth) > epsilon)
            {
                if (manager.stats.actualHealth < manager.stats.maxHealth)
                {
                    manager.stats.actualHealth += 1;
                }
                else
                {
                    manager.stats.actualHealth = manager.stats.maxHealth;
                }
            }
        }

        public void takeDamage(float damageValue)
        {
            float damage;

            if (manager.stats.actualShield > 0)
            {
                damage = damageValue;

                if (manager.stats.actualResistence != 0)
                {
                    damage *= (1 - (manager.stats.actualResistence / 100));
                }

                if (damage > manager.stats.actualShield)
                {
                    damage -= manager.stats.actualShield;
                    manager.stats.actualShield = 0;

                    manager.stats.actualHealth -= damage;

                    StartCoroutine(delayShieldRegen());
                }
                else
                {
                    manager.stats.actualShield -= damage;

                    StartCoroutine(delayShieldRegen());
                }
            }
            else
            {
                damage = damageValue;

                if (manager.stats.actualResistence != 0)
                {
                    damage *= (1 - (manager.stats.actualResistence / 100));
                }

                manager.stats.actualHealth -= damage;

                StartCoroutine(delayShieldRegen());

                if (manager.stats.actualHealth <= 0)
                {
                    Death();
                }
            }
        }
        public virtual void heal(float healValue)
        {
            if (manager.stats.actualHealth + healValue > manager.stats.maxHealth)
            {
                manager.stats.actualHealth = manager.stats.maxHealth;
            }
            else
            {
                manager.stats.actualHealth += healValue;
            }
        }

        private void Death()
        {
            gameObject.SetActive(false);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Lobby"));
        }

        IEnumerator delayShieldRegen()
        {
            canRegen = false;

            yield return new WaitForSeconds(2f);

            canRegen = true;

            yield return null;
        }
    }
}