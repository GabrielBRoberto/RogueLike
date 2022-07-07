using UnityEngine.Pool;
using UnityEngine;
using TMPro;

namespace Roguelike.Player.Gun
{
    public class GunSystem : MonoBehaviour
    {
        #region Attributes
        [Header("Bullet Prefab")]
        public GameObject bulletPrefab;

        [Header("Bullet Force")]
        public float shootForce;
        public float upwardForce;

        [Header("Gun Stats")]
        public float timeBetweenShooting;
        public float spread;
        public float reloadTime;
        public float timeBetweenShots;
        public int magazineSize;
        public int bulletsPerTap;
        public bool allowButtonHold;

        private int bulletsLeft;
        private int bulletsShot;

        [Header("Recoil")]
        public Rigidbody playerRb;
        public float recoilForce;

        //Bools
        private bool shooting;
        private bool readyToShoot;
        private bool reloading;

        [Header("Reference")]
        public Camera camera;
        public Transform attackPoint;

        [Header("Graphics")]
        public GameObject muzzleFlash;
        public TMP_Text ammunitionDisplay;

        private bool allowInvoke = true;

        //Object Pool
        [HideInInspector]
        public ObjectPool<GameObject> bulletsPool;

        #endregion

        private void Awake()
        {
            //Magazine full
            bulletsLeft = magazineSize;
            readyToShoot = true;

            bulletsPool = new ObjectPool<GameObject>(createBullet, takeBullet, releaseBullet);
        }
        private void Update()
        {
            //Check if allowed to hold down button and take corresponding input
            if (allowButtonHold)
            {
                shooting = Input.GetKey(KeyCode.Mouse0);
            }
            else
            {
                shooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            //Reloading
            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            {
                Reload();
            }
            //Reload automatically when trying to shoot without ammo
            if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            {
                Reload();
            }

            //Shooting
            if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
            {
                //Set bullets shot to 0
                bulletsShot = 0;

                Shoot();
            }

            //Update ammunationDisplay
            if (ammunitionDisplay)
            {
                ammunitionDisplay.text = bulletsLeft + " / " + magazineSize;
            }
        }

        #region Functions

        private void Shoot()
        {
            readyToShoot = false;

            //Find the exact hit position using a raycast
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));    // Just a ray through the middle of your current view.
            RaycastHit hit;

            //Check if ray hits something.
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(500); //Just a point far away from the player.
            }

            //Calculate direction from attackPoint to targetPoint
            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

            //Calculate spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            //Calculate new direction with spread
            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);    //Just add spread to last direction

            //Instantiate bullet/projectile
            GameObject bullet = bulletsPool.Get();   //Store the bullet from the pool in currentBullet

            bullet.transform.position = attackPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            bullet.transform.forward = directionWithSpread.normalized;

            //Add forces to bullet
            bullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
            bullet.GetComponent<Rigidbody>().AddForce(camera.transform.up * upwardForce, ForceMode.Impulse);

            //Instantiate muzzle flash, if you have one
            if (muzzleFlash != null)
            {
                Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            }

            bulletsLeft--;
            bulletsShot++;

            //Invoke resetShot (if not already invoked), with you timeBetweenShooting
            if (allowInvoke)
            {
                Invoke("ResetShot", timeBetweenShooting);
                allowInvoke = false;

                //Add recoil to player (should only be called once
                playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
            }

            //If more than one bulletsPerTap make sure to repeat shoot function
            if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            {
                Invoke("Shoot", timeBetweenShots);
            }
        }
        private void ResetShot()
        {
            //Allow shooting and invoking again
            readyToShoot = true;
            allowInvoke = true;
        }
        private void Reload()
        {
            reloading = true;
            Invoke("ReloadFinished", reloadTime);   //Invoke ReloadFinished function with your reloadTime as delay.
        }
        private void ReloadFinished()
        {
            //Fill magazine
            bulletsLeft = magazineSize;
            reloading = false;
        }

        private GameObject createBullet()
        {
            var bullet = Instantiate(bulletPrefab);

            return bullet;
        }
        private void takeBullet(GameObject bullet)
        {
            bullet.SetActive(true);
        }
        public void releaseBullet(GameObject bullet)
        {
            bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;

            bullet.SetActive(false);
        }

        #endregion
    }
}