using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace Roguelike.Enemy
{
    public class EnemyGeneric : MonoBehaviour
    {
        [SerializeField]
        private EnemyStats stats;
        private Material material;

        private float maxHealth;
        private float actualHealth;

        private float maxShield;
        private float actualShield;

        private float resistence;

        private void Start()
        {
            maxHealth = stats.maxHealth;
            actualHealth = stats.maxHealth;

            maxShield = stats.maxShield;
            actualShield = stats.maxShield;

            resistence = stats.actualResistence;

            material = GetComponent<MeshRenderer>().material;
        }
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                takeDamage(100);
            }
        }

        public void takeDamage(float damageValue)
        {
            float damage;

            if (actualShield > 0)
            {
                damage = damageValue;

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
                damage = damageValue;

                if (resistence != 0)
                {
                    damage *= (1-(resistence / 100));
                }

                actualHealth -= damage;

                if (actualHealth <= 0)
                {
                    StartCoroutine(Death());
                }
            }
        }

        private IEnumerator Death()
        {
            float time = 0;

            while (material.GetFloat("_time") <= 1.5f)
            {
                time += 0.05f;

                yield return new WaitForSeconds(0.025F);

                Debug.Log(time);

                material.SetFloat("_time", time);
            }

            gameObject.SetActive(false);

            yield return null;
        }
    }
}