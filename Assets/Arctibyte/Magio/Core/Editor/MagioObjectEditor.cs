using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace Magio
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MagioObjectEffect))]
    public class MagioObjectEditor : Editor
    {
        public Texture logo;
        private SerializedProperty maxSpread;
        private SerializedProperty effectCrawlSpeed;
        private SerializedProperty spreadingColliders;
        private SerializedProperty magioRenderers;
        private SerializedProperty enableMaterialAnimation;
        private SerializedProperty shaderEmissionColor;
        private SerializedProperty shaderToEffectEndInterpolateSpeed;
        private SerializedProperty shaderEmissionMultiplier;
        private SerializedProperty achieveEmissionColorTime_s;
        private SerializedProperty shaderColorNoise;
        private SerializedProperty shaderColorNoiseSpeed;
        private SerializedProperty effectSpreadAreaAddition;
        private SerializedProperty beginEffectOnStart;
        private SerializedProperty effectSpread;
        private SerializedProperty effectEnabledTimer;
        private SerializedProperty fadeOutStart_s;
        private SerializedProperty fadeOutLength_s;
        private SerializedProperty canBeReAnimated;
        private SerializedProperty affectedByWind;
        private SerializedProperty ignitionTime;
        private SerializedProperty addSFXRuntime;
        private SerializedProperty effectSFX;
        private SerializedProperty customStartOrigin;
        private SerializedProperty fullNullifyToughness;
        private SerializedProperty backSpreadCoolDown_s;
        private SerializedProperty vfxProperties;
        private SerializedProperty vfxSpawnerType;
        private SerializedProperty effectBehaviourType;
        private SerializedProperty spreadToOtherObjects;
        private SerializedProperty effectClass;
        private SerializedProperty spreadLayerMask;
        private SerializedProperty useEffectOnAllRenderers;
        private SerializedProperty useAllCollidersToSpread;
        private SerializedProperty magioShaderEffectMode;
        private SerializedProperty effectOnForever;
        private SerializedProperty addedExternalVelocity;
        private SerializedProperty allEffectSFX;
        private SerializedProperty slowAnimationOnEffectStart;
        private SerializedProperty animator;
        private SerializedProperty animationSlowDownSpeed;
        private SerializedProperty animationSlowDownTargetValue;

        private SerializedProperty overrideAlbedoMap;
        private SerializedProperty overrideNormalMap;
        private SerializedProperty overrideTextureTiling;
        private SerializedProperty overrideTextureOffset;
        private SerializedProperty blendToOtherTexture;
        private SerializedProperty overrideStrength;
        private SerializedProperty overrideAddWetness;
        private SerializedProperty overrideColor;

        private SerializedProperty blendAlbedoMap;
        private SerializedProperty blendNormalMap;
        private SerializedProperty blendTextureTiling;
        private SerializedProperty blendTextureNormalStrength;

        private SerializedProperty useOnThisGameObject;
        private SerializedProperty targetGameObject;

        private SerializedProperty deleteObjectAfterFullSpread;

        private SerializedProperty effectNumber;
        private SerializedProperty effectPackNumber;

        private SerializedProperty _effectPrefab;

        private SerializedProperty isSplashEffect;
        private SerializedProperty deleteObjectAfterFadeOut;

        private SerializedProperty dissolveEmissionEdgeWidth;

        private SerializedProperty sfxPitchRandomization;
        private SerializedProperty sfxStartTimeRandomizationMax;

        private SerializedProperty sfxMode;

        private SerializedProperty enableVFX;


        bool materialCompatibilities = true;
        bool lightFoldout = true;

        private int realOpenTab = 0;

        List<string> errors = new List<string>();

        private MagioObjectEffect magioObj;

        private bool effectChange = false;

        private void OnEnable()
        {
            maxSpread = serializedObject.FindProperty("maxSpread");
            effectCrawlSpeed = serializedObject.FindProperty("effectCrawlSpeed");
            spreadingColliders = serializedObject.FindProperty("spreadingColliders");
            magioRenderers = serializedObject.FindProperty("magioRenderers");
            enableMaterialAnimation = serializedObject.FindProperty("enableMaterialAnimation");
            shaderEmissionColor = serializedObject.FindProperty("shaderEmissionColor");
            shaderToEffectEndInterpolateSpeed = serializedObject.FindProperty("shaderToEffectEndInterpolateSpeed");
            shaderEmissionMultiplier = serializedObject.FindProperty("shaderEmissionMultiplier");
            achieveEmissionColorTime_s = serializedObject.FindProperty("achieveEmissionColorTime_s");
            shaderColorNoise = serializedObject.FindProperty("shaderColorNoise");
            shaderColorNoiseSpeed = serializedObject.FindProperty("shaderColorNoiseSpeed");
            effectSpreadAreaAddition = serializedObject.FindProperty("effectSpreadAreaAddition");
            beginEffectOnStart = serializedObject.FindProperty("beginEffectOnStart");
            effectSpread = serializedObject.FindProperty("effectSpread");
            effectEnabledTimer = serializedObject.FindProperty("effectEnabledTimer");
            fadeOutStart_s = serializedObject.FindProperty("fadeOutStart_s");
            fadeOutLength_s = serializedObject.FindProperty("fadeOutLength_s");
            canBeReAnimated = serializedObject.FindProperty("canBeReAnimated");
            affectedByWind = serializedObject.FindProperty("affectedByWind");
            ignitionTime = serializedObject.FindProperty("ignitionTime");
            addSFXRuntime = serializedObject.FindProperty("addSFXRuntime");
            effectSFX = serializedObject.FindProperty("effectSFX");
            customStartOrigin = serializedObject.FindProperty("customStartOrigin");
            fullNullifyToughness = serializedObject.FindProperty("fullNullifyToughness");
            backSpreadCoolDown_s = serializedObject.FindProperty("backSpreadCoolDown_s");
            vfxProperties = serializedObject.FindProperty("vfxProperties");
            vfxSpawnerType = serializedObject.FindProperty("vfxSpawnerType");
            effectBehaviourType = serializedObject.FindProperty("effectBehaviourType");
            spreadToOtherObjects = serializedObject.FindProperty("spreadToOtherObjects");
            effectClass = serializedObject.FindProperty("effectClass");
            spreadLayerMask = serializedObject.FindProperty("spreadLayerMask");
            useEffectOnAllRenderers = serializedObject.FindProperty("useEffectOnAllRenderers");
            useAllCollidersToSpread = serializedObject.FindProperty("useAllCollidersToSpread");
            magioShaderEffectMode = serializedObject.FindProperty("magioShaderEffectMode");
            effectOnForever = serializedObject.FindProperty("effectOnForever");
            addedExternalVelocity = serializedObject.FindProperty("addedExternalVelocity");
            allEffectSFX = serializedObject.FindProperty("allEffectSFX");

            slowAnimationOnEffectStart = serializedObject.FindProperty("slowAnimationOnEffectStart");
            animator = serializedObject.FindProperty("animator");
            animationSlowDownSpeed = serializedObject.FindProperty("animationSlowDownSpeed");
            animationSlowDownTargetValue = serializedObject.FindProperty("animationSlowDownTargetValue");

            overrideAlbedoMap = serializedObject.FindProperty("overrideAlbedoMap");
            overrideNormalMap = serializedObject.FindProperty("overrideNormalMap");
            overrideTextureTiling = serializedObject.FindProperty("overrideTextureTiling");
            overrideTextureOffset = serializedObject.FindProperty("overrideTextureOffset");
            blendToOtherTexture = serializedObject.FindProperty("blendToOtherTexture");
            overrideStrength = serializedObject.FindProperty("overrideStrength");
            overrideAddWetness = serializedObject.FindProperty("overrideAddWetness");
            overrideColor = serializedObject.FindProperty("overrideColor");

            blendAlbedoMap = serializedObject.FindProperty("blendAlbedoMap");
            blendNormalMap = serializedObject.FindProperty("blendNormalMap");
            blendTextureTiling = serializedObject.FindProperty("blendTextureTiling");
            blendTextureNormalStrength = serializedObject.FindProperty("blendTextureNormalStrength");


            useOnThisGameObject = serializedObject.FindProperty("useOnThisGameObject");
            targetGameObject = serializedObject.FindProperty("targetGameObject");

            deleteObjectAfterFullSpread = serializedObject.FindProperty("deleteObjectAfterFullSpread");

            effectNumber = serializedObject.FindProperty("effectNumber");
            effectPackNumber = serializedObject.FindProperty("effectPackNumber");

            _effectPrefab = serializedObject.FindProperty("_effectPrefab");

            isSplashEffect = serializedObject.FindProperty("isSplashEffect");
            deleteObjectAfterFadeOut = serializedObject.FindProperty("deleteObjectAfterFadeOut");

            dissolveEmissionEdgeWidth = serializedObject.FindProperty("dissolveEmissionEdgeWidth");

            sfxPitchRandomization = serializedObject.FindProperty("sfxPitchRandomization");
            sfxStartTimeRandomizationMax = serializedObject.FindProperty("sfxStartTimeRandomizationMax");

            sfxMode = serializedObject.FindProperty("sfxMode");

            enableVFX = serializedObject.FindProperty("enableVFX");

            magioObj = (MagioObjectEffect)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTypeDropDown();

            EditorGUI.BeginChangeCheck();

            foreach (Object magioO in targets)
            {
                MagioObjectEffect eff = magioO as MagioObjectEffect;
                if (eff.targetGameObject == null)
                {
                    eff.targetGameObject = eff.gameObject;
                }
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            CheckForErrors();

            magioObj.openTabUpper = GUILayout.Toolbar(magioObj.openTabUpper, new string[] { "System", "VFX", "Material Animation" });
            switch (magioObj.openTabUpper)
            {
                case 0:
                    realOpenTab = 0;
                    magioObj.openTabLower = 4;
                    break;
                case 1:
                    realOpenTab = 1;
                    magioObj.openTabLower = 4;
                    break;
                case 2:
                    realOpenTab = 2;
                    magioObj.openTabLower = 4;
                    break;


            }

            magioObj.openTabLower = GUILayout.Toolbar(magioObj.openTabLower, new string[] { "SFX", "Advanced", "Errors" + " (" + errors.Count + ")" });
            switch (magioObj.openTabLower)
            {
                case 0:
                    realOpenTab = 3;
                    magioObj.openTabUpper = 4;
                    break;
                case 1:
                    realOpenTab = 4;
                    magioObj.openTabUpper = 4;
                    break;
                case 2:
                    realOpenTab = 5;
                    magioObj.openTabUpper = 4;
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object magioO in targets)
                {
                    MagioObjectEffect eff = magioO as MagioObjectEffect;
                    eff.openTabUpper = magioObj.openTabUpper;
                    eff.openTabLower = magioObj.openTabLower;
                }

                serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

            EditorGUI.BeginChangeCheck();

            switch (realOpenTab)
            {
                case 0:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("General Behaviour", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(useOnThisGameObject);
                    if (!magioObj.useOnThisGameObject)
                    {
                        EditorGUILayout.PropertyField(targetGameObject);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(effectClass);
                    EditorGUILayout.PropertyField(effectBehaviourType);
                    if (magioObj.effectBehaviourType == EffectBehaviourMode.Spread)
                    {
                        EditorGUILayout.PropertyField(beginEffectOnStart);
                        if (magioObj.effectBehaviourType == EffectBehaviourMode.Spread)
                        {
                            if (magioObj.beginEffectOnStart)
                            {
                                EditorGUILayout.PropertyField(customStartOrigin);
                            }
                        }
                    }


                    if (magioObj.effectBehaviourType == EffectBehaviourMode.Spread)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Spread Behaviour", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(effectCrawlSpeed);
                        EditorGUILayout.PropertyField(spreadToOtherObjects);

                        if (magioObj.spreadToOtherObjects)
                        {
                            EditorGUILayout.PropertyField(spreadLayerMask);
                            EditorGUILayout.PropertyField(effectSpreadAreaAddition);
                            EditorGUILayout.PropertyField(useAllCollidersToSpread);
                            if (!magioObj.useAllCollidersToSpread)
                            {
                                EditorGUILayout.PropertyField(spreadingColliders);
                            }

                            if (magioObj.targetGameObject.GetComponentInChildren<MeshCollider>())
                            {
                                GUIStyle s = new GUIStyle(EditorStyles.textField);
                                s.normal.textColor = Color.yellow;
                                s.wordWrap = true;
                                EditorGUILayout.LabelField("Note: Mesh colliders cannot spread effects forward. Add other type collider e.g. on no-physics layer", s);
                            }
                        }
                    }


                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);

                    if (magioObj.effectBehaviourType == EffectBehaviourMode.Spread)
                    {
                        EditorGUILayout.PropertyField(ignitionTime);
                    }

                    EditorGUILayout.PropertyField(effectOnForever);
                    if (!magioObj.effectOnForever)
                    {
                        EditorGUILayout.PropertyField(fadeOutStart_s);

                    }
                    EditorGUILayout.PropertyField(fadeOutLength_s);

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Additional", EditorStyles.boldLabel);

                    if (magioObj.effectBehaviourType == EffectBehaviourMode.Spread)
                    {
                        EditorGUILayout.PropertyField(canBeReAnimated);
                    }
                    EditorGUILayout.PropertyField(affectedByWind);
                    EditorGUILayout.PropertyField(addedExternalVelocity);
                    EditorGUILayout.PropertyField(useEffectOnAllRenderers);

                    if (!magioObj.useEffectOnAllRenderers)
                    {
                        EditorGUILayout.PropertyField(magioRenderers);
                    }

                    break;
                case 1:
                    if (!magioObj.EffectPrefab || !magioObj.EffectPrefab.GetComponent<VisualEffect>())
                    {
                        GUIStyle s = new GUIStyle(EditorStyles.textField);
                        s.normal.textColor = Color.red;
                        EditorGUILayout.LabelField("Error: No Visual Effect in the prefab", s);
                    }
                    else
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(enableVFX);
                        bool sameEffect = true;

                        GameObject firstEffect = magioObj.EffectPrefab;

                        if (Selection.gameObjects.Length > 1)
                        {
                            foreach (Object magioO in targets)
                            {
                                MagioObjectEffect eff = magioO as MagioObjectEffect;

                                if (eff.EffectPrefab != firstEffect)
                                {
                                    sameEffect = false;
                                }
                            }
                        }



                        if (!sameEffect)
                        {
                            GUIStyle s = new GUIStyle(EditorStyles.textField);
                            s.normal.textColor = Color.yellow;
                            s.wordWrap = true;
                            EditorGUILayout.LabelField("Objects with different Visual effect prefabs selected, cannot edit.", s);
                        }
                        else
                        {
                            ConstructDynamicListOfParameters();
                            EditorGUILayout.Space();
                        }

                        bool previews = false;
                        HashSet<GameObject> targetGOs = new HashSet<GameObject>();
                        bool swap = false;
                        bool invalid = false;

                        foreach (Object magioO in targets)
                        {
                            MagioObjectEffect eff = magioO as MagioObjectEffect;
                            PreviewMagioVFX debug = eff.targetGameObject.GetComponent<PreviewMagioVFX>();
                            if (debug)
                            {
                                previews = true;

                                if (debug.magioObj != eff)
                                {
                                    swap = true;
                                }
                            }

                            if (targetGOs.Contains(eff.targetGameObject))
                            {
                                invalid = true;
                                break;
                            }
                            targetGOs.Add(eff.targetGameObject);
                        }

                        if (invalid)
                        {
                            GUIStyle s = new GUIStyle(EditorStyles.textField);
                            s.normal.textColor = Color.yellow;
                            s.wordWrap = true;
                            EditorGUILayout.LabelField("Cannot preview multiple VFX on same target game object.", s);
                        }
                        else
                        {
                            if (!previews)
                            {
                                if (GUILayout.Button("Preview VFX"))
                                {
                                    foreach (Object magioO in targets)
                                    {
                                        MagioObjectEffect eff = magioO as MagioObjectEffect;
                                        PreviewMagioVFX vfxPreview = Undo.AddComponent<PreviewMagioVFX>(eff.targetGameObject);
                                        PreviewMagioVFXController vfxPreviewControl = Undo.AddComponent<PreviewMagioVFXController>(eff.gameObject);
                                        vfxPreviewControl.previewMaster = vfxPreview;

                                        ExtraPropertiesForEditor extra = magioObj.EffectPrefab.GetComponent<ExtraPropertiesForEditor>();

                                        if (extra)
                                        {
                                            vfxPreviewControl.effectSpread = extra.defaultSpread;
                                        }

                                        vfxPreview.SetupAndStart(eff, vfxPreviewControl);
                                    }
                                }
                            }
                            else if (swap)
                            {
                                if (GUILayout.Button("Swap preview VFX"))
                                {
                                    foreach (Object magioO in targets)
                                    {
                                        MagioObjectEffect eff = magioO as MagioObjectEffect;
                                        PreviewMagioVFX debug = eff.targetGameObject.GetComponent<PreviewMagioVFX>();
                                        if (debug)
                                        {
                                            Undo.DestroyObjectImmediate(debug);
                                        }
                                        PreviewMagioVFX vfxPreview = Undo.AddComponent<PreviewMagioVFX>(eff.targetGameObject);
                                        PreviewMagioVFXController vfxPreviewControl = Undo.AddComponent<PreviewMagioVFXController>(eff.gameObject);
                                        vfxPreviewControl.previewMaster = vfxPreview;

                                        ExtraPropertiesForEditor extra = magioObj.EffectPrefab.GetComponent<ExtraPropertiesForEditor>();

                                        if (extra)
                                        {
                                            vfxPreviewControl.effectSpread = extra.defaultSpread;
                                        }

                                        vfxPreview.SetupAndStart(eff, vfxPreviewControl);
                                    }
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Stop previewing VFX"))
                                {
                                    foreach (Object magioO in targets)
                                    {
                                        MagioObjectEffect eff = magioO as MagioObjectEffect;
                                        PreviewMagioVFX debug = eff.targetGameObject.GetComponent<PreviewMagioVFX>();
                                        if (debug)
                                        {
                                            Undo.DestroyObjectImmediate(debug);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(enableMaterialAnimation);
                    if (magioObj.enableMaterialAnimation && !enableMaterialAnimation.hasMultipleDifferentValues)
                    {

                        int magioShaderEmissionCount = 0;
                        int magioShaderDissolveCount = 0;
                        int magioShaderOverlayCount = 0;
                        int compatibleShaders = 0;
                        int magioShaders = 0;
                        EditorGUILayout.Space();

                        materialCompatibilities = EditorGUILayout.Foldout(materialCompatibilities, "Compatibility information");

                        foreach (Object magioO in targets)
                        {
                            MagioObjectEffect eff = magioO as MagioObjectEffect;
                            Renderer[] rends = eff.targetGameObject.GetComponentsInChildren<Renderer>();

                            if (rends.Length > 0)
                            {
                                foreach (Renderer rend in rends)
                                {
                                    MagioMaterialAnimationSettings overrideComp = rend.gameObject.GetComponent<MagioMaterialAnimationSettings>();
                                    if (!overrideComp) overrideComp = Undo.AddComponent<MagioMaterialAnimationSettings>(rend.gameObject);
                                    List<OAVAShaderCompatibilitySO> comps = overrideComp.GetNewCompatibilitiesList();
                                    for (int i = 0; i < comps.Count; i++)
                                    {
                                        OAVAShaderCompatibilitySO comp = comps[i];
                                        if (comp == null)
                                        {
                                            if (rend.sharedMaterials[i].HasProperty("_IsMagioShader"))
                                            {
                                                if (eff.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Dissolve)
                                                {
                                                    magioShaderDissolveCount++;
                                                }
                                                else if (eff.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Emission_Overlay)
                                                {
                                                    magioShaderEmissionCount++;
                                                }
                                                else
                                                {
                                                    magioShaderOverlayCount++;
                                                }
                                            }
                                            else
                                            {
                                                if (materialCompatibilities)
                                                {
                                                    GUIStyle s = new GUIStyle(EditorStyles.textField);
                                                    s.normal.textColor = Color.yellow;
                                                    s.wordWrap = true;
                                                    EditorGUILayout.LabelField("Shader compatibility not found " + rend.gameObject.name + " -> " + rend.sharedMaterials[i].name, s);
                                                }

                                            }
                                        }
                                        else
                                        {
                                            compatibleShaders++;

                                        }
                                    }
                                }
                            }
                            else
                            {
                                // If there is no renderers expose everything.
                                if (magioObj.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Dissolve)
                                {
                                    magioShaderDissolveCount++;
                                }
                                else if (magioObj.magioShaderEffectMode == MagioObjectEffect.MagioShaderEffectMode.Emission_Overlay)
                                {
                                    magioShaderEmissionCount++;
                                }
                                else
                                {
                                    magioShaderOverlayCount++;
                                }

                                compatibleShaders++;
                            }
                        }

                        magioShaders = magioShaderDissolveCount + magioShaderEmissionCount + magioShaderOverlayCount;

                        if (materialCompatibilities)
                        {
                            if (magioShaders > 0)
                            {
                                GUIStyle s = new GUIStyle(EditorStyles.textField);
                                s.normal.textColor = Color.green;
                                EditorGUILayout.LabelField(magioShaders + " Magio Shader found", s);
                            }
                            if (compatibleShaders > 0)
                            {
                                GUIStyle s = new GUIStyle(EditorStyles.textField);
                                s.normal.textColor = Color.green;
                                EditorGUILayout.LabelField(compatibleShaders + " Compatible shaders found", s);
                            }
                        }

                        if (magioShaders > 0)
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("Magio Shader options", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(magioShaderEffectMode);

                            if (magioShaderOverlayCount > 0)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Texture Override", EditorStyles.boldLabel);
                                EditorGUILayout.PropertyField(overrideAlbedoMap);
                                EditorGUILayout.PropertyField(overrideColor);
                                EditorGUILayout.PropertyField(overrideNormalMap);
                                EditorGUILayout.PropertyField(overrideTextureTiling);
                                EditorGUILayout.PropertyField(overrideTextureOffset);
                                EditorGUILayout.PropertyField(overrideStrength);
                                EditorGUILayout.PropertyField(overrideAddWetness);
                            }

                            if (magioShaderEmissionCount > 0)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Emission Overlay", EditorStyles.boldLabel);
                                EditorGUILayout.PropertyField(blendToOtherTexture);
                                if (magioObj.blendToOtherTexture)
                                {
                                    EditorGUILayout.PropertyField(blendAlbedoMap);
                                    EditorGUILayout.PropertyField(blendNormalMap);
                                    EditorGUILayout.PropertyField(blendTextureNormalStrength);
                                    EditorGUILayout.PropertyField(blendTextureTiling);
                                }
                            }

                            if (magioShaderDissolveCount > 0)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Dissolve", EditorStyles.boldLabel);
                                EditorGUILayout.PropertyField(dissolveEmissionEdgeWidth);
                            }
                        }

                        EditorGUILayout.Space();

                        if (magioShaderEmissionCount > 0 || magioShaderDissolveCount > 0 || compatibleShaders > 0)
                        {
                            EditorGUILayout.LabelField("General Shader options", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(shaderEmissionColor);
                            EditorGUILayout.PropertyField(shaderEmissionMultiplier);
                            EditorGUILayout.Space();
                        }

                        if (magioShaderEmissionCount > 0 || compatibleShaders > 0)
                        {
                            EditorGUILayout.PropertyField(shaderToEffectEndInterpolateSpeed);
                            EditorGUILayout.PropertyField(achieveEmissionColorTime_s);
                            EditorGUILayout.PropertyField(shaderColorNoise);
                            EditorGUILayout.PropertyField(shaderColorNoiseSpeed);
                            EditorGUILayout.Space();
                        }

                        bool previews = false;
                        HashSet<GameObject> targetGOs = new HashSet<GameObject>();
                        bool swap = false;
                        bool invalid = false;

                        foreach (Object magioO in targets)
                        {
                            MagioObjectEffect eff = magioO as MagioObjectEffect;
                            PreviewMagioShader debug = eff.targetGameObject.GetComponent<PreviewMagioShader>();
                            if (debug)
                            {
                                previews = true;

                                if (debug.magioObj != eff)
                                {
                                    swap = true;
                                }
                            }

                            if (targetGOs.Contains(eff.targetGameObject))
                            {
                                invalid = true;
                                break;
                            }
                            targetGOs.Add(eff.targetGameObject);
                        }

                        if (invalid)
                        {
                            GUIStyle s = new GUIStyle(EditorStyles.textField);
                            s.normal.textColor = Color.yellow;
                            s.wordWrap = true;
                            EditorGUILayout.LabelField("Cannot preview multiple material animations on same target game object.", s);
                        }
                        else
                        {

                            if (!previews)
                            {
                                if (GUILayout.Button("Preview Material Animation"))
                                {
                                    foreach (Object magioO in targets)
                                    {
                                        MagioObjectEffect eff = magioO as MagioObjectEffect;

                                        PreviewMagioShader previewShader = Undo.AddComponent<PreviewMagioShader>(eff.targetGameObject);
                                        PreviewMagioShaderController shaderPreviewControl = Undo.AddComponent<PreviewMagioShaderController>(eff.gameObject);
                                        shaderPreviewControl.previewMaster = previewShader;

                                        previewShader.Setup(eff, shaderPreviewControl);

                                        ExtraPropertiesForEditor extra = magioObj.EffectPrefab.GetComponent<ExtraPropertiesForEditor>();

                                        if (extra)
                                        {
                                            shaderPreviewControl.SetStatusFromSpread(extra.defaultSpread);
                                        }
                                    }

                                }
                            }
                            else if (swap)
                            {
                                if (GUILayout.Button("Swap Material Animation Preview"))
                                {
                                    foreach (Object magioO in targets)
                                    {
                                        MagioObjectEffect eff = magioO as MagioObjectEffect;
                                        PreviewMagioShader debug = eff.targetGameObject.GetComponent<PreviewMagioShader>();
                                        if (debug)
                                        {
                                            Undo.DestroyObjectImmediate(debug);
                                        }
                                        PreviewMagioShader previewShader = Undo.AddComponent<PreviewMagioShader>(eff.targetGameObject);
                                        PreviewMagioShaderController shaderPreviewControl = Undo.AddComponent<PreviewMagioShaderController>(eff.gameObject);
                                        shaderPreviewControl.previewMaster = previewShader;

                                        previewShader.Setup(eff, shaderPreviewControl);

                                        ExtraPropertiesForEditor extra = magioObj.EffectPrefab.GetComponent<ExtraPropertiesForEditor>();

                                        if (extra)
                                        {
                                            shaderPreviewControl.SetStatusFromSpread(extra.defaultSpread);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Stop Material Animation Preview"))
                                {
                                    foreach (Object magioO in targets)
                                    {
                                        MagioObjectEffect eff = magioO as MagioObjectEffect;
                                        PreviewMagioShader debug = eff.targetGameObject.GetComponent<PreviewMagioShader>();
                                        if (debug)
                                        {
                                            Undo.DestroyObjectImmediate(debug);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        PreviewMagioShader shader = magioObj.targetGameObject.GetComponent<PreviewMagioShader>();
                        if (shader)
                            Undo.DestroyObjectImmediate(shader);
                    }

                    break;
                case 3:
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(sfxMode);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(addSFXRuntime);
                    if (magioObj.addSFXRuntime && !addSFXRuntime.hasMultipleDifferentValues)
                    {
                        EditorGUILayout.PropertyField(effectSFX);
                        if (magioObj.effectSFX)
                        {
                            EditorGUILayout.PropertyField(sfxPitchRandomization);
                            EditorGUILayout.PropertyField(sfxStartTimeRandomizationMax);
                        }

                    }

                    EditorGUILayout.PropertyField(allEffectSFX);

                    break;
                case 4:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Spread settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(maxSpread);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Current progress", EditorStyles.boldLabel);
                    if (magioObj.effectBehaviourType == EffectBehaviourMode.Spread)
                    {
                        EditorGUILayout.PropertyField(effectSpread);
                    }
                    EditorGUILayout.PropertyField(effectEnabledTimer);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Nullify parameters", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(fullNullifyToughness);
                    EditorGUILayout.PropertyField(backSpreadCoolDown_s);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Extra settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(deleteObjectAfterFullSpread);
                    EditorGUILayout.PropertyField(deleteObjectAfterFadeOut);
                    EditorGUILayout.PropertyField(isSplashEffect);
                    lightFoldout = EditorGUILayout.Foldout(lightFoldout, "Lights");

                    if (lightFoldout)
                    {
                        GUIStyle styl = new GUIStyle(EditorStyles.textField);
                        styl.normal.textColor = Color.white;
                        styl.wordWrap = true;
                        EditorGUILayout.LabelField("To animate lights with Magio effects: Add MagioLight script to the light gameobject.", styl);
                    }

                    if (magioObj.vfxSpawnerType == VFXSpawnerType.SkinnedMesh)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(slowAnimationOnEffectStart);

                        if (magioObj.slowAnimationOnEffectStart)
                        {
                            EditorGUILayout.PropertyField(animator);
                            EditorGUILayout.PropertyField(animationSlowDownSpeed);
                            EditorGUILayout.PropertyField(animationSlowDownTargetValue);
                        }
                    }

                    EditorGUILayout.Space();
                    bool eventInvokers = false;

                    foreach (Object magioO in targets)
                    {
                        MagioObjectEffect eff = magioO as MagioObjectEffect;
                        MagioEventInvoker debug = eff.GetComponent<MagioEventInvoker>();
                        if (debug)
                        {
                            eventInvokers = true;
                        }
                    }

                    if (!eventInvokers)
                    {
                        if (GUILayout.Button("Enable Event Hooks"))
                        {
                            foreach (Object magioO in targets)
                            {
                                MagioObjectEffect eff = magioO as MagioObjectEffect;
                                Undo.AddComponent<MagioEventInvoker>(eff.gameObject);
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Disable Event Hooks"))
                        {
                            foreach (Object magioO in targets)
                            {
                                MagioObjectEffect eff = magioO as MagioObjectEffect;
                                MagioEventInvoker eventInvoker = eff.GetComponent<MagioEventInvoker>();
                                if (eventInvoker)
                                {
                                    DestroyImmediate(eventInvoker);
                                }
                            }

                        }
                    }

                    break;
                case 5:
                    EditorGUILayout.LabelField("Errors with gameobjects:", EditorStyles.boldLabel);
                    EditorGUILayout.Space();

                    if (errors.Count > 0)
                    {
                        foreach (string error in errors)
                        {
                            GUIStyle styl = new GUIStyle(EditorStyles.textField);
                            styl.normal.textColor = Color.yellow;
                            styl.wordWrap = true;
                            EditorGUILayout.LabelField(error, styl);
                        }
                    }
                    else
                    {
                        GUIStyle styl = new GUIStyle(EditorStyles.textField);
                        styl.normal.textColor = Color.green;
                        styl.wordWrap = true;
                        EditorGUILayout.LabelField("No errors.", styl);
                    }

                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void DrawTypeDropDown()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(vfxSpawnerType);

            List<string> effectPacks = new List<string>();

            int selectedIndex = -1;
            foreach (MagioEffectPack pack in MagioEngine.instance.effectPacks)
            {
                effectPacks.Add(SeparateCamelCase(pack.name));
            }

            effectPacks.Add("Select effect prefab...");

            if (effectPackNumber.intValue < 0)
            {
                selectedIndex = effectPacks.Count - 1;
            }
            else
            {
                selectedIndex = effectPackNumber.intValue;
            }


            EditorGUI.showMixedValue = effectPackNumber.hasMultipleDifferentValues;
            int newEffectPackValue = EditorGUILayout.Popup("Effect Pack", selectedIndex, effectPacks.ToArray());
            EditorGUI.showMixedValue = false;
            GameObject newEffectPrefab = null;
            int newEffectValue = 0;

            if (newEffectPackValue > -1 && newEffectPackValue < MagioEngine.instance.effectPacks.Count)
            {
                List<string> effects = new List<string>();

                if (magioObj.vfxSpawnerType == VFXSpawnerType.Mesh)
                {
                    foreach (GameObject eff in MagioEngine.instance.effectPacks[newEffectPackValue].meshEffects)
                    {
                        string effName = string.Join("", eff.name.Split('_').Skip(1));
                        effects.Add(SeparateCamelCase(effName));
                    }
                }
                else
                {
                    foreach (GameObject eff in MagioEngine.instance.effectPacks[newEffectPackValue].skinnedMeshEffects)
                    {
                        string effName = string.Join("", eff.name.Split('_').Skip(1));
                        effects.Add(SeparateCamelCase(effName));
                    }
                }

                if (effects.Count > 0 && !effectPackNumber.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = effectNumber.hasMultipleDifferentValues || effectPackNumber.hasMultipleDifferentValues;
                    newEffectValue = EditorGUILayout.Popup("Effect", effectNumber.intValue, effects.ToArray());
                    EditorGUI.showMixedValue = false;
                }
            }
            else
            {
                EditorGUI.showMixedValue = _effectPrefab.hasMultipleDifferentValues;
                newEffectPrefab = (GameObject)EditorGUILayout.ObjectField("Effect Prefab", _effectPrefab.objectReferenceValue, typeof(GameObject), true);
                EditorGUI.showMixedValue = false;
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (MagioEngine.instance.effectPacks.Count > newEffectPackValue)
                {
                    effectPackNumber.intValue = newEffectPackValue;
                }
                else
                {
                    effectPackNumber.intValue = -1;
                }

                effectNumber.intValue = newEffectValue;

                if (newEffectPrefab != null)
                {
                    foreach (Object magioO in targets)
                    {
                        MagioObjectEffect eff = magioO as MagioObjectEffect;
                        eff.EffectPrefab = newEffectPrefab;
                    }
                }

                effectChange = true;

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            foreach (Object magioO in targets)
            {
                MagioObjectEffect eff = magioO as MagioObjectEffect;

                if (MagioEngine.instance.effectPacks.Count > eff.effectPackNumber && eff.effectPackNumber > -1)
                {
                    if (eff.vfxSpawnerType == VFXSpawnerType.Mesh)
                    {
                        if (MagioEngine.instance.effectPacks[eff.effectPackNumber].meshEffects.Count > eff.effectNumber)
                        {
                            eff.EffectPrefab = MagioEngine.instance.effectPacks[eff.effectPackNumber].meshEffects[eff.effectNumber];
                        }
                        else
                        {
                            eff.effectNumber = 0;
                            eff.EffectPrefab = MagioEngine.instance.effectPacks[eff.effectPackNumber].meshEffects[0];
                        }
                    }
                    else
                    {
                        if (MagioEngine.instance.effectPacks[eff.effectPackNumber].skinnedMeshEffects.Count > eff.effectNumber)
                        {
                            eff.EffectPrefab = MagioEngine.instance.effectPacks[eff.effectPackNumber].skinnedMeshEffects[eff.effectNumber];
                        }
                        else
                        {
                            eff.effectNumber = 0;
                            eff.EffectPrefab = MagioEngine.instance.effectPacks[eff.effectPackNumber].skinnedMeshEffects[0];
                        }
                    }
                }
            }



            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset VFX Parameters"))
            {
                foreach (Object magioO in targets)
                {
                    MagioObjectEffect eff = magioO as MagioObjectEffect;
                    Undo.RecordObject(eff, "Reset the VFX parameters");
                    eff.RefreshAndResetVFXProperties();
                }
            }
            EditorGUILayout.EndHorizontal();


        }

        public string SeparateCamelCase(string source)
        {
            return string.Join(" ", Regex.Split(source, @"(?<!^)(?=[A-Z](?![A-Z]|$))")); ;
        }

        public void CheckForErrors()
        {
            List<string> newErrors = new List<string>();
            foreach (Object magioO in targets)
            {
                SkinnedMeshRenderer[] renderers;
                List<MeshFilter> filters;
                MagioObjectEffect eff = magioO as MagioObjectEffect;

                if (!eff.useEffectOnAllRenderers && eff.magioRenderers != null)
                {
                    renderers = eff.magioRenderers.OfType<SkinnedMeshRenderer>().ToArray();
                    List<MeshFilter> filterList = new List<MeshFilter>();
                    foreach (Renderer rend in eff.magioRenderers)
                    {
                        if (!rend) continue;
                        MeshFilter fil = rend.GetComponent<MeshFilter>();
                        if (fil) filterList.Add(fil);
                    }

                    filters = filterList;
                }
                else
                {
                    filters = eff.targetGameObject.GetComponentsInChildren<MeshFilter>().ToList();
                    renderers = eff.targetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                }

                if (eff.vfxSpawnerType == VFXSpawnerType.Mesh)
                {
                    foreach (MeshFilter filter in filters)
                    {
                        if (!filter || !filter.sharedMesh) continue;
                        if (!filter.sharedMesh.isReadable)
                        {
                            newErrors.Add("Read/Write is not enabled! Mesh: " + filter.sharedMesh.name + " in gameobject: " + filter.gameObject.name);
                        }

                        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(filter.gameObject);
                        if ((flags & StaticEditorFlags.BatchingStatic) != 0)
                        {
                            newErrors.Add("Static batching is enabled! Please disable static batching in gameobject: " + filter.gameObject.name);
                        }
                    }
                }
                else
                {
                    foreach (SkinnedMeshRenderer rend in renderers)
                    {
                        if (!rend.sharedMesh.isReadable)
                        {
                            newErrors.Add("Read/Write is not enabled! Mesh: " + rend.sharedMesh.name + " in gameobject: " + rend.gameObject.name);
                        }

                        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(rend.gameObject);
                        if ((flags & StaticEditorFlags.BatchingStatic) != 0)
                        {
                            newErrors.Add("Static batching is enabled! Please disable static batching in gameobject: " + rend.gameObject.name);
                        }
                    }
                }

                if (filters.Count == 0 && eff.vfxSpawnerType == VFXSpawnerType.Mesh)
                {
                    newErrors.Add("VFX spawner type is Mesh and there is no Mesh filters in: " + eff.gameObject.name);
                }

                if (renderers.Length == 0 && eff.vfxSpawnerType == VFXSpawnerType.SkinnedMesh)
                {
                    newErrors.Add("VFX spawner type is Skinned Mesh and there is no Skinned Mesh renderer in: " + eff.gameObject.name);
                }

                if (eff.effectBehaviourType == EffectBehaviourMode.Spread && eff.targetGameObject.GetComponentsInChildren<Collider>().Length == 0 && !eff.beginEffectOnStart)
                {
                    newErrors.Add("Effect mode is Spread and not beginning on start and there is no colliders to catch other effects: " + eff.gameObject.name);
                }
            }

            errors = newErrors;
        }

        public void ConstructDynamicListOfParameters()
        {
            // Update check
            if (Event.current.type == EventType.Repaint || effectChange)
            {
                foreach (Object magioO in targets)
                {
                    MagioObjectEffect eff = magioO as MagioObjectEffect;
                    List<VFXExposedProperty> exposedProperties = new List<VFXExposedProperty>();
                    VisualEffect visEff = eff.EffectPrefab.GetComponent<VisualEffect>();
                    visEff.visualEffectAsset.GetExposedProperties(exposedProperties);
                    if (exposedProperties.Count != eff.propertyCount)
                    {
                        eff.UpdateVFXProperties();
                    }
                }

                effectChange = false;
            }

            if (magioObj.vfxProperties.Count != vfxProperties.arraySize)
            {
                Selection.objects = new[] { ((MagioObjectEffect)target).gameObject };
                EditorApplication.delayCall += () => Selection.objects = System.Array.ConvertAll(targets, target => ((MagioObjectEffect)target).gameObject);
            }

            if (magioObj.vfxProperties != null && vfxProperties.arraySize > 0 && magioObj.vfxProperties.Count == vfxProperties.arraySize)
            {
                string currentHeader = "";
                for (int i = 0; i < vfxProperties.arraySize; i++)
                {
                    MagioObjectEffect.VFXProps prop = magioObj.vfxProperties[i];

                    SerializedProperty element = vfxProperties.GetArrayElementAtIndex(i);

                    string[] name = System.Text.RegularExpressions.Regex.Split(prop.name, @"(?<!^)(?=[A-Z](?![A-Z]|$))");

                    if (currentHeader.Equals(string.Empty) || !currentHeader.Equals(name[0]))
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(name[0], EditorStyles.boldLabel);
                        currentHeader = name[0];
                    }

                    if (prop.type == VFXParameterType.ANIMATION_CURVE)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("animationCurveValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.BOOL)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("boolValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.FLOAT)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("floatValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.GRADIENT)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("gradientValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.INT)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("intValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.MATRIX)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("matrixValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.MESH)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("meshValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.SKINNED_MESH_RENDERER)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("skinnedMeshRendererValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.TEXTURE)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("textureValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.UINT)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("uintValue"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.VECTOR2)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("vector2Value"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.VECTOR3)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("vector3Value"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.VECTOR4)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("vector4Value"), new GUIContent(prop.name));
                    }
                    else if (prop.type == VFXParameterType.COLOR)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("colorValue"), new GUIContent(prop.name));
                    }
                }

            }



        }
    }
}
