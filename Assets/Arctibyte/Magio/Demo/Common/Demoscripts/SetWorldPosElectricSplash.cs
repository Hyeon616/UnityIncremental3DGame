using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    public class SetWorldPosElectricSplash : MonoBehaviour
    {

        public MagioObjectEffect eff;

        // Update is called once per frame
        void Update()
        {
            MagioObjectEffect.VFXProps prop = eff.GetVFXPropertyValue("SourcePositionWorld");

            prop.vector3Value = eff.GetEffectOrigin();

            eff.SetVFXPropertyValue(prop);
        }
    }
}

