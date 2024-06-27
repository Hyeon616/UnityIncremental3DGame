using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Adds ability to Ignite the effect using raycast.
    /// </summary>
    public class RaycastIgnite : MonoBehaviour
    {
        [Tooltip("Which direction to check")]
        public Vector3 direction = new Vector3(0, -1, 0);

        [Tooltip("Offset of ray starting point")]
        public Vector3 startOffset = new Vector3(0, 0, 0);

        [Tooltip("How many times in second to check")]
        public float checkFrequency = 10;

        [Tooltip("How large is the raycast area")]
        public float raycastRadius = 0.1f;

        [Tooltip("How much the particle will ignite the object on one collision. (If object has ignite time). 1 = ignite time in seconds by object props")]
        public float IgnitePowerMultiplier = 5f;

        [Tooltip("Max distance from the cast pos")]
        public float maxDist = 3f;

        [Tooltip("Raycast layermask")]
        public LayerMask mask = ~0;

        [Tooltip("This class will be affected by this raycast. For multiple classes add multiple scripts.")]
        public EffectClass affectedClass = EffectClass.Default;

        [Tooltip("Start invoking the repeating raycast on start ")]
        public bool repeatingRaycast = true;

        [Tooltip("None: not used. Effect: If the object that is hit by this cast does not have a MagioObject of affected class defined add this splash effect to object. E.g. magic splashed.")]
        public GameObject splashEffectPrefab;

        private bool splashAdded = false;

        // Start is called before the first frame update
        void Start()
        {
            if(repeatingRaycast)
                InvokeRepeating(nameof(CastRayCastIgnite), 1 / checkFrequency, 1 / checkFrequency);
        }

        private void OnEnable()
        {
            splashAdded = false;
        }

        /// <summary>
        /// Casts a raycast sphere once looking for ignite the flammable objects. Uses the public variables for the parameters.
        /// </summary>
        public void CastRayCastIgnite()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            Vector3 castPoint = transform.position + startOffset;
            RaycastHit[] hits = Physics.SphereCastAll(castPoint, raycastRadius, direction, maxDist, mask);
            bool splash = false;

            if (hits.Length > 0)
            {
                Magio.MagioObjectEffect hitMagioObj = null;
                Vector3 hitPoint = Vector3.zero;
                List<RaycastHit> otherHitObjects = new List<RaycastHit>();
                foreach (RaycastHit hit in hits)
                {
                    bool foundEffect = false;
                    Magio.MagioObjectMaster magioTarget = hit.collider.GetComponentInParent<Magio.MagioObjectMaster>();

                    if (MagioEngine.instance.enableMeshPrefabReplacers)
                    {
                        MagioMeshPrefabReplacer replacer = hit.collider.gameObject.GetComponentInParent<MagioMeshPrefabReplacer>();

                        if (replacer)
                        {
                            GameObject replaced = replacer.TryToSwitchToMagioPrefab(affectedClass);

                            if (replaced)
                            {
                                magioTarget = replaced.gameObject.GetComponentInChildren<MagioObjectMaster>();


                                if (!magioTarget)
                                {
                                    replaced.gameObject.GetComponentInChildren<MagioObjectEffect>().Setup();
                                    magioTarget = replaced.gameObject.GetComponentInChildren<MagioObjectMaster>();
                                }
                                if (magioTarget)
                                {
                                    magioTarget.Setup();
                                }
                            }
                        }
                    }


                    if (magioTarget)
                    {
                        foreach (MagioObjectEffect magioObj in magioTarget.magioObjects)
                        {
                            if (magioObj && magioObj.effectClass == affectedClass && !magioObj.isSplashEffect)
                            {
                                hitPoint = hit.point;
                                hitMagioObj = magioObj;
                                foundEffect = true;
                            }
                            else
                            {
                                otherHitObjects.Add(hit);
                            }
                        }
                    }

                    if (splashEffectPrefab && !foundEffect && !splashAdded)
                    {
                        GameObject obj = Instantiate(splashEffectPrefab, MagioEngine.instance.splashEffectParent);
                        MagioObjectEffect eff = obj.GetComponent<MagioObjectEffect>();
                        eff.targetGameObject = hit.collider.gameObject;
                        eff.useOnThisGameObject = false;
                        eff.Setup();

                        eff.TryToAnimateEffect(hit.collider.ClosestPointOnBounds(transform.position), IgnitePowerMultiplier);

                        splash = true;
                    }
                }

                if (splash) splashAdded = true;

                if(hitMagioObj != null)
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
                        hitMagioObj.TryToAnimateEffect(hitPoint, IgnitePowerMultiplier * (1/checkFrequency));
                    }
                }

            }

            

            if (MagioEngine.instance.VegetationStudioProCompatible)
            {
                // Check for collisions in a "traditional way" since VS has colliders
                RaycastHit hit;

                if (Physics.SphereCast(castPoint, raycastRadius, direction, out hit, maxDist, mask))
                {
                    MagioEngine.instance.MaskInstanceAndSpawnAPrefabIfNecessary(hit.collider.gameObject, affectedClass);
                }
            }


            if (MagioEngine.instance.unityTerrainCompatible)
            {
                // Check for collisions in a "traditional way" since terrain has colliders
                RaycastHit hit;

                if (Physics.SphereCast(castPoint, raycastRadius, direction, out hit, maxDist, mask))
                {
                    MagioEngine.instance.MaskUnityTerrainTreeAndInstanceAPrefabIfNecessary(hit.collider.gameObject, hit.point, affectedClass, raycastRadius);
                }
                
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (Physics.SphereCast(transform.position, raycastRadius, direction, out RaycastHit hit, maxDist, mask))
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
