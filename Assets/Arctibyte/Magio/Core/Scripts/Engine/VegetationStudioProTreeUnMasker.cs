using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Unmasks VSPro instance if necessary (not ignited).
    /// </summary>
    public class VegetationStudioProTreeUnMasker : MonoBehaviour
    {
        public GameObject mask;
        public MagioObjectEffect magioObj;
        public string VegetationInstanceItemId;
        private float extraTimer = 5;

        void Update()
        {
            if(!magioObj.effectEnabled && magioObj.GetCurrentIgnitionProgress() <= 0)
            {
                extraTimer -= Time.deltaTime;

                if(extraTimer < 0)
                {
                    Unmask();
                }
                
            } 
            else
            {
                extraTimer = 5;
            }
        }


        /// <summary>
        /// Unmask saved instance.
        /// </summary>
        public void Unmask()
        {
            Destroy(mask);
            Destroy(magioObj.gameObject);
        }
    }
}

