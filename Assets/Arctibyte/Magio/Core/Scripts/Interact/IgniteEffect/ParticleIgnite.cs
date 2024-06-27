using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Adds ability to Ignite the effect using particles.
    /// </summary>
    public class ParticleIgnite : MonoBehaviour
    {
        [Tooltip("How much the particle will ignite the object on one collision. (If object has ignite time). 1 = 1 second addition to Ignite time on one touch")]
        public float IgnitePowerMultiplier = 5f;

        [Tooltip("This class will be affected by this particle. For multiple classes add multiple scripts.")]
        public EffectClass affectedClass = EffectClass.Default;

        [Tooltip("None: not used. Effect: If the object that is hit by this cast does not have a MagioObject of affected class defined add this splash effect to object. E.g. magic splashed.")]
        public GameObject splashEffectPrefab;

        private ParticleSystem part;
        private List<ParticleCollisionEvent> collisionEvents;
        private HashSet<GameObject> collidedAlready = new HashSet<GameObject>();

        void Start()
        {
            part = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
        }

        void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
            bool foundEffect = false;
            Magio.MagioObjectMaster magioTarget = other.GetComponentInParent<Magio.MagioObjectMaster>();

            if (MagioEngine.instance.enableMeshPrefabReplacers)
            {
                MagioMeshPrefabReplacer replacer = other.gameObject.GetComponentInParent<MagioMeshPrefabReplacer>();

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
                    int i = 0;
                    if (magioObj && magioObj.effectClass == affectedClass && !magioObj.isSplashEffect)
                    {
                        while (i < numCollisionEvents)
                        {
                            Vector3 pos = collisionEvents[i].intersection;
                            magioObj.TryToAnimateEffect(pos, IgnitePowerMultiplier);
                            i++;
                            foundEffect = true;
                        }
                    }
                }
            }

            if (splashEffectPrefab && !foundEffect)
            {
                GameObject obj = Instantiate(splashEffectPrefab, MagioEngine.instance.splashEffectParent);
                MagioObjectEffect eff = obj.GetComponent<MagioObjectEffect>();
                eff.targetGameObject = other;
                eff.useOnThisGameObject = false;
                eff.Setup();
                Vector3 pos = collisionEvents[0].intersection;

                eff.TryToAnimateEffect(pos, IgnitePowerMultiplier);
            }

            if (MagioEngine.instance.VegetationStudioProCompatible)
            {
                if(!collidedAlready.Contains(other))
                {
                    if(!other.transform.parent || other.transform.parent && !collidedAlready.Contains(other.transform.parent.gameObject))
                    {
                        GameObject maskedObj = MagioEngine.instance.MaskInstanceAndSpawnAPrefabIfNecessary(other, affectedClass);
                        if (maskedObj)
                        {
                            collidedAlready.Add(maskedObj);
                            StartCoroutine(RemoveFromCollidedAlready(maskedObj));
                        }
                    }
                }
            }

            if (MagioEngine.instance.unityTerrainCompatible)
            {
                MagioEngine.instance.MaskUnityTerrainTreeAndInstanceAPrefabIfNecessary(other, collisionEvents[0].intersection, affectedClass, 1f);
            }
        }

        /// <summary>
        /// Because VS PRO shuffles the colliders (I guess for effiency) we need to blacklist a masked collider for a while to not randomly ignite other vegetation far away
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        IEnumerator RemoveFromCollidedAlready(GameObject go)
        {
            yield return new WaitForSeconds(0.5f);
            collidedAlready.Remove(go);
        }
    }
}
