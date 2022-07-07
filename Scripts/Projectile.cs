using gNox.BehaviorTreeVisualizer;
using Roguelike.Player.Lifecycle;
using System.Collections;
using UnityEngine;

namespace Roguelike.Player.Gun
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Projectile : MonoBehaviour
    {

        [Header("Assignables")]
        public Rigidbody rb;
        public GameObject explosion;
        public LayerMask whatIsEnemy;
        public bool fromPlayer;
        [SerializeField]
        private string enemyName;

        [Header("Stats")]
        [Range(0f, 1f)]
        public float bounciness;
        public bool useGravity;

        [Header("Damage")]
        private int explosionDamage;
        public float explosionRange;
        public float explosionForce;

        [Header("Lifetime")]
        public int maxCollisions;
        public float maxLifetime;
        public bool explodeOnTouch = true;

        int collisions;
        PhysicMaterial physics_mat;

        private void Setup()
        {
            //Create a new Physic material
            physics_mat = new PhysicMaterial();
            physics_mat.bounciness = bounciness;
            physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
            physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

            //Assign material to collider
            GetComponent<SphereCollider>().material = physics_mat;

            //Set gravity
            rb.useGravity = useGravity;

            if (fromPlayer)
            {
                explosionDamage = (int)GetPlayerStats().actualDamage;
            }
            else
            {

            }
        }

        private void Start()
        {
            Setup();
        }
        private void Update()
        {
            //When to explode:
            if (collisions > maxCollisions)
            {
                Explode();
            }

            //Count down lifetime
            maxLifetime -= Time.deltaTime;
            if (maxLifetime <= 0)
            {
                Explode();
            }
        }

        private void Explode()
        {
            //Instantiate explosion
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }

            //Check for enemies
            Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemy);
            for (int i = 0; i < enemies.Length; i++)
            {
                //Get component of enemy and call takeDamage
                if (fromPlayer)
                {
                    enemies[i].GetComponent<MeleeBasico>().takeDamage(explosionDamage);

                    if (GetPlayerStats().actualLifeSteal != 0)
                    {
                        float heal = explosionDamage * (GetPlayerStats().actualLifeSteal / 100);

                        FindObjectOfType<PlayerLifecycle>().heal(heal);
                    }
                }      

                //Add explosion force (If enemy has a rigidbody)
                if (enemies[i].GetComponent<Rigidbody>())
                {
                    enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
                }
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                Debug.Log(enemies[i].name);
            }

            //Add a little delay, just to make sure everything works fine
            Invoke("Delay", 0.05f);
        }

        private static PlayerStats GetPlayerStats()
        {
            return FindObjectOfType<PlayerManager>().stats;
        }

        private void Delay()
        {
            if (fromPlayer)
            {
                FindObjectOfType<GunSystem>().releaseBullet(gameObject);
            }
            else
            {
                if (enemyName == "BabyDragon")
                {
                    FindObjectOfType<GameManager>().fireballPool.Release(gameObject);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Don't coint collisions with other bullets
            if (collision.collider.CompareTag("Bullet"))
            {
                return;
            }

            //Count up collisions
            collisions++;

            //Explode if bullet hits an enemy directly and explodeOnTouch is activated
            if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
            {
                Explode();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRange);
        }
    }
}