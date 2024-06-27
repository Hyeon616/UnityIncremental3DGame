using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Magio
{
    /// <summary>
    /// Destroys gameobject after a while.
    /// </summary>
    public class SelfDestroy : MonoBehaviour
    {
        public float destroyDelay = 7;

        private float timer = 0;

        private void Start()
        {
            VisualEffect eff = GetComponent<VisualEffect>();
            if (eff)
            {
                if (eff.HasFloat("Magio_DestroyDelay"))
                {
                    destroyDelay = eff.GetFloat("Magio_DestroyDelay");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (timer > destroyDelay)
            {
                Destroy(this.gameObject);
            }
        }
    }
}

