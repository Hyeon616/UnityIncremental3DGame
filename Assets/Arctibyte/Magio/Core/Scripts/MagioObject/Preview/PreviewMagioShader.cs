#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Magio
{
    [ExecuteInEditMode]
    public class PreviewMagioShader : MonoBehaviour
    {
        [HideInInspector]
        public MagioObjectEffect magioObj;

        [HideInInspector]
        public List<Renderer> rends;

        [HideInInspector]
        public Material[] debugMats;

        [HideInInspector]
        public GameObject magioObjCopy;

        [HideInInspector]
        [Tooltip("This has only effect with Magio shader")]
        public Vector3 _effectSpreadOriginLocal = Vector3.zero;

        [HideInInspector]
        [Header("Enable Gizmos to force scene update")]
        [Tooltip("Gradually increases spread status")]
        public bool animateEffectSpreadStatus = false;

        [HideInInspector]
        [Range(0, 1)]
        [Tooltip("Drag this value to animate the spread")]
        public float effectSpreadStatus = 0;

        [HideInInspector]
        public PreviewMagioShaderController controller;

        [HideInInspector]
        public float effectSpread = 0;
        [HideInInspector]
        public float effectEnabledTimer = 0;

        private float _approxSize = 0;

        private MaterialPropertySaver saver;

        public void Setup(MagioObjectEffect magioO, PreviewMagioShaderController contr)
        {
            magioObj = magioO;
            controller = contr;
        }

        public void SetupDebugMaterials()
        {
            if (magioObjCopy != null)
            {
                DestroyImmediate(magioObjCopy);
            }

            magioObjCopy = Instantiate(magioObj.targetGameObject.gameObject, magioObj.targetGameObject.transform.parent);
            magioObjCopy.name += "_debug";

            MagioEngine.instance.creatingDebug = true;
            //Do not destroy other debug objects
            PreviewMagioVFX debugVFX = magioObjCopy.GetComponent<PreviewMagioVFX>();
            if (debugVFX)
            {
                debugVFX.effects.Clear();
                debugVFX.effects = null;
                debugVFX.tempColliders.Clear();
                debugVFX.tempColliders = null;
                DestroyImmediate(magioObjCopy.GetComponent<PreviewMagioVFX>());
            }
            DestroyImmediate(magioObjCopy.GetComponent<PreviewMagioShader>());
            foreach (MagioObjectEffect magObj in magioObjCopy.GetComponentsInChildren<MagioObjectEffect>())
            {
                DestroyImmediate(magObj);
            }

            foreach (Renderer rend in magioObj.targetGameObject.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = false;
            }

            if (magioObj.customStartOrigin)
            {
                _effectSpreadOriginLocal = magioObj.targetGameObject.transform.InverseTransformPoint(magioObj.customStartOrigin.position);
            }


            rends = magioObjCopy.gameObject.GetComponentsInChildren<Renderer>().ToList();
            CalculateApproxSize();
            foreach (Renderer child in rends)
            {
                debugMats = new Material[child.sharedMaterials.Length];

                for (int i = 0; i < child.sharedMaterials.Length; i++)
                {
                    debugMats[i] = new Material(child.sharedMaterials[i]);
                    debugMats[i].name = debugMats[i] + "_Debug";
                }

                child.sharedMaterials = debugMats;
            }

            saver = magioObjCopy.GetComponent<MaterialPropertySaver>();

            if (!saver)
            {
                saver = magioObjCopy.AddComponent<MaterialPropertySaver>();
                saver.SaveSharedOriginalMaterialShaderProperties();
            }
        }


        private void Awake()
        {
            if (Application.isPlaying)
            {
                this.enabled = false;
            }

        }

        private void CalculateApproxSize()
        {
            List<BoxCollider> boxCols = magioObj.targetGameObject.GetComponentsInChildren<BoxCollider>().ToList();

            if (boxCols.Count > 0)
            {
                foreach (BoxCollider box in boxCols)
                {
                    _approxSize += box.size.magnitude;
                }
            }
            else
            {
                if (rends.Count <= 0)
                {
                    _approxSize = 1;
                }
                else
                {
                    Bounds combinedRendBounds = rends[0].bounds;

                    foreach (Renderer rend in rends)
                    {
                        if (rend != rends[0]) combinedRendBounds.Encapsulate(rend.bounds);
                    }

                    _approxSize = combinedRendBounds.size.magnitude;
                }
            }
        }

        public void UpdateShaders()
        {
            if (!magioObjCopy) return;

            if (rends.Count > 0)
            {
                foreach (Renderer rend in rends)
                {
                    for (int i = 0; i < rend.sharedMaterials.Length; i++)
                    {
                        if (rend.sharedMaterials[i].HasProperty("_IsMagioShader"))
                        {
                            SetupMagioShader(rend, i);
                            UpdateMagioShader(rend, i);

                        }
                        else
                        {

                            KeywordsEnableCompatibleShaders(rend, i);
                            UpdateSupportedThirdPartyShaders(rend, i);
                        }
                    }
                }
            }
        }

        private void UpdateMagioShader(Renderer rend, int i)
        {
            if (magioObj.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Dissolve)
            {
                rend.sharedMaterials[i].SetFloat("_DissolveSpread", effectSpread);
                rend.sharedMaterials[i].SetVector("_DissolveOrigin", magioObj.targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));
            }
            else if (magioObj.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Emission_Overlay)
            {
                rend.sharedMaterials[i].SetFloat("_EffectSpread", effectSpread);
                rend.sharedMaterials[i].SetVector("_EffectOrigin", magioObj.targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));
            }
            else
            {
                rend.sharedMaterials[i].SetFloat("_TextureOverlaySpread", effectSpread);
                rend.sharedMaterials[i].SetVector("_TextureOverlayOrigin", magioObj.targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));

                if (effectEnabledTimer > magioObj.fadeOutStart_s)
                {
                    float currentBurnout = Mathf.Clamp((1 - ((effectEnabledTimer - magioObj.fadeOutStart_s)) / magioObj.fadeOutLength_s), 0, 1);

                    rend.sharedMaterials[i].SetFloat("_LifelineMultiplier", Mathf.MoveTowards(rend.sharedMaterials[i].GetFloat("_LifelineMultiplier"), currentBurnout, 0.1f));
                }
                else
                {
                    rend.sharedMaterials[i].SetFloat("_LifelineMultiplier", 1);
                }
            }

            if (magioObj.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Emission_Overlay)
            {
                if (effectEnabledTimer >= magioObj.fadeOutStart_s)
                {
                    float currentBurnout = Mathf.Clamp((1 - ((effectEnabledTimer - magioObj.fadeOutStart_s)) / magioObj.fadeOutLength_s), 0, 1);
                    rend.sharedMaterials[i].SetFloat("_OverlayBrightness", Mathf.MoveTowards(rend.sharedMaterials[i].GetFloat("_OverlayBrightness"), currentBurnout, 0.1f));
                }
                else
                {
                    float currentProgress = Mathf.Min(1, effectEnabledTimer / magioObj.achieveEmissionColorTime_s);
                    rend.sharedMaterials[i].SetFloat("_OverlayBrightness",
                    Mathf.Lerp(0, 1, currentProgress) * ((1 - magioObj.shaderColorNoise) + (Mathf.PerlinNoise(effectSpread * magioObj.shaderColorNoiseSpeed, 0) * (magioObj.shaderColorNoise * 2))));

                }

                if (magioObj.blendToOtherTexture)
                {
                    if (effectEnabledTimer > magioObj.fadeOutStart_s / 2)
                    {
                        rend.sharedMaterials[i].SetFloat("_BlendTextureLerpT",
                                    Mathf.Clamp((effectEnabledTimer - (magioObj.fadeOutStart_s / 2)) / (magioObj.fadeOutStart_s / 2), 0, 1));
                    }
                }
            }
        }

        private void UpdateSupportedThirdPartyShaders(Renderer rend, int a)
        {
            UpdateCompatibleShaders(rend, a);
        }

        private void UpdateCompatibleShaders(Renderer rend, int a)
        {
            List<OAVAShaderCompatibilitySO> compShaders = MagioEngine.instance.GetCompatibleShaders();
            foreach (OAVAShaderCompatibilitySO shaderComp in compShaders)
            {
                if (rend.sharedMaterials[a].HasProperty(shaderComp.ShaderCheckProperty) ||
                    (shaderComp.ShaderCheckProperty.Equals(string.Empty) && rend.sharedMaterials[a].shader.name.ToLower().Equals(shaderComp.ShaderName.ToLower())))
                {
                    float currentProgress;
                    if (effectEnabledTimer < magioObj.fadeOutStart_s)
                    {
                        currentProgress = Mathf.Min(1, effectEnabledTimer / magioObj.achieveEmissionColorTime_s);
                    }
                    else
                    {
                        float timeSinceFadeOutStart = effectEnabledTimer - magioObj.fadeOutStart_s;
                        currentProgress = Mathf.Max(0, 1 - (timeSinceFadeOutStart / (magioObj.fadeOutLength_s)));
                    }

                    float shaderEmissionIntensity = 1;

                    if (shaderComp.useEmissionHDRColor)
                        shaderEmissionIntensity = magioObj.shaderEmissionMultiplier;

                    if (rend.sharedMaterials[a].HasProperty(shaderComp.ShaderMainColorPropertyName))
                    {
                        Color originalColor = saver.OriginalMaterialValues[rend.sharedMaterials[a]].originalMainColor;
                        rend.sharedMaterials[a].SetColor(shaderComp.ShaderMainColorPropertyName, Color.Lerp(originalColor,
                            new Color(magioObj.shaderEmissionColor.r * shaderEmissionIntensity,
                                magioObj.shaderEmissionColor.g * shaderEmissionIntensity,
                                magioObj.shaderEmissionColor.b * shaderEmissionIntensity),
                                currentProgress) * ((1 - magioObj.shaderColorNoise) + (Mathf.PerlinNoise(effectEnabledTimer * magioObj.shaderColorNoiseSpeed, 0)) * (magioObj.shaderColorNoise * 2)));
                    }
                    else if (!shaderComp.ShaderMainColorPropertyName.Equals(string.Empty))
                    {
                        Debug.LogWarning("Shader " + rend.sharedMaterials[a].shader.name + " does not contain color property: " + shaderComp.ShaderMainColorPropertyName + " which has been added to ShaderCompabilities");
                    }

                    if (rend.sharedMaterials[a].HasProperty(shaderComp.ShaderEmissionColorPropertyName))
                    {
                        Color originalColor = saver.OriginalMaterialValues[rend.sharedMaterials[a]].originalEmissionColor;
                        rend.sharedMaterials[a].SetColor(shaderComp.ShaderEmissionColorPropertyName,
                        Color.Lerp(originalColor,
                            new Color(magioObj.shaderEmissionColor.r * shaderEmissionIntensity,
                                magioObj.shaderEmissionColor.g * shaderEmissionIntensity,
                                magioObj.shaderEmissionColor.b * shaderEmissionIntensity),
                                currentProgress) * ((1 - magioObj.shaderColorNoise) + (Mathf.PerlinNoise(effectEnabledTimer * magioObj.shaderColorNoiseSpeed, 0)) * (magioObj.shaderColorNoise * 2)));
                    }
                    else if (!shaderComp.ShaderEmissionColorPropertyName.Equals(string.Empty))
                    {
                        Debug.LogWarning("Shader " + rend.sharedMaterials[a].shader.name + " does not contain color property: " + shaderComp.ShaderEmissionColorPropertyName + " which has been added to ShaderCompabilities");
                    }


                    if (effectEnabledTimer > magioObj.fadeOutStart_s)
                    {
                        shaderComp.SetAfterFadeOutProperties(rend.sharedMaterials[a], magioObj, currentProgress);
                    }
                    else
                    {
                        shaderComp.SetDuringProperties(rend.sharedMaterials[a], magioObj, currentProgress);
                    }
                    break;
                }
            }
        }

        private void SetupMagioShader(Renderer rend, int i)
        {
            if (magioObj.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Emission_Overlay)
            {
                rend.sharedMaterials[i].SetFloat("_UseOverlay", 1);
                rend.sharedMaterials[i].SetFloat("_UseDissolve", 0);
                rend.sharedMaterials[i].SetFloat("_UseTextureOverride", 0);
                rend.sharedMaterials[i].SetFloat("_VoronoiMove", magioObj.shaderColorNoiseSpeed * 2);
                rend.sharedMaterials[i].SetColor("_OverlayColor", magioObj.shaderEmissionColor);
                rend.sharedMaterials[i].SetFloat("_OverlayMaximumBrightness", magioObj.shaderEmissionMultiplier);
                rend.sharedMaterials[i].SetFloat("_OverlayMinimumBrightness", magioObj.shaderEmissionMultiplier / 50);

                rend.sharedMaterials[i].SetTexture("_BlendTextureAlbedo", magioObj.blendAlbedoMap);
                rend.sharedMaterials[i].SetTexture("_BlendTextureNormal", magioObj.blendNormalMap);

                rend.sharedMaterials[i].SetVector("_BlendTextureTiling", magioObj.blendTextureTiling);
                rend.sharedMaterials[i].SetFloat("_BlendTextureNormalStrength", magioObj.blendTextureNormalStrength);
            }
            else if (magioObj.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Dissolve)
            {
                rend.sharedMaterials[i].SetColor("_OverlayColor", magioObj.shaderEmissionColor);
                rend.sharedMaterials[i].SetFloat("_OverlayMaximumBrightness", magioObj.shaderEmissionMultiplier);
                rend.sharedMaterials[i].SetFloat("_OverlayMinimumBrightness", 0);
                rend.sharedMaterials[i].SetFloat("_DissolveEdgeWidth", magioObj.dissolveEmissionEdgeWidth);
                rend.sharedMaterials[i].SetFloat("_UseOverlay", 0);
                rend.sharedMaterials[i].SetFloat("_UseDissolve", 1);
                rend.sharedMaterials[i].SetFloat("_UseTextureOverride", 0);
            }
            else
            {
                rend.sharedMaterials[i].SetFloat("_UseOverlay", 0);
                rend.sharedMaterials[i].SetFloat("_UseDissolve", 0);
                rend.sharedMaterials[i].SetFloat("_UseTextureOverride", 1);

                rend.sharedMaterials[i].SetTexture("_OverrideAlbedo", magioObj.overrideAlbedoMap);
                rend.sharedMaterials[i].SetTexture("_OverrideNormalMap", magioObj.overrideNormalMap);

                rend.sharedMaterials[i].SetVector("_OverrideTiling", magioObj.overrideTextureTiling);
                rend.sharedMaterials[i].SetVector("_OverrideOffset", magioObj.overrideTextureOffset);

                rend.sharedMaterials[i].SetFloat("_OverrideStrength", magioObj.overrideStrength);
                rend.sharedMaterials[i].SetFloat("_OverrideAddWetness", magioObj.overrideAddWetness);
                rend.sharedMaterials[i].SetColor("_OverrideColor", magioObj.overrideColor);
            }
        }

        private void KeywordsEnableCompatibleShaders(Renderer rend, int a)
        {
            List<OAVAShaderCompatibilitySO> compShaders = MagioEngine.instance.GetCompatibleShaders();
            foreach (OAVAShaderCompatibilitySO shaderComp in compShaders)
            {
                if (rend.sharedMaterials[a].HasProperty(shaderComp.ShaderCheckProperty) ||
                    (shaderComp.ShaderCheckProperty.Equals(string.Empty) && rend.sharedMaterials[a].shader.name.ToLower().Equals(shaderComp.ShaderName.ToLower())))
                {
                    shaderComp.OnEffectStart(rend.sharedMaterials[a], magioObj);
                    break;
                }
            }
        }



        private void OnDisable()
        {
            Clean();

        }

        public void Clean()
        {
            if (debugMats != null)
            {
                foreach (Material mat in debugMats)
                {
                    DestroyImmediate(mat);
                }
                debugMats = null;
            }
            if (magioObjCopy)
            {
                DestroyImmediate(magioObjCopy);
            }
            magioObjCopy = null;
            if (magioObj)
            {
                foreach (Renderer rend in magioObj.targetGameObject.GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = true;
                }
            }


            if (rends != null)
            {
                rends.Clear();
                rends = null;
            }

            _approxSize = 0;
            effectEnabledTimer = 0;
            effectSpread = 0;
        }

        private void OnDestroy()
        {
            Clean();
            EditorApplication.delayCall += () =>
            {
                if (controller) GameObject.DestroyImmediate(controller);
            };
        }

        void OnDrawGizmos()
        {
            // Your gizmo drawing thing goes here if required...
            // Ensure continuous Update calls.
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
        }
    }
}
#endif
