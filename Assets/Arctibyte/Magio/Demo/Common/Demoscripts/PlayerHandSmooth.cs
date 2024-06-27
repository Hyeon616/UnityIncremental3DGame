using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    public class PlayerHandSmooth : MonoBehaviour
    {
        public Transform playerCamera;

        // Update is called once per frame
        void Update()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, playerCamera.rotation, 0.5f);
        }
    }
}

