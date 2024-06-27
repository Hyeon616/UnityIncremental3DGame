using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Handles the saving of original material values (in case of 3rd-party shaders)
    /// </summary>
    public class MaterialPropertySaver : MonoBehaviour
    {
        public struct MatProps
        {
            public bool isMagioShader;
            public Renderer renderer;
            public string originalMainColorName;
            public Color originalMainColor;
            public string originalEmissionColorName;
            public Color originalEmissionColor;
            public Dictionary<string, float> originalNameFloatPairs;
        }


        private Dictionary<Material, MatProps> originalMaterialValues = new Dictionary<Material, MatProps>();

        public Dictionary<Material, MatProps> OriginalMaterialValues { get => originalMaterialValues; }

        // Start is called before the first frame update
        void Start()
        {
            SaveOriginalMaterialShaderProperties();
        }

        /// <summary>
        /// Saves the original shader/material properties which are to be animated by Ignis.
        /// </summary>
        public void SaveOriginalMaterialShaderProperties()
        {
            originalMaterialValues.Clear();
            Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                foreach (Renderer rend in rends)
                {
                    for (int i = 0; i < rend.materials.Length; i++)
                    {
                        Material mat = rend.materials[i];
                        OAVAShaderCompatibilitySO comp = MagioEngine.instance.GetCompatibleShaders().FirstOrDefault(o => o != null && (mat.HasProperty(o.ShaderCheckProperty) || (o.ShaderCheckProperty.Equals(string.Empty) && mat.shader.name.Equals(o.ShaderName))));
                        if (comp)
                        {
                            MatProps origMatProps = new MatProps
                            {
                                renderer = rend,
                                originalNameFloatPairs = new Dictionary<string, float>()
                            };

                            if (mat.HasProperty(comp.ShaderMainColorPropertyName))
                            {
                                origMatProps.originalMainColor = mat.GetColor(comp.ShaderMainColorPropertyName);
                                origMatProps.originalMainColorName = comp.ShaderMainColorPropertyName;
                            }

                            if (mat.HasProperty(comp.ShaderEmissionColorPropertyName))
                            {
                                origMatProps.originalEmissionColor = mat.GetColor(comp.ShaderEmissionColorPropertyName);
                                origMatProps.originalEmissionColorName = comp.ShaderEmissionColorPropertyName;
                            }

                            foreach (OAVAShaderCompatibilitySO.ShaderProperty shprop in comp.duringEffectChangeProperties)
                            {
                                if (mat.HasProperty(shprop.name))
                                    origMatProps.originalNameFloatPairs.Add(shprop.name, mat.GetFloat(shprop.name));
                            }

                            foreach (OAVAShaderCompatibilitySO.ShaderProperty shprop in comp.onBurnoutChangeProperties)
                            {
                                if (mat.HasProperty(shprop.name) && !origMatProps.originalNameFloatPairs.ContainsKey(shprop.name))
                                {
                                    origMatProps.originalNameFloatPairs.Add(shprop.name, mat.GetFloat(shprop.name));
                                }

                            }
                            originalMaterialValues.Add(mat, origMatProps);
                        }
                        else if (mat.HasProperty("_IsMagioShader"))
                        {
                            // Just save reference to the material. We only need to set couple of values to 0 to reset Ignis shader.
                            originalMaterialValues.Add(mat, new MatProps() { renderer = rend });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the original shader/material properties which are to be animated by Ignis.
        /// </summary>
        public void SaveSharedOriginalMaterialShaderProperties()
        {
            originalMaterialValues.Clear();
            Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                foreach (Renderer rend in rends)
                {
                    for (int i = 0; i < rend.sharedMaterials.Length; i++)
                    {
                        Material mat = rend.sharedMaterials[i];
                        OAVAShaderCompatibilitySO comp = MagioEngine.instance.GetCompatibleShaders().FirstOrDefault(o => o != null && (mat.HasProperty(o.ShaderCheckProperty) || (o.ShaderCheckProperty.Equals(string.Empty) && mat.shader.name.Equals(o.ShaderName))));
                        if (comp)
                        {
                            MatProps origMatProps = new MatProps
                            {
                                renderer = rend,
                                originalNameFloatPairs = new Dictionary<string, float>()
                            };

                            if (mat.HasProperty(comp.ShaderMainColorPropertyName))
                            {
                                origMatProps.originalMainColor = mat.GetColor(comp.ShaderMainColorPropertyName);
                                origMatProps.originalMainColorName = comp.ShaderMainColorPropertyName;
                            }

                            if (mat.HasProperty(comp.ShaderEmissionColorPropertyName))
                            {
                                origMatProps.originalEmissionColor = mat.GetColor(comp.ShaderEmissionColorPropertyName);
                                origMatProps.originalEmissionColorName = comp.ShaderEmissionColorPropertyName;
                            }

                            foreach (OAVAShaderCompatibilitySO.ShaderProperty shprop in comp.duringEffectChangeProperties)
                            {
                                if (mat.HasProperty(shprop.name))
                                    origMatProps.originalNameFloatPairs.Add(shprop.name, mat.GetFloat(shprop.name));
                            }

                            foreach (OAVAShaderCompatibilitySO.ShaderProperty shprop in comp.onBurnoutChangeProperties)
                            {
                                if (mat.HasProperty(shprop.name) && !origMatProps.originalNameFloatPairs.ContainsKey(shprop.name))
                                {
                                    origMatProps.originalNameFloatPairs.Add(shprop.name, mat.GetFloat(shprop.name));
                                }

                            }
                            originalMaterialValues.Add(mat, origMatProps);
                        }
                        else if (mat.HasProperty("_IsMagioShader"))
                        {
                            // Just save reference to the material. We only need to set couple of values to 0 to reset Ignis shader.
                            originalMaterialValues.Add(mat, new MatProps() { renderer = rend });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the material values which were animated by Ignis to the originals. Does not reset other material shader properties.
        /// </summary>
        public void ResetMaterialFromMagio()
        {
            if (MagioEngine.instance)
            {
                foreach (Material mat in originalMaterialValues.Keys)
                {
                    MatProps props = originalMaterialValues[mat];

                    if (mat.HasProperty("_IsMagioShader"))
                    {
                        mat.SetFloat("_EffectSpread", 0);
                        mat.SetFloat("_DissolveSpread", 0);
                        mat.SetFloat("_TextureOverlaySpread", 0);
                        mat.SetFloat("_OverlayBrightness", 0);
                        mat.SetVector("_EffectOrigin", new Vector3());
                        mat.SetVector("_TextureOverlayOrigin", new Vector3());
                        mat.SetVector("_DissolveOrigin", new Vector3());
                        mat.SetVector("_NullifyOrigin", new Vector3());
                        mat.SetVector("_NullifyTextureOverlayOrigin", new Vector3());
                        mat.SetFloat("_NullifySpread", 0);
                        mat.SetFloat("_NullifyTextureOverlaySpread", 0);
                        mat.SetFloat("_BlendTextureLerpT", 0);
                        mat.SetFloat("LifelineMultiplier", 1);
                    }
                    else
                    {
                        if (mat.HasProperty(props.originalMainColorName))
                        {
                            mat.SetColor(props.originalMainColorName, props.originalMainColor);
                        }

                        if (mat.HasProperty(props.originalEmissionColorName))
                        {
                            mat.SetColor(props.originalEmissionColorName, props.originalEmissionColor);
                            DynamicGI.SetEmissive(props.renderer, mat.GetColor(props.originalEmissionColorName));
                        }

                        foreach (string floatPropName in props.originalNameFloatPairs.Keys)
                        {
                            mat.SetFloat(floatPropName, props.originalNameFloatPairs[floatPropName]);
                        }
                    }
                }
            }

        }
    }

}
