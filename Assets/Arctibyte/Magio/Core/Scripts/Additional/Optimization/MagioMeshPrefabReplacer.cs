using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Replaces this gameobject with variant specified in magioVariantPrefab if the effect is being Ignited.
    /// You can use this to optimize if you have lots of 'dormant' magio gameobject.
    /// 
    /// E.g. Have 'Normal' static lit gameobject, attach this script to it, make other prefab of this same gameobject with it's own materials, Convert it to Magio and attach it to magioVariantPrefab of this script.
    /// This way you avoid all overhead from Magio before object effect is ignited.
    /// 
    /// Tip: you can possibly use this with any instance renderer if you attach this script to colliders with the right prefab and mask the instance if the prefab is switched.
    /// </summary>
    public class MagioMeshPrefabReplacer: MonoBehaviour
    {
        public GameObject magioVariantPrefab;
        private HashSet<EffectClass> effectClasses = new HashSet<EffectClass>();

        protected virtual void Start()
        {
            if (!magioVariantPrefab)
            {
                Debug.LogWarning("No magioVariantPrefab assigned in MeshPrefabReplacer. Destroying component in game object: " + gameObject.name);
                Destroy(this);
                return;
            }

            MagioObjectMaster master = magioVariantPrefab.GetComponentInChildren<MagioObjectMaster>();

            if(master)
            {
                foreach(MagioObjectEffect eff in master.effectParent.GetComponentsInChildren<MagioObjectEffect>(true))
                {
                    effectClasses.Add(eff.effectClass);
                }

                return;
            }

            MagioObjectEffect simpleEffect = magioVariantPrefab.GetComponentInChildren<MagioObjectEffect>();

            if (simpleEffect)
            {
                effectClasses.Add(simpleEffect.effectClass);
                return;
            }

            Debug.LogWarning("No Magio Object Effects in game object which should be replaced, destroying component in game object: " + gameObject.name);
            Destroy(this);
        }

        /// <summary>
        /// Switches the game object to magioprefab if magioprefab has effect class which is trying to ignite the effect
        /// </summary>
        /// <param name="effectClass">Effect class which should be ignited</param>
        /// <returns>True if the prefab was replaced, false if not.</returns>
        public virtual GameObject TryToSwitchToMagioPrefab(EffectClass effectClass)
        {
            if(effectClasses.Contains(effectClass))
            {
                GameObject magioPrefab = Instantiate(magioVariantPrefab, this.transform.parent);

                magioPrefab.transform.localPosition = transform.localPosition;
                magioPrefab.transform.localRotation = transform.localRotation;
                magioPrefab.transform.localScale = transform.localScale;
                magioPrefab.name = this.gameObject.name;

                Destroy(this.gameObject);

                return magioPrefab;
            }

            return null;
        }
    }
}

