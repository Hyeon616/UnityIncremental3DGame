using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Master for all the effect on one object. Handles interaction and turns.
    /// </summary>
    public class MagioObjectMaster : MonoBehaviour
    {
        private float approxSize = 5;

        public float ApproxSize { get => approxSize; }
        public Vector3 ApproxCenter { get => approxCenter; }

        private Vector3 approxCenter;

        public List<MagioObjectEffect> magioObjects = new List<MagioObjectEffect>();

        public GameObject effectParent;

        private MagioObjectEffect currentMaterialAnimator;
        private MagioObjectEffect currentTextureOverlayer;
        private MagioObjectEffect currentDissolver;
        private MagioObjectEffect currentEmissionOverlayer;
        private bool firstSetupDone = false;

        private void Start()
        {
            if (!firstSetupDone)
            {
                Setup();
            }
        }

        public void Setup()
        {
            CalculateApproxSize();

            if (effectParent)
            {
                // Add inactive effects to the list
                foreach (MagioObjectEffect eff in effectParent.GetComponentsInChildren<MagioObjectEffect>(true))
                {
                    if (eff.targetGameObject == gameObject && !magioObjects.Contains(eff))
                    {
                        magioObjects.Add(eff);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates approx size of object from the renderer bounds.
        /// </summary>
        public void CalculateApproxSize()
        {
            List<Renderer> rends = GetComponentsInChildren<Renderer>().ToList();
            if (rends.Count <= 0)
            {
                approxSize = 1;
                return;
            }

            Bounds combinedRendBounds = rends[0].bounds;

            foreach (Renderer rend in rends)
            {
                if (rend != rends[0]) combinedRendBounds.Encapsulate(rend.bounds);
            }

            approxSize = combinedRendBounds.size.magnitude;
            approxCenter = combinedRendBounds.center;
        }

        /// <summary>
        /// Returns true if other effects are not using materialAnimation
        /// </summary>
        /// <param name="magioObj"></param>
        /// <returns></returns>
        public bool CanIUseMaterialAnimation(MagioObjectEffect magioObj, bool isMagioShader)
        {
            if (isMagioShader)
            {
                switch (magioObj.magioShaderEffectMode)
                {
                    case MagioObjectEffect.MagioShaderEffectMode.Dissolve:
                        if (!currentDissolver)
                            currentDissolver = magioObj;

                        if (currentDissolver == magioObj)
                            return true;

                        return false;
                    case MagioObjectEffect.MagioShaderEffectMode.Emission_Overlay:
                        if (!currentEmissionOverlayer || !currentEmissionOverlayer.effectEnabled)
                            currentEmissionOverlayer = magioObj;

                        if (currentEmissionOverlayer == magioObj)
                            return true;

                        return false;
                        
                    case MagioObjectEffect.MagioShaderEffectMode.Texture_Override:
                        if (!currentTextureOverlayer || !currentTextureOverlayer.effectEnabled)
                            currentTextureOverlayer = magioObj;

                        if (currentTextureOverlayer == magioObj)
                            return true;

                        return false;
                
                }
                return false;
            }
            else
            {
                if (!currentMaterialAnimator || !currentMaterialAnimator.effectEnabled)
                {
                    currentMaterialAnimator = magioObj;
                }

                if (currentMaterialAnimator == magioObj)
                {
                    return true;
                }

                return false;
            }
        }


        public bool IsEmissionOverlayerEffectEnabled()
        {
            if(currentEmissionOverlayer && currentEmissionOverlayer.effectEnabled)
            {
                return true;
            }

            return false;
        }

        public bool CanEffectEnable(MagioObjectEffect magioObj)
        {
            foreach (MagioObjectEffect effect in magioObjects)
            {
                if (effect == magioObj || !effect.effectEnabled)
                {
                    continue;
                }

                if (effect.effectBehaviourType == EffectBehaviourMode.Spread)
                {
                    
                    float nullifyRuleLagBehind = MagioEngine.instance.GetNullifyRuleLagBehind(effect.effectClass, magioObj.effectClass);
                    if (nullifyRuleLagBehind > -0.01)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void HandleNewEffectEnable(MagioObjectEffect magioObj)
        {
            foreach(MagioObjectEffect effect in magioObjects)
            {
                if(effect == magioObj || !effect.effectEnabled)
                {
                    continue;
                }

                if (effect.effectBehaviourType == EffectBehaviourMode.Spread)
                {
                    float nullifyRuleLagBehind = MagioEngine.instance.GetNullifyRuleLagBehind(magioObj.effectClass, effect.effectClass);
                    if (nullifyRuleLagBehind > -0.01)
                    {
                        if (!effect.gameObject.GetComponent<SpreadNullify>())
                        {
                            SpreadNullify nullify = effect.gameObject.AddComponent<SpreadNullify>();
                            nullify.nullifyBehindLag_m = nullifyRuleLagBehind;
                            nullify.spreader = magioObj;
                        }
                    }
                }
            }
        }

        public void AddMagioObject(MagioObjectEffect magioObj)
        {
            if(!magioObjects.Contains(magioObj))
                magioObjects.Add(magioObj);
        }

        public void RemoveMagioObject(MagioObjectEffect magioObj)
        {
            if (magioObjects.Contains(magioObj))
                magioObjects.Remove(magioObj);
        }
    }
}

