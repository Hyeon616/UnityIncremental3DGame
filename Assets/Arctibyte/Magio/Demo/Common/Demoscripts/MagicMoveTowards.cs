using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Magio
{
    public class MagicMoveTowards : MonoBehaviour
    {
        public Vector3 target;
        public float speed = 2f;
        VisualEffect eff;



        private void Start()
        {
            eff = GetComponent<VisualEffect>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }
}

