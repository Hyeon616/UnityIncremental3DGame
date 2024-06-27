#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    public class PlayerLook : MonoBehaviour
    {
        public float mouseSens = 100;

        public Transform body;

        float xRot = 0f;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {

#if ENABLE_INPUT_SYSTEM
            float lookX = 0;
            float lookY = 0;

            if (Mouse.current != null)
            {
                var delta = Mouse.current.delta.ReadValue() / 15.0f;
                lookX += delta.x;
                lookY += delta.y;
            }
            if (Gamepad.current != null)
            {
                var value = Gamepad.current.rightStick.ReadValue() * 2;
                lookX += value.x;
                lookY += value.y;
            }

            lookX *= mouseSens * Time.deltaTime;
            lookY *= mouseSens * Time.deltaTime;
#else
            float lookX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            float lookY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
#endif


            xRot -= lookY;
            xRot = Mathf.Clamp(xRot, -90, 90);

            transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            body.Rotate(Vector3.up * lookX);
        }
    }

}
