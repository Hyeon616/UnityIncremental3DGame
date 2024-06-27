using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Compatibilities are saved here to optimize the performance.
    /// You can also override these compatibilities to make different animations for different objects (or not to animate some).
    /// </summary>
    public class MagioMaterialAnimationSettings : MonoBehaviour
    {
        [System.Serializable]
        public struct CompatibilityOverride
        {
            public int materialIndex;
            public OAVAShaderCompatibilitySO shaderComp;
        }

        public List<CompatibilityOverride> compatibilitySettingOverrides = new List<CompatibilityOverride>();

        private List<OAVAShaderCompatibilitySO> compatibilities = new List<OAVAShaderCompatibilitySO>();

        /// <summary>
        /// Updates the compatibility list
        /// </summary>
        /// <returns>List of current compatibilities</returns>
        public List<OAVAShaderCompatibilitySO> GetNewCompatibilitiesList()
        {
            List<OAVAShaderCompatibilitySO> ret = new List<OAVAShaderCompatibilitySO>();
            Renderer rend = GetComponent<Renderer>();
            if (rend)
            {
                for (int i = 0; i < rend.sharedMaterials.Length; i++)
                {
                    bool found = false;
                    foreach (CompatibilityOverride over in compatibilitySettingOverrides)
                    {
                        if (over.materialIndex == i)
                        {
                            ret.Add(over.shaderComp);
                            found = true;
                            break;
                        }
                    }

                    // If not found try to find comp from the engine defaults
                    if (!found)
                    {
                        bool found2 = false;
                        List<OAVAShaderCompatibilitySO> compShaders = MagioEngine.instance.GetCompatibleShaders();
                        Material mat = rend.sharedMaterials[i];
                        foreach (OAVAShaderCompatibilitySO shaderCompiterator in compShaders)
                        {
                            if (shaderCompiterator && (mat.HasProperty(shaderCompiterator.ShaderCheckProperty) ||
                                (shaderCompiterator.ShaderCheckProperty.Equals(string.Empty) && mat.shader.name.Equals(shaderCompiterator.ShaderName))))
                            {
                                ret.Add(shaderCompiterator);
                                found2 = true;
                                break;
                            }
                        }

                        if (!found2)
                        {
                            ret.Add(null);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("No renderer on MagioMaterialAnimationSettings");
            }

            return ret;

        }

        /// <summary>
        /// Gets the existing (or lazy initializes in case of first) list of compatibilites
        /// </summary>
        /// <returns>List of comps for current materials</returns>
        public List<OAVAShaderCompatibilitySO> GetCompatibilities()
        {
            // Init list lazy
            if (compatibilities.Count <= 0)
            {
                compatibilities = GetNewCompatibilitiesList();
            }

            return compatibilities;
        }
    }

}
