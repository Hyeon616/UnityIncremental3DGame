using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Adds ability to Ignite the effect using Spherecast.
    /// </summary>
    public class SphereIgnite : MonoBehaviour
    {

        [Tooltip("How many times in second to check")]
        public float checkFrequency = 10;

        [Tooltip("How large is the raycast area")]
        public float raycastRadius = 1f;

        [Tooltip("How much the particle will ignite the object on one collision. (If object has ignite time). 1 = ignite time in seconds by object props")]
        public float IgnitePowerMultiplier = 5f;

        [Tooltip("Raycast layermask")]
        public LayerMask mask = ~0;

        [Tooltip("This class will be affected by this sphere. For multiple classes add multiple scripts.")]
        public EffectClass affectedClass = EffectClass.Default;

        [Tooltip("Start invoking the repeating raycast on start ")]
        public bool repeatingRaycast = true;

        [Tooltip("None: not used. Effect: If the object that is hit by this cast does not have a MagioObject of affected class defined add this splash effect to object. E.g. magic splashed.")]
        public GameObject splashEffectPrefab;

        private bool splashAdded = false;

        // Start is called before the first frame update
        void Start()
        {
            if (repeatingRaycast)
                InvokeRepeating(nameof(SphereIgniteCast), 1 / checkFrequency, 1 / checkFrequency);
        }

        private void OnEnable()
        {
            splashAdded = false;
        }

        /// <summary>
        /// Casts a raycast sphere once looking for ignite the flammable objects. Uses the public variables for the parameters.
        /// </summary>
        public void SphereIgniteCast()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;

            Collider[] hits = Physics.OverlapSphere(transform.position, raycastRadius, mask);
            if (hits.Length > 0)
            {
                bool splash = false;

                foreach(Collider hit in hits)
                {
                    Magio.MagioObjectMaster magioTarget = hit.gameObject.GetComponentInParent<Magio.MagioObjectMaster>();

                    if (MagioEngine.instance.enableMeshPrefabReplacers)
                    {
                        MagioMeshPrefabReplacer replacer = hit.gameObject.GetComponentInParent<MagioMeshPrefabReplacer>();

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

                    bool foundEffect = false;
                    if (magioTarget)
                    {
                        foreach(MagioObjectEffect magioObj in magioTarget.magioObjects)
                        {
                            if(magioObj && magioObj.effectClass == affectedClass && !magioObj.isSplashEffect)
                            {
                                magioObj.TryToAnimateEffect(hit.ClosestPointOnBounds(transform.position), IgnitePowerMultiplier);
                                foundEffect = true;
                            }
                                
                        }
                    }

                    if (splashEffectPrefab && !foundEffect && !splashAdded)
                    {
                        GameObject obj = Instantiate(splashEffectPrefab, MagioEngine.instance.splashEffectParent);
                        MagioObjectEffect eff = obj.GetComponent<MagioObjectEffect>();
                        eff.targetGameObject = hit.gameObject;
                        eff.useOnThisGameObject = false;
                        eff.Setup();

                        eff.TryToAnimateEffect(hit.ClosestPointOnBounds(transform.position), IgnitePowerMultiplier);

                        splash = true;
                    }

                    if (MagioEngine.instance.VegetationStudioProCompatible)
                    {
                        MagioEngine.instance.MaskInstanceAndSpawnAPrefabIfNecessary(hit.gameObject,affectedClass);
                    }

                    if (MagioEngine.instance.unityTerrainCompatible)
                    {
                        MagioEngine.instance.MaskUnityTerrainTreeAndInstanceAPrefabIfNecessary(hit.gameObject, hit.ClosestPointOnBounds(transform.position), affectedClass, raycastRadius);
                    }
                }

                if (splash) splashAdded = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, raycastRadius);
        }
    }
}