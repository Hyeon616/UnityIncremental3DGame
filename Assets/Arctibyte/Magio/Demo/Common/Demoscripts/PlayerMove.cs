#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    public class PlayerMove : MonoBehaviour
    {
        public CharacterController playerController;

        public float speed = 12f;
        public float gravity = -10f;
        public float jumpHeight = 2f;

        public Transform groundCheck;
        public float groundDist;
        public LayerMask groundMask;

        Vector3 vel;
        bool isGrounded;

#if ENABLE_INPUT_SYSTEM
        InputAction move;
        InputAction jump;

        void Start()
        {
            move = new InputAction("PlayerMovement", binding: "<Gamepad>/leftStick");
            move.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");

            jump = new InputAction("PlayerJump", binding: "<Gamepad>/a");
            jump.AddBinding("<Keyboard>/space");

            move.Enable();
            jump.Enable();
        }

#endif

        // Update is called once per frame
        void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

            if (isGrounded && vel.y < 0)
            {
                vel.y = -2;
            }

            float xVal;
            float zVal;
            bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
            var delta = move.ReadValue<Vector2>();
            xVal = delta.x;
            zVal = delta.y;
            jumpPressed = Mathf.Approximately(jump.ReadValue<float>(), 1);
#else
            xVal = Input.GetAxis("Horizontal");
            zVal = Input.GetAxis("Vertical");
            jumpPressed = Input.GetButtonDown("Jump");
#endif

            Vector3 moveVal = transform.right * xVal + transform.forward * zVal;

            playerController.Move(moveVal * speed * Time.deltaTime);

            if (jumpPressed && isGrounded)
            {
                vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            vel.y += gravity * Time.deltaTime;

            playerController.Move(vel * Time.deltaTime);
        }
    }

}
