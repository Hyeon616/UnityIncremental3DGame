using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Compatibility with 3rd party shader. Compatible with Magio and Ignis
    /// </summary>
    [CreateAssetMenu(fileName = "NewShaderCompatibility", menuName = "Magio/ShaderCompatibility", order = 1)]
    public class OAVAShaderCompatibilitySO : ScriptableObject
    {
        public enum ReflectObjectVectorProperty
        {
            None,
            SpreadCenter,
            EmissionIntensity,
            NullifyCenter
        }

        public enum ReflectObjectFloatProperty
        {
            None,
            SpreadRadius,
            EmissionIntensity,
            NullifyRadius,
            EnabledTimer
        }

        [Header("General")]
        [Tooltip("System will check this property from the shader to confirm it is a right type of shader")]
        public string ShaderCheckProperty = "";

        [Tooltip("If ShaderCheckProperty is left empty, the system can compare this string to check if it is a right shader")]
        public string ShaderName = "";

        [Tooltip("Color to animate while burning")]
        public string ShaderMainColorPropertyName = "_MainColor";

        [Tooltip("Emission Color to animate while burning (if material has this)")]
        public string ShaderEmissionColorPropertyName = "";

        [Tooltip("If you are not animating/using emission intensity, this will animate the emission color intensity in HDR mode.")]
        public bool useEmissionHDRColor = true;

        [System.Serializable]
        public class ShaderProperty
        {
            [Tooltip("Property name")]
            public string name = "";

            [Tooltip("If you want to reflect one of Magio Objects properties to the object use this. If set to None, target values will be used")]
            public ReflectObjectFloatProperty reflectObjectProperty = ReflectObjectFloatProperty.None;

            [Tooltip("Target value of the property. Only used if reflectObjectProperty == None")]
            public float targetValue = 0;

            [Tooltip("How fast it is animated to change. This is updated with Time.deltaTime. Only used if reflectObjectProperty == None")]
            public float speedMultiplier = 1;
        }

        [System.Serializable]
        public class ShaderVectorProperty
        {
            [Tooltip("Property name")]
            public string name = "";

            [Tooltip("If you want to reflect one of Magio Objects properties to the object use this. If set to None, target values will be used")]
            public ReflectObjectVectorProperty reflectObjectProperty = ReflectObjectVectorProperty.None;

            [Tooltip("Target value of the property. Only used if reflectObjectProperty == None")]
            public Vector4 targetValue = Vector4.zero;

            [Tooltip("How fast each value of Vector 4 is animated to change. This is updated with Time.deltaTime. Only used if reflectObjectProperty == None")]
            public Vector4 speedMultiplier = new Vector4(1, 1, 1, 1);

            [Tooltip("If you want the vector to be animated back and forth, set these > 0. Each corresponds to each of the values in Vector4. Only used if reflectObjectProperty == None")]
            public Vector4 perlinNoiseAnimationSpeed = Vector4.zero;

            [Tooltip("If you want the vector to be animated back and forth, set these > 0. Each corresponds to each of the values in Vector4. Only used if reflectObjectProperty == None")]
            public Vector4 perlinNoiseAnimationSize = Vector4.zero;
        }

        [System.Serializable]
        public class ShaderTextureProperty
        {
            [Tooltip("Property name")]
            public string name = "";

            [Tooltip("Target texture")]
            public Texture targetValue;
        }

        [Header("On Start")]
        [Tooltip("List of keywords to enable when the effect starts (e.g. Enable emission)")]
        public List<string> onEffectStartEnableKeywords = new List<string>();

        [Tooltip("List of flags to enable when the effect starts (e.g. Enable emission)")]
        public List<MaterialGlobalIlluminationFlags> onEffectStartEnableIlluminationFlag = new List<MaterialGlobalIlluminationFlags>();

        [Tooltip("List of textures to set when the effect starts (e.g. set emission noise texture)")]
        public List<ShaderTextureProperty> onEffectStartSetTextures = new List<ShaderTextureProperty>();

        [Header("During")]
        [Tooltip("List of properties to animate while burning and their target values")]
        public List<ShaderProperty> duringEffectChangeProperties = new List<ShaderProperty>();

        [Tooltip("List of vector properties to animate while burning and their target values")]
        public List<ShaderVectorProperty> duringEffectChangeVectorProperties = new List<ShaderVectorProperty>();

        [Header("On Fade Out")]
        [Tooltip("List of properties to animate after burnout has started (like stopping the wind affecting)")]
        public List<ShaderProperty> onBurnoutChangeProperties = new List<ShaderProperty>();

        [Tooltip("List of vector properties to animate after burnout has started (like stopping the wind affecting)")]
        public List<ShaderVectorProperty> onBurnoutChangeVectorProperties = new List<ShaderVectorProperty>();

        [Tooltip("List of properties to animate when effect touches it (e.g. snow melting on rocks)")]
        public List<ShaderProperty> onTouchChangeProperties = new List<ShaderProperty>();

        public void OnEffectStart(Material mat, MagioObjectEffect eff)
        {
            foreach (string keyword in onEffectStartEnableKeywords)
            {
                mat.EnableKeyword(keyword);

            }
            foreach (MaterialGlobalIlluminationFlags flag in onEffectStartEnableIlluminationFlag)
            {
                mat.globalIlluminationFlags = flag;
            }

            foreach (ShaderTextureProperty prop in onEffectStartSetTextures)
            {
                if (mat.HasProperty(prop.name))
                {
                    mat.SetTexture(prop.name, prop.targetValue);
                }
            }
        }

        public void SetDuringProperties(Material mat, MagioObjectEffect eff, float currentProgress)
        {
            foreach (ShaderProperty prop in duringEffectChangeProperties)
            {
                if (mat.HasProperty(prop.name))
                {
                    switch (prop.reflectObjectProperty)
                    {
                        case ReflectObjectFloatProperty.None:
                            mat.SetFloat(prop.name, Mathf.MoveTowards(mat.GetFloat(prop.name), prop.targetValue, Time.deltaTime * prop.speedMultiplier));
                            break;
                        case ReflectObjectFloatProperty.EmissionIntensity:
                            mat.SetFloat(prop.name, eff.shaderEmissionMultiplier * currentProgress);
                            break;
                        case ReflectObjectFloatProperty.SpreadRadius:
                            mat.SetFloat(prop.name, eff.effectSpread);
                            break;
                        case ReflectObjectFloatProperty.NullifyRadius:
                            mat.SetFloat(prop.name, eff.GetNullifyRadius());
                            break;
                        case ReflectObjectFloatProperty.EnabledTimer:
                            mat.SetFloat(prop.name, eff.effectEnabledTimer);
                            break;
                    }
                        
                }
            }

            foreach (ShaderVectorProperty prop in duringEffectChangeVectorProperties)
            {
                if (mat.HasProperty(prop.name))
                {
                    switch (prop.reflectObjectProperty)
                    {
                        case ReflectObjectVectorProperty.None:
                            mat.SetVector(prop.name, new Vector4(
                                 Mathf.MoveTowards(mat.GetVector(prop.name).x, prop.targetValue.x * ((1 - currentProgress * prop.perlinNoiseAnimationSize.x) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.x, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.x * 2)), Time.deltaTime * prop.speedMultiplier.x),
                                 Mathf.MoveTowards(mat.GetVector(prop.name).y, prop.targetValue.y * ((1 - currentProgress * prop.perlinNoiseAnimationSize.y) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.y, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.y * 2)), Time.deltaTime * prop.speedMultiplier.y),
                                 Mathf.MoveTowards(mat.GetVector(prop.name).z, prop.targetValue.z * ((1 - currentProgress * prop.perlinNoiseAnimationSize.z) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.z, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.z * 2)), Time.deltaTime * prop.speedMultiplier.z),
                                 Mathf.MoveTowards(mat.GetVector(prop.name).w, prop.targetValue.w * ((1 - currentProgress * prop.perlinNoiseAnimationSize.w) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.w, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.w * 2)), Time.deltaTime * prop.speedMultiplier.w)));
                            break;
                        case ReflectObjectVectorProperty.EmissionIntensity:
                            mat.SetVector(prop.name, new Vector4(eff.shaderEmissionMultiplier * currentProgress, eff.shaderEmissionMultiplier * currentProgress, 1.0f, 0));
                            break;
                        case ReflectObjectVectorProperty.SpreadCenter:
                            mat.SetVector(prop.name, eff.GetEffectOrigin());
                            break;
                        case ReflectObjectVectorProperty.NullifyCenter:
                            mat.SetVector(prop.name, eff.GetNullifyCenter());
                            break;
                    }

                }
            }
        }

        public void SetAfterFadeOutProperties(Material mat, MagioObjectEffect eff, float currentProgress)
        {
            foreach (ShaderProperty prop in onBurnoutChangeProperties)
            {
                if (mat.HasProperty(prop.name))
                {
                    switch (prop.reflectObjectProperty)
                    {
                        case ReflectObjectFloatProperty.None:
                            mat.SetFloat(prop.name, Mathf.MoveTowards(mat.GetFloat(prop.name), prop.targetValue, Time.deltaTime * prop.speedMultiplier));
                            break;
                        case ReflectObjectFloatProperty.EmissionIntensity:
                            mat.SetFloat(prop.name, eff.shaderEmissionMultiplier * currentProgress);
                            break;
                        case ReflectObjectFloatProperty.SpreadRadius:
                            mat.SetFloat(prop.name, eff.effectSpread);
                            break;
                        case ReflectObjectFloatProperty.NullifyRadius:
                            mat.SetFloat(prop.name, eff.GetNullifyRadius());
                            break;
                        case ReflectObjectFloatProperty.EnabledTimer:
                            mat.SetFloat(prop.name, eff.effectEnabledTimer);
                            break;
                    }

                }
            }

            foreach (ShaderVectorProperty prop in onBurnoutChangeVectorProperties)
            {
                if (mat.HasProperty(prop.name))
                {
                    switch (prop.reflectObjectProperty)
                    {
                        case ReflectObjectVectorProperty.None:
                            mat.SetVector(prop.name, new Vector4(
                                 Mathf.MoveTowards(mat.GetVector(prop.name).x, prop.targetValue.x * ((1 - currentProgress * prop.perlinNoiseAnimationSize.x) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.x, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.x * 2)), Time.deltaTime * prop.speedMultiplier.x),
                                 Mathf.MoveTowards(mat.GetVector(prop.name).y, prop.targetValue.y * ((1 - currentProgress * prop.perlinNoiseAnimationSize.y) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.y, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.y * 2)), Time.deltaTime * prop.speedMultiplier.y) ,
                                 Mathf.MoveTowards(mat.GetVector(prop.name).z, prop.targetValue.z * ((1 - currentProgress * prop.perlinNoiseAnimationSize.z) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.z, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.z * 2)), Time.deltaTime * prop.speedMultiplier.z),
                                 Mathf.MoveTowards(mat.GetVector(prop.name).w, prop.targetValue.w * ((1 - currentProgress * prop.perlinNoiseAnimationSize.w) + (Mathf.PerlinNoise(eff.effectEnabledTimer * prop.perlinNoiseAnimationSpeed.w, GetHashCode())) * (currentProgress * prop.perlinNoiseAnimationSize.w * 2)), Time.deltaTime * prop.speedMultiplier.w)));
                            break;
                        case ReflectObjectVectorProperty.EmissionIntensity:
                            mat.SetVector(prop.name, new Vector4(eff.shaderEmissionMultiplier * currentProgress, eff.shaderEmissionMultiplier * currentProgress, 1.0f, 0));
                            break;
                        case ReflectObjectVectorProperty.SpreadCenter:
                            mat.SetVector(prop.name, eff.GetEffectOrigin());
                            break;
                        case ReflectObjectVectorProperty.NullifyCenter:
                            mat.SetVector(prop.name, eff.GetNullifyCenter());
                            break;
                    }

                }
            }
        }
    }
}
