using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{

    /// <summary>
    /// Example how you can inherit mesh replacer and use it with your own instancer.
    /// </summary>
    public class CustomMagioMeshPrefabReplacerExample : MagioMeshPrefabReplacer
    {
        protected override void Start()
        {
            base.Start();

            // Do your own start things.
        }

        public override GameObject TryToSwitchToMagioPrefab(EffectClass effectClass)
        {
            GameObject replaced = base.TryToSwitchToMagioPrefab(effectClass);

            // Do something if replaced != null (Mask an instance etc.)

            return replaced;
        }
    }
}

