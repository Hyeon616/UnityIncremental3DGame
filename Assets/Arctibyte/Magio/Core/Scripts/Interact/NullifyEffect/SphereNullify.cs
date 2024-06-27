using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Adds ability to Nullify/Extinguish the effect using Spherecast.
    /// </summary>
    public class SphereNullify : MonoBehaviour
    {
        public enum NullifyMode
        {
            Area,
            Incremental
        }

        [Tooltip("Area = Nullify exactly this area. Incremental = Increment the nullified area as the sphere moves")]
        public NullifyMode nullifyMode = NullifyMode.Area;

        [Tooltip("How many times in second to check")]
        public float checkFrequency = 10;

        [Tooltip("How large is the raycast area")]
        public float raycastRadius = 1f;

        [Tooltip("Raycast layermask")]
        public LayerMask mask = ~0;

        [Tooltip("Start invoking the repeating raycast on start ")]
        public bool repeatingRaycast = true;

        [Tooltip("This class will be affected by this particle. For multiple classes add multiple scripts.")]
        public EffectClass affectedClass = EffectClass.Default;

        private List<MagioObjectEffect> magioObjs = new List<MagioObjectEffect>();

        // Start is called before the first frame update
        void Start()
        {
            if (repeatingRaycast)
                InvokeRepeating(nameof(SphereNullifyCast), 1 / checkFrequency, 1 / checkFrequency);
        }

        private void Update()
        {
            foreach(MagioObjectEffect magioObj in magioObjs)
            {
                if (nullifyMode == NullifyMode.Incremental)
                {
                    magioObj.IncrementalNullify(transform.position, raycastRadius, 0);
                }
                else
                {
                    magioObj.SetNullifyArea(transform.position, raycastRadius);
                }
            }
        }

        /// <summary>
        /// Casts a raycast sphere once looking for ignite the flammable objects. Uses the public variables for the parameters.
        /// </summary>
        public void SphereNullifyCast()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            magioObjs.Clear();
            Collider[] hits = Physics.OverlapSphere(transform.position, raycastRadius, mask);
            if (hits.Length > 0)
            {
                foreach (Collider hit in hits)
                {
                    Magio.MagioObjectMaster magioTarget = hit.gameObject.GetComponentInParent<Magio.MagioObjectMaster>();
                    if (magioTarget)
                    {
                        foreach (MagioObjectEffect hitMagioObj in magioTarget.magioObjects)
                        {
                            if (hitMagioObj && hitMagioObj.effectClass == affectedClass)
                            {
                                magioObjs.Add(hitMagioObj);
                            }
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, raycastRadius);
        }
    }
}
