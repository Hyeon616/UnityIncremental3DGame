using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Nullifies the effect according to other effect (or own) spread.
    /// </summary>
    public class SpreadNullify : MonoBehaviour
    {
        [Tooltip("How much does the nullify come after the spread?")]
        public float nullifyBehindLag_m = 0.3f;

        [Tooltip("Nullify according to this spread. If left empty -  Use own spread AKA self-nullify")]
        public MagioObjectEffect spreader;

        MagioObjectEffect magioObj;

        void Start()
        {
            magioObj = GetComponent<MagioObjectEffect>();

            if(!spreader)
                spreader = GetComponent<MagioObjectEffect>();
        }

        // Update is called once per frame
        void Update()
        {
            if (spreader && spreader.effectEnabled)
            {
                magioObj.SetNullifyArea(spreader.GetEffectOrigin(), Mathf.Max(spreader.effectSpread - nullifyBehindLag_m, 0));
            }
        }
    }
}

