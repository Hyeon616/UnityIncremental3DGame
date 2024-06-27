using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Represents one effect pack.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMagioEffectPack", menuName = "Magio/Effect Pack", order = 1)]
    public class MagioEffectPack : ScriptableObject
    {
        [Tooltip("Effect prefabs that support mesh. Must have visual effect component")]
        public List<GameObject> meshEffects = new List<GameObject>();

        [Tooltip("Effect prefabs that support skinned mesh. Must have visual effect component")]
        public List<GameObject> skinnedMeshEffects = new List<GameObject>();
    }
}

