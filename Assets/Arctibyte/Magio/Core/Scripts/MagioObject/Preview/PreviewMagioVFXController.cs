#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    [ExecuteInEditMode]
    public class PreviewMagioVFXController : MonoBehaviour
    {
        [Range(0, 10000)]
        public float effectSpread = 100000;

        [HideInInspector]
        public PreviewMagioVFX previewMaster;

        private void Update()
        {
            if(previewMaster)
                previewMaster.effectSpread = effectSpread;
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

