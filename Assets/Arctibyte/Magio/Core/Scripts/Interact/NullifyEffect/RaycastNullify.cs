using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Adds ability to Nullify/Extinguish the effect using Raycast.
    /// </summary>
    public class RaycastNullify : MonoBehaviour
    {
        [Tooltip("Which direction to check")]
        public Vector3 direction = new Vector3(0, -1, 0);

        [Tooltip("Offset of ray starting point")]
        public Vector3 startOffset = new Vector3(0, 0, 0);

        [Tooltip("How many times in second to check")]
        public float checkFrequency = 10;

        [Tooltip("How large is the raycast area")]
        public float raycastRadius = 0.1f;

        [Tooltip("How much radius is incremented each consicutive hit")]
        public float radiusIncrement = 0.01f;

        [Tooltip("Max distance from the cast pos")]
        public float maxDist = 3f;

        [Tooltip("Raycast layermask")]
        public LayerMask mask = ~0;

        [Tooltip("This class will be affected by this raycast. For multiple classes add multiple scripts.")]
        public EffectClass affectedClass = EffectClass.Default;

        [Tooltip("Start invoking the repeating raycast on start ")]
        public bool repeatingRaycast = true;

        [Tooltip("Should raycast go through objects and extinguish everything on the path?")]
        public bool goThroughObjects = true;

        // Start is called before the first frame update
        void Start()
        {
            if(repeatingRaycast)
                InvokeRepeating(nameof(CastRayCastNullify), 1 / checkFrequency, 1 / checkFrequency);
        }

        /// <summary>
        /// Casts a raycast sphere looking for extinguishing the flammable objects. Uses the public variables for the parameters.
        /// </summary>
        public void CastRayCastNullify()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            Vector3 castPoint = transform.position + startOffset;
            RaycastHit[] hits = Physics.SphereCastAll(castPoint, raycastRadius, direction, maxDist, mask);
            if (hits.Length > 0)
            {
                Magio.MagioObjectEffect hitMagioObj = null;
                Vector3 hitPoint = Vector3.zero;
                List<RaycastHit> otherHitObjects = new List<RaycastHit>();
                foreach (RaycastHit hit in hits)
                {
                    Magio.MagioObjectMaster magioTarget = hit.collider.GetComponentInParent<Magio.MagioObjectMaster>();
                    if (magioTarget)
                    {
                        foreach (MagioObjectEffect magioObj in magioTarget.magioObjects)
                        {
                            if (magioObj && magioObj.effectClass == affectedClass)
                            {
                                if (goThroughObjects)
                                {
                                    magioObj.IncrementalNullify(hit.point, raycastRadius, radiusIncrement);
                                }
                                else
                                {
                                    hitPoint = hit.point;
                                    hitMagioObj = magioObj;
                                }

                            }
                            else
                            {
                                otherHitObjects.Add(hit);
                            }
                        }
                    }
                }

                if (hitMagioObj != null && !goThroughObjects)
                {
                    float dist = Vector3.Distance(castPoint, hitPoint);

                    bool objectsInFront = false;
                    foreach (RaycastHit hit in otherHitObjects)
                    {
                        if (!hit.collider.isTrigger)
                        {
                            if (Vector3.Distance(hit.point, castPoint) < dist)
                            {
                                objectsInFront = true;
                                break;
                            }
                        }
                    }

                    if (!objectsInFront)
                    {
                        hitMagioObj.IncrementalNullify(hitPoint, raycastRadius, radiusIncrement);
                    }
                }

            }
        }

        private void OnDrawGizmos()
        {
            RaycastHit hit;

            Gizmos.color = Color.blue;
            if (Physics.SphereCast(transform.position, raycastRadius, direction, out hit, maxDist, mask))
            {
                Gizmos.DrawLine(transform.position, hit.point);
                Gizmos.DrawWireSphere(hit.point, raycastRadius);
            }
            else
            {
                Gizmos.DrawRay(transform.position, direction);
            }
        }
    }
}
