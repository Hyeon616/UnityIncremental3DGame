#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    [ExecuteInEditMode]
    public class PreviewMagioShaderController : MonoBehaviour
    {
        [Range(0, 1)]
        public float effectSpreadStatus = 0.5f;

        [HideInInspector]
        public PreviewMagioShader previewMaster;

        public void SetStatusFromSpread(float spread)
        {
            effectSpreadStatus = spread / (previewMaster.magioObj.effectCrawlSpeed * (previewMaster.magioObj.fadeOutStart_s + previewMaster.magioObj.fadeOutLength_s));
        }

        private void Update()
        {
            if (!Application.isPlaying && previewMaster && previewMaster.magioObj)
            {
                if (previewMaster.animateEffectSpreadStatus)
                {
                    effectSpreadStatus += 0.001f;
                }

                previewMaster.effectEnabledTimer = effectSpreadStatus * (previewMaster.magioObj.fadeOutStart_s + previewMaster.magioObj.fadeOutLength_s);
                previewMaster.effectSpread = previewMaster.magioObj.effectCrawlSpeed * previewMaster.effectEnabledTimer;
                
                if (previewMaster.effectSpread <= 0.01f)
                {
                    if (previewMaster.magioObjCopy != null)
                    {
                        previewMaster.Clean();
                    }
                }
                else
                {
                    if (previewMaster.magioObjCopy == null)
                    {
                        previewMaster.SetupDebugMaterials();
                    }
                }

                previewMaster.UpdateShaders();
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                enabled = false;
            }
        }
    }
}
#endif
