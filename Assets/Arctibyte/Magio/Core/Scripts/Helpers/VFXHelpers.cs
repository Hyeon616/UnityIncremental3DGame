using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Magio
{
    /// <summary>
    /// Helps handling the vfx properties
    /// </summary>
    public static class VFXHelpers
    {
        /// <summary>
        /// Returns all VFX properties which are not Magio Properties (Marked as Magio_)
        /// </summary>
        /// <param name="eff">Visual effect to look for properties</param>
        /// <returns>A list of properties as serialized style struct</returns>
        public static List<MagioObjectEffect.VFXProps> GetNonMagioVFXProperties(VisualEffect eff)
        {
            List<VFXExposedProperty> props = new List<VFXExposedProperty>();
            List<MagioObjectEffect.VFXProps> resProps = new List<MagioObjectEffect.VFXProps>();
            if (eff.visualEffectAsset)
            {
                eff.visualEffectAsset.GetExposedProperties(props);
            }
            else return resProps;

            foreach (VFXExposedProperty prop in props)
            {
                if (prop.name.Contains("Magio")) continue;

                MagioObjectEffect.VFXProps vfxProps = new MagioObjectEffect.VFXProps();
                if (prop.type == typeof(AnimationCurve))
                {
                    vfxProps.animationCurveValue = eff.GetAnimationCurve(prop.name);
                    vfxProps.type = VFXParameterType.ANIMATION_CURVE;
                }
                else if (prop.type == typeof(bool))
                {
                    vfxProps.boolValue = eff.GetBool(prop.name);
                    vfxProps.type = VFXParameterType.BOOL;
                }
                else if (prop.type == typeof(float))
                {
                    vfxProps.floatValue = eff.GetFloat(prop.name);
                    vfxProps.type = VFXParameterType.FLOAT;
                }
                else if (prop.type == typeof(Gradient))
                {
                    vfxProps.gradientValue = eff.GetGradient(prop.name);
                    vfxProps.type = VFXParameterType.GRADIENT;
                }
                else if (prop.type == typeof(int))
                {
                    vfxProps.intValue = eff.GetInt(prop.name);
                    vfxProps.type = VFXParameterType.INT;
                }
                else if (prop.type == typeof(Matrix4x4))
                {
                    vfxProps.matrixValue = eff.GetMatrix4x4(prop.name);
                    vfxProps.type = VFXParameterType.MATRIX;
                }
                else if (prop.type == typeof(Mesh))
                {
                    vfxProps.meshValue = eff.GetMesh(prop.name);
                    vfxProps.type = VFXParameterType.MESH;
                }
                else if (prop.type == typeof(SkinnedMeshRenderer))
                {
                    vfxProps.skinnedMeshRendererValue = eff.GetSkinnedMeshRenderer(prop.name);
                    vfxProps.type = VFXParameterType.SKINNED_MESH_RENDERER;
                }
                else if (prop.type == typeof(Texture))
                {
                    vfxProps.textureValue = eff.GetTexture(prop.name);
                    vfxProps.type = VFXParameterType.TEXTURE;
                }
                else if (prop.type == typeof(uint))
                {
                    vfxProps.uintValue = eff.GetUInt(prop.name);
                    vfxProps.type = VFXParameterType.UINT;
                }
                else if (prop.type == typeof(Vector2))
                {
                    vfxProps.vector2Value = eff.GetVector2(prop.name);
                    vfxProps.type = VFXParameterType.VECTOR2;
                }
                else if (prop.type == typeof(Vector3))
                {
                    vfxProps.vector3Value = eff.GetVector3(prop.name);
                    vfxProps.type = VFXParameterType.VECTOR3;
                }
                else if (prop.type == typeof(Vector4))
                {
                    if(prop.name.ToLower().Contains("color") || prop.name.ToLower().Contains("colour"))
                    {
                        vfxProps.colorValue = eff.GetVector4(prop.name);
                        vfxProps.type = VFXParameterType.COLOR;
                    }
                    else
                    {
                        vfxProps.vector4Value = eff.GetVector4(prop.name);
                        vfxProps.type = VFXParameterType.VECTOR4;
                    }
                    
                }

                vfxProps.name = prop.name;

                resProps.Add(vfxProps);
            }

            return resProps;
        }


        /// <summary>
        /// Adds new values to the vfx property list, but does not override.
        /// </summary>
        /// <param name="eff">Effect for the properties</param>
        /// <param name="originalProps">Original magio object properties</param>
        /// <returns>Modified original magio properties with the new values</returns>
        public static List<MagioObjectEffect.VFXProps> UpdateVfxPropertyList(VisualEffect eff, List<MagioObjectEffect.VFXProps> originalProps)
        {
            List<VFXExposedProperty> props = new List<VFXExposedProperty>();
            List<MagioObjectEffect.VFXProps> resProps = new List<MagioObjectEffect.VFXProps>();

            if (eff.visualEffectAsset)
            {
                eff.visualEffectAsset.GetExposedProperties(props);
            }
            else return resProps;

            foreach (VFXExposedProperty prop in props)
            {
                if (prop.name.Contains("Magio")) continue;

                MagioObjectEffect.VFXProps vfxProps = new MagioObjectEffect.VFXProps();

                MagioObjectEffect.VFXProps[] originalPropArray = originalProps.Where(o => o.name == prop.name).Take(1).ToArray();

                if (originalPropArray.Any())
                {
                    MagioObjectEffect.VFXProps originalProp = originalPropArray[0];

                    resProps.Add(originalProp);
                }
                else
                {
                    if (prop.type == typeof(AnimationCurve))
                    {
                        vfxProps.animationCurveValue = eff.GetAnimationCurve(prop.name);
                        vfxProps.type = VFXParameterType.ANIMATION_CURVE;
                    }
                    else if (prop.type == typeof(bool))
                    {
                        vfxProps.boolValue = eff.GetBool(prop.name);
                        vfxProps.type = VFXParameterType.BOOL;
                    }
                    else if (prop.type == typeof(float))
                    {
                        vfxProps.floatValue = eff.GetFloat(prop.name);
                        vfxProps.type = VFXParameterType.FLOAT;
                    }
                    else if (prop.type == typeof(Gradient))
                    {
                        vfxProps.gradientValue = eff.GetGradient(prop.name);
                        vfxProps.type = VFXParameterType.GRADIENT;
                    }
                    else if (prop.type == typeof(int))
                    {
                        vfxProps.intValue = eff.GetInt(prop.name);
                        vfxProps.type = VFXParameterType.INT;
                    }
                    else if (prop.type == typeof(Matrix4x4))
                    {
                        vfxProps.matrixValue = eff.GetMatrix4x4(prop.name);
                        vfxProps.type = VFXParameterType.MATRIX;
                    }
                    else if (prop.type == typeof(Mesh))
                    {
                        vfxProps.meshValue = eff.GetMesh(prop.name);
                        vfxProps.type = VFXParameterType.MESH;
                    }
                    else if (prop.type == typeof(SkinnedMeshRenderer))
                    {
                        vfxProps.skinnedMeshRendererValue = eff.GetSkinnedMeshRenderer(prop.name);
                        vfxProps.type = VFXParameterType.SKINNED_MESH_RENDERER;
                    }
                    else if (prop.type == typeof(Texture))
                    {
                        vfxProps.textureValue = eff.GetTexture(prop.name);
                        vfxProps.type = VFXParameterType.TEXTURE;
                    }
                    else if (prop.type == typeof(uint))
                    {
                        vfxProps.uintValue = eff.GetUInt(prop.name);
                        vfxProps.type = VFXParameterType.UINT;
                    }
                    else if (prop.type == typeof(Vector2))
                    {
                        vfxProps.vector2Value = eff.GetVector2(prop.name);
                        vfxProps.type = VFXParameterType.VECTOR2;
                    }
                    else if (prop.type == typeof(Vector3))
                    {
                        vfxProps.vector3Value = eff.GetVector3(prop.name);
                        vfxProps.type = VFXParameterType.VECTOR3;
                    }
                    else if (prop.type == typeof(Vector4))
                    {
                        if (prop.name.ToLower().Contains("color") || prop.name.ToLower().Contains("colour"))
                        {
                            vfxProps.colorValue = eff.GetVector4(prop.name);
                            vfxProps.type = VFXParameterType.COLOR;
                        }
                        else
                        {
                            vfxProps.vector4Value = eff.GetVector4(prop.name);
                            vfxProps.type = VFXParameterType.VECTOR4;
                        }

                    }

                    vfxProps.name = prop.name;
                    resProps.Add(vfxProps);
                }

                
            }

            return resProps;
        }

        /// <summary>
        /// Sets the struct serialized like properties back to the effect.
        /// </summary>
        /// <param name="effect">Effect to set the properties</param>
        /// <param name="vfxProperties">List of properties to set</param>
        public static void SetupMagioParameters(VisualEffect effect, List<MagioObjectEffect.VFXProps> vfxProperties)
        {
            foreach (MagioObjectEffect.VFXProps prop in vfxProperties)
            {
                if (prop.type == VFXParameterType.ANIMATION_CURVE)
                {
                    if (effect.HasAnimationCurve(prop.name))
                        effect.SetAnimationCurve(prop.name, prop.animationCurveValue);
                }
                else if (prop.type == VFXParameterType.BOOL)
                {
                    if (effect.HasBool(prop.name))
                        effect.SetBool(prop.name, prop.boolValue);
                }
                else if (prop.type == VFXParameterType.FLOAT)
                {
                    if (effect.HasFloat(prop.name))
                        effect.SetFloat(prop.name, prop.floatValue);
                }
                else if (prop.type == VFXParameterType.GRADIENT)
                {
                    if (effect.HasGradient(prop.name))
                        effect.SetGradient(prop.name, prop.gradientValue);
                }
                else if (prop.type == VFXParameterType.INT)
                {
                    if (effect.HasInt(prop.name))
                        effect.SetInt(prop.name, prop.intValue);
                }
                else if (prop.type == VFXParameterType.MATRIX)
                {
                    if (effect.HasMatrix4x4(prop.name))
                        effect.SetMatrix4x4(prop.name, prop.matrixValue);
                }
                else if (prop.type == VFXParameterType.MESH)
                {
                    if (effect.HasMesh(prop.name))
                        effect.SetMesh(prop.name, prop.meshValue);
                }
                else if (prop.type == VFXParameterType.SKINNED_MESH_RENDERER)
                {
                    if (effect.HasSkinnedMeshRenderer(prop.name))
                        effect.SetSkinnedMeshRenderer(prop.name, prop.skinnedMeshRendererValue);
                }
                else if (prop.type == VFXParameterType.TEXTURE)
                {
                    if (effect.HasTexture(prop.name))
                        effect.SetTexture(prop.name, prop.textureValue);
                }
                else if (prop.type == VFXParameterType.UINT)
                {
                    if (effect.HasUInt(prop.name))
                        effect.SetUInt(prop.name, prop.uintValue);
                }
                else if (prop.type == VFXParameterType.VECTOR2)
                {
                    if (effect.HasVector2(prop.name))
                        effect.SetVector2(prop.name, prop.vector2Value);
                }
                else if (prop.type == VFXParameterType.VECTOR3)
                {
                    if (effect.HasVector3(prop.name))
                        effect.SetVector3(prop.name, prop.vector3Value);
                }
                else if (prop.type == VFXParameterType.VECTOR4)
                {
                    if (effect.HasVector4(prop.name))
                        effect.SetVector4(prop.name, prop.vector4Value);
                }
                else if (prop.type == VFXParameterType.COLOR)
                {
                    if (effect.HasVector4(prop.name))
                        effect.SetVector4(prop.name, prop.colorValue);
                }
            }
        }
    }
}

