using gNox.BehaviorTreeVisualizer;
using Roguelike.Player.Lifecycle;
using Roguelike.Player.Movement;
using UnityEngine.UI;
using UnityEngine;

namespace Roguelike.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerLifecycle))]
    public class PlayerManager : MonoBehaviour
    {
        [Header("References")]
        public PlayerStats stats;
        public Slider slider_Health;
        public Slider slider_Shield;

        private bool canTakeDamage = true;

        private void Start()
        {
            slider_Health.maxValue = stats.maxHealth;
            slider_Shield.maxValue = stats.maxShield;

            slider_Health.value = stats.actualHealth;
            slider_Shield.value = stats.actualShield;
        }
        private void Update()
        {
            slider_Health.value = stats.actualHealth;
            slider_Shield.value = stats.actualShield;

            if (Input.GetKeyDown(KeyCode.E))
            {
                GetComponent<PlayerLifecycle>().takeDamage(10);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("EnemyAttack"))
            {
                float damage = other.GetComponentInParent<MeleeBasico>().damage;

                if (canTakeDamage)
                {
                    GetComponent<PlayerLifecycle>().takeDamage(damage);

                    canTakeDamage = false;

                    Invoke("DamageComplete", 1f);
                }
            }
        }

        private void DamageComplete()
        {
            canTakeDamage = true;
        }
    }
}