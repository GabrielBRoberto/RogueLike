using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Roguelike.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerManager manager;

        #region Movement
        [SerializeField]
        private LayerMask groundMask = 1;

        private Vector3 movementInputValues;
        private bool jumpingInputValue;
        private bool isGrounded;

        private Rigidbody rb;
        private CapsuleCollider collider;

        private float acceleration = 2.5f;
        private float movementSmoothing = 0.3f;
        private Vector3 vector3_Reference;
        #endregion

        #region Camera
        private Vector2 lookingInputValue;
        [SerializeField]
        private float horizontalRange = 50f;
        [SerializeField]
        private float verticalRange = 50f;
        private Vector3 targetRotation;

        [SerializeField]
        private GameObject camera;

        private float damping = 10f;
        private Vector3 cameraOffset = new Vector3(0, 0.8f, 0);
        #endregion

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            manager = GetComponent<PlayerManager>();
            collider = GetComponent<CapsuleCollider>();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Inputs();

            Looking();

            Jumping();
        }
        private void FixedUpdate()
        {
            Accelerate();

            RotateRigidbodyToLookDirection(rb);
        }
        private void LateUpdate()
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, transform.position + cameraOffset, (damping * 10) * Time.deltaTime);
        }

        private void Inputs()
        {
            lookingInputValue.x = Input.GetAxis("Mouse X");
            lookingInputValue.y = Input.GetAxis("Mouse Y");

            movementInputValues.x = Input.GetAxis("Horizontal");
            movementInputValues.y = Input.GetAxis("Vertical");

            jumpingInputValue = Input.GetButtonDown("Jump");
        }

        #region Movement + Camera

        private void Accelerate()
        {
            if (!rb)
            {
                return;
            }
            Vector3 delta = new Vector3(movementInputValues.x, 0, movementInputValues.y);
            delta = Vector3.ClampMagnitude(delta, 1);
            delta = rb.transform.TransformDirection(delta) * acceleration * manager.stats.speed;
            Vector3 target = new Vector3(delta.x, rb.velocity.y, delta.z);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, target, ref vector3_Reference, Time.smoothDeltaTime * movementSmoothing);
        }
        private void Jumping()
        {
            if (jumpingInputValue)
            {
                rb.velocity = new Vector3(rb.velocity.x, manager.stats.jumpForce, rb.velocity.z);
            }
        }

        private void Looking()
        {
            if (!camera)
            {
                return;
            }
            if (System.Math.Abs(lookingInputValue.x) < Physics.sleepThreshold && System.Math.Abs(lookingInputValue.y) < Physics.sleepThreshold)
            {
                return;
            }
            lookingInputValue.y *= -1;

            targetRotation.x += lookingInputValue.y * manager.stats.sensitivity * Time.deltaTime;
            targetRotation.y += lookingInputValue.x * manager.stats.sensitivity * Time.deltaTime;
            targetRotation.y = Mathf.Repeat(targetRotation.y, 360);

            if (System.Math.Abs(horizontalRange) > Physics.sleepThreshold)
            {
                targetRotation.y = Mathf.Clamp(targetRotation.y, -horizontalRange, horizontalRange);
            }
            if (System.Math.Abs(verticalRange) > Physics.sleepThreshold)
            {
                targetRotation.x = Mathf.Clamp(targetRotation.x, -verticalRange, verticalRange);
            }

            camera.transform.rotation = Quaternion.Euler(targetRotation);
        }
        private void RotateRigidbodyToLookDirection(Rigidbody rigidbody)
        {
            if (!camera)
            {
                return;
            }
            rigidbody.rotation = Quaternion.Euler(0, camera.transform.rotation.eulerAngles.y, 0);
        }

        #endregion
    }
}