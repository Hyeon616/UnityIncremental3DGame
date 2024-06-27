using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Magio
{
    /// <summary>
    /// Represents one attached effect on object (flame, plants, freeze etc.)
    /// </summary>
    public class MagioObjectEffect : MonoBehaviour
    {
        [System.Serializable]
        public struct VFXProps
        {
            public string name;
            public VFXParameterType type;
            public AnimationCurve animationCurveValue;
            public bool boolValue;
            public float floatValue;
            public Gradient gradientValue;
            public int intValue;
            public Matrix4x4 matrixValue;
            public Mesh meshValue;
            public SkinnedMeshRenderer skinnedMeshRendererValue;
            public Texture textureValue;
            public uint uintValue;
            public Vector2 vector2Value;
            public Vector3 vector3Value;
            public Vector4 vector4Value;

            [ColorUsage(true, true)]
            public Color colorValue;
        }

        [System.Serializable]
        public struct MagioEffectObject
        {
            public Transform boundObject;
            public SkinnedMeshRenderer skinnedMeshRenderer;
            public VisualEffect magioEffect;
            public float size;
        }

        public enum CalculationArea
        {
            None,
            Object,
            Vegetation
        }

        public enum CanBeReanimated
        {
            No,
            Only_After_Nullify,
            Always
        }

        public enum MagioShaderEffectMode
        {
            Emission_Overlay,
            Dissolve,
            Texture_Override
        }

        public enum SFXMode
        {
            WhenEffectIsOn,
            WhenEffectIsSpreading
        }

        [Tooltip("Use the effect on gameobject attached to this script")]
        public bool useOnThisGameObject = true;

        [Tooltip("Target gameobject for the effect")]
        public GameObject targetGameObject;

        [Tooltip("Sets whether effect is enabled fully always when MagioObject script is activated or is it spread naturally.")]
        public EffectBehaviourMode effectBehaviourType = EffectBehaviourMode.Spread;

        [Tooltip("Is effect spread to other objects?")]
        public bool spreadToOtherObjects = true;

        [Tooltip("Only objects within same effect class can spread the effect to each other.")]
        public EffectClass effectClass = EffectClass.Default;

        [Tooltip("Spread layer mask. Only check for spreading in this layer.")]
        public LayerMask spreadLayerMask = ~0;

        [Tooltip("Colliders which spread the effect.")]
        public List<Collider> spreadingColliders = new List<Collider>();

        [Tooltip("Do you want to use all renderers for the effect? If set to false you can exclude some renderers from the effect")]
        public bool useEffectOnAllRenderers = true;

        [Tooltip("Renderers to use for effect")]
        public List<Renderer> magioRenderers = new List<Renderer>();

        [Tooltip("Do you want to use all colliders to spread the effect?")]
        public bool useAllCollidersToSpread = true;

        [Tooltip("If you want to set custom effect origin on the start assign this to transform. Otherwise it will use transform.position")]
        public Transform customStartOrigin;

        [Min(0)]
        [Tooltip("How long will the ignition take to spread effect from other objects? (Seconds touching one other effect), If there are two effects this is halved and so on.")]
        public float ignitionTime = 0;

        [Tooltip("Is this object's effect affected by wind?")]
        public bool affectedByWind = true;

        [Tooltip("Added external velocity to particles. Use this to simulate magic effects, impacts, speed etc.")]
        public Vector3 addedExternalVelocity = new Vector3();

        [Tooltip("Can object be reanimated after fadeout?")]
        public MagioObjectEffect.CanBeReanimated canBeReAnimated = MagioObjectEffect.CanBeReanimated.Only_After_Nullify;

        [Tooltip("Is effect on forever or does it fade out after some time.")]
        public bool effectOnForever = false;

        [Min(0)]
        [Tooltip("When effect starts to fade out. (Seconds)")]
        public float fadeOutStart_s = 30;

        [Min(0)]
        [Tooltip("How long the transition will take. (Seconds). Note: Object can be still faded out after nullify even if effectOnForever == True")]
        public float fadeOutLength_s = 10;

        [Range(-5, 5)]
        [Tooltip("How fast does the effect move (time... m / s). Set this to 0 to stop the automatic spread and effectEnabled timer.")]
        public float effectCrawlSpeed = 1f;

        [Range(0.1f, 10000f)]
        [Tooltip("How far can effect spread (m)")]
        public float maxSpread = 10000f;

        [Tooltip("Enables Material animation if your shader has support created for it.")]
        public bool enableMaterialAnimation = true;

        [Tooltip("Shader emission color")]
        public Color shaderEmissionColor = new Color(241f / 255f, 121f / 255f, 11f / 255, 1f);

        [Tooltip("Is VFX emit enabled?")]
        public bool enableVFX = true;

        [Tooltip("How fast shader changes to the burnt color? Lerp percentage multiplier.")]
        [Range(0, 10)]
        public float shaderToEffectEndInterpolateSpeed = 0.03f;

        [Min(0)]
        [Tooltip("How strong is emission in the shader")]
        public float shaderEmissionMultiplier = 1;

        [Min(0)]
        [Tooltip("How long does it to reach max brightness in material shader. Maximum value = FadeoutStart_s (Seconds)")]
        public float achieveEmissionColorTime_s = 5;

        [Range(0, 1f)]
        [Tooltip("Shader Color noise multiplier min max")]
        public float shaderColorNoise = 0.05f;

        [Range(0, 10)]
        [Tooltip("Shader Color noise change speed.")]
        public float shaderColorNoiseSpeed = 1;

        [Tooltip("How Magio shaders behave. Emission overlay = Overlay emission with color with some noise. Dissolve = Dissolves the mesh alongside with spread.")]
        public MagioShaderEffectMode magioShaderEffectMode = MagioShaderEffectMode.Emission_Overlay;

        [Tooltip("How big the emission edge for dissolve is?")]
        public float dissolveEmissionEdgeWidth = 0.2f;

        [Tooltip("Override object texture with this when effect has spread")]
        public Texture2D overrideAlbedoMap;

        [Tooltip("Override object texture normal with this when effect has spread")]
        public Texture2D overrideNormalMap;

        [Tooltip("Override object texture tiling")]
        public Vector2 overrideTextureTiling = new Vector2(1, 1);

        [Tooltip("Override object texture offset")]
        public Vector2 overrideTextureOffset;

        [Tooltip("Which color to multiply the texture with")]
        public Color overrideColor = Color.white;

        [Range(0f, 1f)]
        [Tooltip("How much strength the override texture has? 1 = Full override, 0 = no override")]
        public float overrideStrength = 1.0f;

        [Range(0f, 1f)]
        [Tooltip("How much wetness you want to add with override? 1 = Add full wetness, 0 = no added wetness")]
        public float overrideAddWetness = 0.0f;

        [Tooltip("Do you want to lerp the texture to something else. (e.g. burnt with fire).")]
        public bool blendToOtherTexture = false;

        [Tooltip("Blend object texture with this when effect has spread")]
        public Texture2D blendAlbedoMap;

        [Tooltip("Blend object texture normal with this when effect has spread")]
        public Texture2D blendNormalMap;

        [Tooltip("Blend object texture normal map strength")]
        public float blendTextureNormalStrength = 1;

        [Tooltip("Blend object texture tiling")]
        public Vector2 blendTextureTiling = new Vector2(1, 1);

        [Tooltip("How large area should be added to collider for effect to spread (Meters)")]
        public Vector3 effectSpreadAreaAddition = new Vector3(0.5f, 0.5f, 0.5f);

        [Tooltip("Begins effect on Start()")]
        public bool beginEffectOnStart = false;

        [Tooltip("How far has the effect spread already")]
        public float effectSpread = 0;

        [Min(0)]
        [Tooltip("How long has the effect been enabled? (Seconds)")]
        public float effectEnabledTimer = 0;

        [HideInInspector]
        public bool effectEnabled = false;

        [Tooltip("Do you want to add SFX for this effect?")]
        public bool addSFXRuntime = false;

        [Range(0f, 1f)]
        [Tooltip("How much pitch is randomized for the sfx? This is done to avoid phasing in case of same sound overlapping")]
        public float sfxPitchRandomization = 0.3f;

        [Min(0)]
        [Tooltip("Randomizes sfx loop progress time from 0 to sfxStartTimeRandomizationMax")]
        public float sfxStartTimeRandomizationMax = 10;

        [Tooltip("Whether to play the sfx when the effect is on or only when spreading.")]
        public SFXMode sfxMode = SFXMode.WhenEffectIsOn;

        [Tooltip("Prefab for the effect sound to be added when the object is ignited.")]
        public GameObject effectSFX;

        [Tooltip("Attach external audiosources here to enable them linearly when Magio effect starts.")]
        public List<AudioSource> allEffectSFX = new List<AudioSource>();

        [Range(0.0f, 1.0f)]
        [Tooltip("How hard is it to fully nullify this effect? 0 = one drop will nullify the whole object, 1 = object needs to be fully nullified before it fade out. Starts the fade out when: nullify diameter > (object approx size * fullyNullifyToughness)")]
        public float fullNullifyToughness = 0.7f;

        [Min(0.01f)]
        [Tooltip("Seconds before effect starts to spread back after nullify attempt.")]
        public float backSpreadCoolDown_s = 5;

        [Tooltip("Is the VFX spawned to the mesh or skinned mesh renderer.")]
        public VFXSpawnerType vfxSpawnerType = VFXSpawnerType.Mesh;

        [Tooltip("Do you want to slow animation down when effect is enabled?")]
        public bool slowAnimationOnEffectStart = false;

        [Tooltip("Animator to slow down.")]
        public Animator animator;

        [Tooltip("If you want to slow the animation down after the effect is enabled.")]
        public float animationSlowDownSpeed = 0.2f;

        [Tooltip("To what value do you want to slow the animation down to?")]
        public float animationSlowDownTargetValue = 0;

        [Tooltip("When spread > approximinate size - delete object. Useful for dissolve effects")]
        public bool deleteObjectAfterFullSpread = false;

        [Tooltip("If this is enabled, object can stack multiple effects of same prefab. Only enable this if you intend to use this as splash effect.")]
        public bool isSplashEffect = false;

        [Tooltip("Deletes Gameobject after fade out. Use this to optimize multi-effect-objects if the object cannot be reanimated etc.")]
        public bool deleteObjectAfterFadeOut = false;



        [Tooltip("Prefab containing at least the vfx.")]
        [SerializeField]
        private GameObject _effectPrefab;

        /// <summary>
        /// Current effect prefab in use
        /// </summary>
        public GameObject EffectPrefab
        {
            get
            {
                return _effectPrefab;
            }
            set
            {
                if (value == _effectPrefab) return;
                if (value == null)
                {
                    _effectPrefab = null;
                    vfxProperties.Clear();
                    return;
                }
                VisualEffect visEff = value.GetComponent<VisualEffect>();
                if (visEff)
                {
                    vfxProperties = VFXHelpers.GetNonMagioVFXProperties(visEff);
                    List<VFXExposedProperty> properties = new List<VFXExposedProperty>();
                    visEff.visualEffectAsset.GetExposedProperties(properties);

                    propertyCount = properties.Count;

                    MagioPrefabDefaults defaults = value.GetComponent<MagioPrefabDefaults>();
                    if (defaults)
                    {
                        effectClass = defaults.effectClass;
                        shaderEmissionColor = defaults.shaderEmissionColor;
                        shaderColorNoise = defaults.shaderColorNoise;
                        shaderColorNoiseSpeed = defaults.shaderColorNoiseSpeed;
                        shaderEmissionMultiplier = defaults.shaderEmissionMultiplier;
                        shaderToEffectEndInterpolateSpeed = defaults.shaderToEffectEndInterpolateSpeed;
                        achieveEmissionColorTime_s = defaults.achieveEmissionColorTime_s;
                        magioShaderEffectMode = defaults.magioShaderEffectMode;
                        effectSFX = defaults.effectSFX;
                        enableMaterialAnimation = defaults.enableMaterialAnimation;
                        slowAnimationOnEffectStart = defaults.slowAnimationOnEffectStart;
                        animationSlowDownSpeed = defaults.animationSlowDownSpeed;
                        animationSlowDownTargetValue = defaults.animationSlowDownTargetValue;
                        overrideAlbedoMap = defaults.overrideAlbedoMap;
                        overrideNormalMap = defaults.overrideNormalMap;
                        overrideTextureOffset = defaults.overrideTextureOffset;
                        overrideTextureTiling = defaults.overrideTextureTiling;
                        blendToOtherTexture = defaults.blendToOtherTexture;
                        blendAlbedoMap = defaults.blendAlbedoMap;
                        blendNormalMap = defaults.blendNormalMap;
                        blendTextureNormalStrength = defaults.blendTextureNormalStrength;
                        blendTextureTiling = defaults.blendTextureTiling;
                        overrideStrength = defaults.overrideStrength;
                        overrideAddWetness = defaults.overrideAddWetness;
                        overrideColor = defaults.overrideColor;
                    }
                }
                else
                {
                    Debug.LogWarning("Assigned effect prefab gameobject does not have Visual Effect.");
                    vfxProperties.Clear();
                    propertyCount = 0;
                }

                _effectPrefab = value;
            }
        }

        public List<VFXProps> vfxProperties = new List<VFXProps>();

        public int openTabUpper = 0;
        public int openTabLower = 0;

        public MagioEffectPack magioEffectPack = null;
        public int effectPackNumber = 0;
        public int effectNumber = 0;
        public int propertyCount = 0;

        private List<MagioEffectObject> magioEffects = new List<MagioEffectObject>();
        private List<SpreadPhysics> spreadPhysics = new List<SpreadPhysics>();

        private MagioEventInvoker magioEventInvoker;

        private float currentAnimateProgress_s = 0;
        private float currentAnimateCoolingCooldown_s = 0;
        private float ignitionCoolingCooldown_s = 1;
        private bool reAnimated = false;
        private Vector3 _effectSpreadOriginLocal;

        private float nullifyRadius = 0;
        private Vector3 nullifyAreaCenter = Vector3.zero;
        private float curBackSpreadCoolDown_s = 0;

        private AudioSource curAudio;
        private bool nullified = false;
        private bool firstSetup = false;
        private bool effectEnded = false;
        private float originalAnimationSpeed = 1;
        private MaterialPropertySaver saver;
        private MagioObjectMaster objectMaster;
        private float materialAnimationTimer = 0;
        private float magioMaterialAnimationTimer = 0;
        private float originalFadeOutStart = 30;
        private float originalFadeOutLength = 10;
        private Renderer[] rends;
        private bool firstMagioShaderSetup = false;


        private void Awake()
        {
            effectEnabled = false;
            firstSetup = false;
            effectEnded = false;
            materialAnimationTimer = 0;
        }

        private void Start()
        {
            Setup();
            BeginEffectIfNecessary();
        }

        void OnEnable()
        {
            if (firstSetup)
            {
                Setup();
                BeginEffectIfNecessary();
            }

        }

        /// <summary>
        /// Initial setup. Call this if you need to change target gameobject runtime etc.
        /// </summary>
        public void Setup()
        {
            if (useOnThisGameObject)
            {
                targetGameObject = this.gameObject;
            }

            magioEventInvoker = gameObject.GetComponent<MagioEventInvoker>();

            objectMaster = targetGameObject.GetComponent<MagioObjectMaster>();
            if (!objectMaster)
            {
                objectMaster = targetGameObject.AddComponent<MagioObjectMaster>();
            }

            objectMaster.AddMagioObject(this);

            SaveOriginalMaterialShaderProperties();

            originalFadeOutStart = fadeOutStart_s;
            originalFadeOutLength = fadeOutLength_s;

            achieveEmissionColorTime_s = Mathf.Min(achieveEmissionColorTime_s, fadeOutStart_s);
            if (slowAnimationOnEffectStart)
            {
                if (animator)
                {
                    originalAnimationSpeed = animator.speed;
                }
                else
                {
                    animator = targetGameObject.GetComponent<Animator>();
                    if (animator)
                    {
                        originalAnimationSpeed = animator.speed;
                    }
                    else
                    {
                        Debug.LogWarning("No animator attached to Magio Object. Cannot slow down animation. Setting: Advanced->Slow Down Animation.");
                    }
                }
            }

            if (useEffectOnAllRenderers)
            {
                magioRenderers = targetGameObject.GetComponentsInChildren<Renderer>(true).ToList();
            }

            rends = targetGameObject.GetComponentsInChildren<Renderer>();

            if (!_effectPrefab && effectPackNumber > -1 && effectPackNumber < MagioEngine.instance.effectPacks.Count)
            {
                if (vfxSpawnerType == VFXSpawnerType.Mesh)
                {
                    EffectPrefab = MagioEngine.instance.effectPacks[effectPackNumber].meshEffects[effectNumber];
                }
                else
                {
                    EffectPrefab = MagioEngine.instance.effectPacks[effectPackNumber].skinnedMeshEffects[effectNumber];
                }
            }

            if (effectBehaviourType == EffectBehaviourMode.Enable)
            {
                effectSpread = float.MaxValue;
                ignitionTime = 0;
                beginEffectOnStart = true;
            }
            else
            {
                if (useAllCollidersToSpread)
                {
                    spreadingColliders = targetGameObject.GetComponentsInChildren<Collider>().ToList();
                }
            }

            if (effectOnForever)
            {
                fadeOutStart_s = float.MaxValue;
            }

            firstSetup = true;
        }

        /// <summary>
        /// Begins the effect if beginEffectOnStart = true;
        /// </summary>
        public void BeginEffectIfNecessary()
        {
            if (beginEffectOnStart)
            {
                currentAnimateProgress_s = ignitionTime + 1;

                if (!customStartOrigin)
                {
                    TryToAnimateEffect(targetGameObject.transform.position, 1);
                }
                else
                {
                    TryToAnimateEffect(customStartOrigin.position, 1);
                }
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                foreach (MagioEffectObject eff in magioEffects)
                {
                    if (eff.magioEffect)
                    {
                        eff.magioEffect.SetFloat("Magio_LifelineMultiplier", 0);
                    }
                }
                ResetObj();
                nullified = false;
                ResetMaterialFromMagio();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!effectEnded && !MagioEngine.instance.pause)
            {
                if (currentAnimateCoolingCooldown_s > 0)
                {
                    currentAnimateCoolingCooldown_s -= Time.deltaTime;
                }
                else if (currentAnimateProgress_s > 0 && currentAnimateCoolingCooldown_s <= 0)
                {
                    currentAnimateProgress_s -= Time.deltaTime;
                }
                else if (currentAnimateProgress_s < 0)
                {
                    currentAnimateProgress_s = 0;
                }

                if (effectEnabled)
                {
                    effectEnabledTimer += Time.deltaTime;

                    //Stop the spread at some point
                    if (effectSpread + (effectCrawlSpeed * Time.deltaTime) < maxSpread)
                    {
                        effectSpread += effectCrawlSpeed * Time.deltaTime;

                        effectSpread = Mathf.Max(effectSpread, 0);
                    }
                    else
                    {
                        effectSpread = maxSpread;
                    }

                    if (nullifyRadius > 0 && curBackSpreadCoolDown_s < 0)
                    {
                        nullifyRadius -= Time.deltaTime * effectSpread * 0.01f;

                    }
                    else if (curBackSpreadCoolDown_s > 0 && nullifyRadius > 0)
                    {
                        curBackSpreadCoolDown_s -= Time.deltaTime;
                    }

                    UpdateShaders();

                    if (enableVFX)
                    {
                        UpdateVFX(magioEffects, effectSpread, _effectSpreadOriginLocal, objectMaster.ApproxSize);
                    }


                    UpdateSFX();

                    if (effectEnabledTimer > fadeOutStart_s + fadeOutLength_s && !effectEnded)
                    {
                        effectEnded = true;
                        effectEnabled = false;
                        if (magioEventInvoker) magioEventInvoker.FadedOut.Invoke();
                        CheckForRemove();
                    }

                    if (slowAnimationOnEffectStart && animator)
                    {
                        if (effectEnabledTimer < fadeOutStart_s)
                        {
                            animator.speed = Mathf.MoveTowards(animator.speed, animationSlowDownTargetValue, animationSlowDownSpeed * Time.deltaTime);
                        }
                        else
                        {
                            animator.speed = Mathf.MoveTowards(animator.speed, originalAnimationSpeed, animationSlowDownSpeed * Time.deltaTime);
                        }
                    }

                    CheckForObjectDelete();
                }
            }
        }

        /// <summary>
        /// Checks if the object should be deleted.
        /// </summary>
        public void CheckForObjectDelete()
        {
            if (deleteObjectAfterFullSpread)
            {
                float distFromCenter = Vector3.Distance(objectMaster.ApproxCenter, targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));

                float percentageFromSize = distFromCenter / objectMaster.ApproxSize;
                if (effectSpread > (objectMaster.ApproxSize * (0.5f + percentageFromSize)) + 0.5f)
                {
                    foreach (MagioEffectObject eff in magioEffects)
                    {
                        eff.magioEffect.gameObject.AddComponent<SelfDestroy>();
                    }

                    magioEffects.Clear();

                    Destroy(targetGameObject);
                }

            }
        }

        /// <summary>
        /// Saves the original shader/material properties which are to be animated by Magio.
        /// </summary>
        public void SaveOriginalMaterialShaderProperties()
        {
            saver = targetGameObject.GetComponent<MaterialPropertySaver>();

            if (!saver)
            {
                saver = targetGameObject.AddComponent<MaterialPropertySaver>();
            }
        }

        /// <summary>
        /// Resets the material values which were animated by Magio to the originals. Does not reset other material shader properties.
        /// </summary>
        public void ResetMaterialFromMagio()
        {
            if (saver)
                saver.ResetMaterialFromMagio();
        }

        /// <summary>
        /// Resets flammable object values. Does not reset the shader. Please Call ResetMaterialFromIgnis() to reset the shader.
        /// </summary>
        public void ResetObj()
        {
            fadeOutStart_s = originalFadeOutStart;
            fadeOutLength_s = originalFadeOutLength;
            effectEnabled = false;
            firstMagioShaderSetup = false;
            effectEnabledTimer = 0;
            materialAnimationTimer = 0;
            effectSpread = 0;
            nullifyRadius = 0;
            currentAnimateProgress_s = 0;
            nullifyAreaCenter = Vector3.zero;
            if (animator && slowAnimationOnEffectStart)
            {
                animator.speed = originalAnimationSpeed;
            }
            foreach (MagioEffectObject eff in magioEffects)
            {
                if (eff.magioEffect)
                {
                    eff.magioEffect.gameObject.AddComponent<SelfDestroy>();
                }
            }
            foreach (SpreadPhysics trigger in spreadPhysics)
            {
                if (trigger)
                    Destroy(trigger);
            }

            magioEffects.Clear();
            spreadPhysics.Clear();

            SpreadNullify nullif = GetComponent<SpreadNullify>();
            if (nullif)
            {
                Destroy(nullif);
            }
        }

        /// <summary>
        /// Animates the object from the center of transform.
        /// </summary>
        public void AnimateFromCenter()
        {
            currentAnimateProgress_s = ignitionTime + 1;
            TryToAnimateEffect(targetGameObject.transform.position, 1);
        }

        /// <summary>
        /// Tries to animate the object. (Ignition time, canBeReanimated etc. matters)
        /// </summary>
        /// <param name="effectSpreadOrigin">Where should the effect spread.</param>
        /// <param name="addToAnimateProgress">How much the ignition time/Progress should be added</param>
        public void TryToAnimateEffect(Vector3 effectSpreadOrigin, float addToAnimateProgress)
        {
            if ((!enabled || !gameObject.activeInHierarchy) && effectBehaviourType != EffectBehaviourMode.Enable) return;

            if (effectEnabled)
            {
                if (effectEnabledTimer > fadeOutStart_s)
                {
                    if (canBeReAnimated == CanBeReanimated.Always || (canBeReAnimated == CanBeReanimated.Only_After_Nullify && nullified && !GetComponent<SpreadNullify>()))
                    {
                        nullified = false;
                        effectEnabledTimer = 0;
                        reAnimated = true;
                    }
                }

                return;
            }

            if (!effectEnded || canBeReAnimated == CanBeReanimated.Always || (canBeReAnimated == CanBeReanimated.Only_After_Nullify && nullified))
            {
                // Ignition conditions
                currentAnimateCoolingCooldown_s = ignitionCoolingCooldown_s;
                currentAnimateProgress_s += addToAnimateProgress;

                if (currentAnimateProgress_s < ignitionTime)
                {
                    return;
                }

                // Other effect conditions
                if (!objectMaster.CanEffectEnable(this)) return;

                if (magioEventInvoker) magioEventInvoker.Ignited.Invoke();

                effectEnabled = true;
                effectEnded = false;
                nullified = false;
                _effectSpreadOriginLocal = targetGameObject.transform.InverseTransformPoint(effectSpreadOrigin);

                InvokeCallbacks();

                // Shader setup
                SetupShaders();

                if (enableVFX)
                {
                    SetupEffect();
                }


                foreach (AudioSource source in allEffectSFX)
                {
                    source.enabled = true;
                    source.gameObject.SetActive(true);
                }

                objectMaster.HandleNewEffectEnable(this);
            }
        }

        /// <summary>
        /// Incrementally nullifies the effect from position.
        /// </summary>
        /// <param name="position">Current nullify position</param>
        /// <param name="startRadius">Radius of the nullify</param>
        /// <param name="radiusIncrement">Radius incremented in every call, if new position is not added</param>
        public void IncrementalNullify(Vector3 position, float startRadius, float radiusIncrement)
        {
            if (!effectEnabled)
            {
                return;
            }

            if (magioEventInvoker) magioEventInvoker.BeingNullified.Invoke();

            curBackSpreadCoolDown_s = backSpreadCoolDown_s;

            Vector3 localPos = targetGameObject.transform.InverseTransformPoint(position);

            if (nullifyRadius < 0.05f || (Vector3.Distance(nullifyAreaCenter, localPos) - nullifyRadius) > startRadius * 3)
            {
                nullifyAreaCenter = localPos;
                nullifyRadius = startRadius;
            }
            else
            {
                if (Vector3.Distance(localPos, nullifyAreaCenter) > (nullifyRadius - startRadius))
                {
                    Vector3 prevCenter = nullifyAreaCenter;
                    float distanceToPos = Vector3.Distance(nullifyAreaCenter, localPos);
                    nullifyAreaCenter = Vector3.Lerp(nullifyAreaCenter, Vector3.MoveTowards(nullifyAreaCenter, localPos, (distanceToPos + startRadius - nullifyRadius)), 0.2f);
                    nullifyRadius += (Vector3.Distance(prevCenter, nullifyAreaCenter));
                }
                else
                {
                    nullifyRadius += radiusIncrement;
                }
            }

            if (magioEffects.Count > 0)
            {
                if (magioEffects[0].magioEffect.HasVector3("Magio_NullifyEffectPos"))
                {
                    magioEffects[0].magioEffect.SetVector3("Magio_NullifyEffectPos", position);
                    magioEffects[0].magioEffect.SendEvent("Nullify");
                }
            }

            CheckForFadeOut(radiusIncrement);
        }

        /// <summary>
        /// Sets the nullify area.
        /// </summary>
        /// <param name="center">Center of the area</param>
        /// <param name="radius">Radius of the area</param>
        public void SetNullifyArea(Vector3 center, float radius)
        {
            nullifyAreaCenter = targetGameObject.transform.InverseTransformPoint(center);
            nullifyRadius = radius;

            curBackSpreadCoolDown_s = backSpreadCoolDown_s;

            CheckForFadeOut(0);
        }


        /// <summary>
        /// Updates all the effects according to parameters on this object.
        /// </summary>
        /// <param name="magioEffs">Effects to update</param>
        /// <param name="spread">Spread of the effects</param>
        /// <param name="spreadOriginLocal">Local origin of the spread</param>
        /// <param name="approxSize">Approx size of the object effect is attached</param>
        public void UpdateVFX(List<MagioEffectObject> magioEffs, float spread, Vector3 spreadOriginLocal, float approxSize)
        {
            for (int i = 0; i < magioEffs.Count; i++)
            {
                MagioEffectObject magioEffectObj = magioEffs[i];
                VisualEffect magioEffect = magioEffectObj.magioEffect;

                if (magioEffect.HasFloat("Magio_Spread_radius"))
                {
                    magioEffect.SetFloat("Magio_Spread_radius", spread);
                    magioEffect.SetVector3("Magio_Spread_center", targetGameObject.transform.TransformPoint(spreadOriginLocal));
                }



                if (magioEffect.HasVector3("Magio_Transform_position"))
                {
                    if (vfxSpawnerType == VFXSpawnerType.Mesh)
                    {
                        magioEffect.SetVector3("Magio_Transform_position", magioEffectObj.boundObject.position);
                        magioEffect.SetVector3("Magio_Transform_angles", magioEffectObj.boundObject.rotation.eulerAngles);
                        magioEffect.SetVector3("Magio_Transform_scale", magioEffectObj.boundObject.lossyScale);
                    }
                    else
                    {
                        magioEffect.SetVector3("Magio_Transform_position", magioEffectObj.skinnedMeshRenderer.rootBone.position);
                        magioEffect.SetVector3("Magio_Transform_angles", magioEffectObj.skinnedMeshRenderer.rootBone.rotation.eulerAngles);
                    }

                }

                if (magioEffect.HasVector3("Magio_Nullify_center") && nullifyRadius > 0)
                {
                    magioEffect.SetVector3("Magio_Nullify_center", targetGameObject.transform.TransformPoint(nullifyAreaCenter));
                    magioEffect.SetFloat("Magio_Nullify_radius", Mathf.MoveTowards(magioEffect.GetFloat("Magio_Nullify_radius"), nullifyRadius, 2f * Time.deltaTime));
                }

                if (magioEffect.HasVector3("Magio_ExternalForce"))
                {
                    Vector3 realExternalForce = addedExternalVelocity;
                    if (MagioEngine.instance.flameWindRetriever.OnUse() && affectedByWind)
                    {
                        realExternalForce += MagioEngine.instance.flameWindRetriever.GetCurrentWindVelocity();
                    }
                    magioEffect.SetVector3("Magio_ExternalForce", realExternalForce);
                }

                if (MagioEngine.instance.modifyEffectParametersOnRuntime || !Application.isPlaying)
                {
                    if (magioEffect.HasFloat("Magio_VFXMultiplier"))
                    {
                        magioEffect.SetFloat("Magio_VFXMultiplier", magioEffectObj.size);
                    }

                    VFXHelpers.SetupMagioParameters(magioEffect, vfxProperties);
                }


                if (effectEnabledTimer > fadeOutStart_s)
                {
                    float currentBurnout = Mathf.Clamp((1 - ((effectEnabledTimer - fadeOutStart_s)) / fadeOutLength_s), 0, 1);

                    if (magioEffect.HasFloat("Magio_LifelineMultiplier"))
                    {
                        magioEffect.SetFloat("Magio_LifelineMultiplier", currentBurnout);
                    }
                }
                else
                {
                    if (magioEffect.HasFloat("Magio_LifelineMultiplier"))
                    {
                        magioEffect.SetFloat("Magio_LifelineMultiplier", 1);
                    }
                }
            }
        }

        /// <summary>
        /// Resets all the vfx properties according to EffectPrefab
        /// </summary>
        public void RefreshAndResetVFXProperties()
        {
            VisualEffect visEff = EffectPrefab.GetComponent<VisualEffect>();
            if (visEff)
            {
                vfxProperties = VFXHelpers.GetNonMagioVFXProperties(visEff);
            }
            else
            {
                Debug.LogWarning("Assigned effect prefab gameobject does not have Visual Effect.");
                vfxProperties.Clear();
            }
        }

        /// <summary>
        /// Updates the properties list but does not override any.
        /// </summary>
        public void UpdateVFXProperties()
        {
            VisualEffect visEff = EffectPrefab.GetComponent<VisualEffect>();
            if (visEff)
            {
                vfxProperties = VFXHelpers.UpdateVfxPropertyList(visEff, vfxProperties);
            }
        }

        /// <summary>
        /// Gets VFX property with the name
        /// </summary>
        /// <param name="propertyName">name of the property (Case sensitive)</param>
        /// <returns>Property</returns>
        public VFXProps GetVFXPropertyValue(string propertyName)
        {
            List<VFXProps> props = vfxProperties.Where(o => o.name == propertyName).ToList();
            if (props.Count > 0)
            {
                return props[0];
            }

            return new VFXProps();
        }

        /// <summary>
        /// Sets VFX property value.
        /// </summary>
        /// <param name="prop">Property to replace with right name</param>
        /// <returns>True if property was valid. False if it was not</returns>
        public bool SetVFXPropertyValue(VFXProps prop)
        {
            int propIndex = vfxProperties.FindIndex(o => o.name == prop.name);
            if (propIndex > 0)
            {
                vfxProperties[propIndex] = prop;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Has the object already burnt out?
        /// </summary>
        /// <returns></returns>
        public bool HasEffectEnded()
        {
            return effectEnabledTimer > (fadeOutStart_s + fadeOutLength_s);
        }

        /// <summary>
        /// Was the object extinguished
        /// </summary>
        /// <returns></returns>
        public bool IsExtinguished()
        {
            return nullified;
        }

        /// <summary>
        /// Return the radius of the current extinguish effect
        /// </summary>
        /// <returns>radius in m</returns>
        public float GetNullifyRadius()
        {
            return nullifyRadius;
        }

        /// <summary>
        /// Return the center of the current extinguish effect
        /// </summary>
        /// <returns>World position</returns>
        public Vector3 GetNullifyCenter()
        {
            return targetGameObject.transform.TransformPoint(nullifyAreaCenter);
        }

        /// <summary>
        /// Returns current ignition progress
        /// </summary>
        /// <returns>ignition progress in seconds</returns>
        public float GetCurrentIgnitionProgress()
        {
            return currentAnimateProgress_s;
        }

        /// <summary>
        /// Gets Position of the origin of the spread
        /// </summary>
        /// <returns>Position of the origin of the spread</returns>
        public Vector3 GetEffectOrigin()
        {
            if (magioEffects.Count > 0)
            {
                return targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal);
            }
            else
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Gets the approximated longest size of an object
        /// </summary>
        /// <returns></returns>
        public float GetObjectApproxSize()
        {
            return objectMaster.ApproxSize;
        }

        private void UpdateMagioShader(Material mat, Renderer rend)
        {
            if (MagioEngine.instance.modifyEffectParametersOnRuntime || !firstMagioShaderSetup)
            {
                SetupMagioShader(mat);
            }

            if (magioShaderEffectMode == MagioShaderEffectMode.Dissolve)
            {
                mat.SetFloat("_DissolveSpread", effectSpread);
                mat.SetVector("_DissolveOrigin", targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));
            }
            else if (magioShaderEffectMode == MagioShaderEffectMode.Emission_Overlay)
            {
                mat.SetFloat("_EffectSpread", effectSpread);
                mat.SetVector("_EffectOrigin", targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));

                if (effectEnabledTimer >= fadeOutStart_s)
                {
                    mat.SetFloat("_OverlayBrightness",
                        Mathf.MoveTowards(mat.GetFloat("_OverlayBrightness"), 0, (1 / fadeOutLength_s) * Time.deltaTime));
                }
                else
                {
                    float currentProgress = Mathf.Min(1, magioMaterialAnimationTimer / achieveEmissionColorTime_s);
                    mat.SetFloat("_OverlayBrightness",
                    Mathf.Lerp(0, 1, currentProgress) * ((1 - shaderColorNoise) + (Mathf.PerlinNoise(magioMaterialAnimationTimer * shaderColorNoiseSpeed, 0) * (shaderColorNoise * 2))));

                    if (!reAnimated && blendToOtherTexture)
                    {
                        if (magioMaterialAnimationTimer > fadeOutStart_s / 2)
                        {
                            mat.SetFloat("_BlendTextureLerpT",
                                        Mathf.Clamp((magioMaterialAnimationTimer - (fadeOutStart_s / 2)) / (fadeOutStart_s / 2), 0, 1));
                        }
                    }
                }

                mat.SetVector("_NullifyOrigin", targetGameObject.transform.TransformPoint(nullifyAreaCenter));
                mat.SetFloat("_NullifySpread", nullifyRadius);
            }
            else
            {
                mat.SetFloat("_TextureOverlaySpread", effectSpread);
                mat.SetVector("_TextureOverlayOrigin", targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));

                if (effectEnabledTimer > fadeOutStart_s)
                {
                    float currentBurnout = Mathf.Clamp((1 - ((effectEnabledTimer - fadeOutStart_s)) / fadeOutLength_s), 0, 1);

                    mat.SetFloat("_LifelineMultiplier", Mathf.MoveTowards(mat.GetFloat("_LifelineMultiplier"), currentBurnout, 0.1f));
                }
                else
                {
                    if (effectEnabledTimer < 2)
                    {
                        mat.SetFloat("_LifelineMultiplier", 1);
                    }
                    else
                    {
                        // Smooth transition if there was other effect before this
                        mat.SetFloat("_LifelineMultiplier", Mathf.MoveTowards(mat.GetFloat("_LifelineMultiplier"), 1, 0.3f * Time.deltaTime));
                    }

                }

                mat.SetVector("_NullifyTextureOverlayOrigin", targetGameObject.transform.TransformPoint(nullifyAreaCenter));
                mat.SetFloat("_NullifyTextureOverlaySpread", nullifyRadius);
            }
        }

        private void CheckForFadeOut(float radiusIncrement)
        {
            float distFromCenter = Vector3.Distance(objectMaster.ApproxCenter, targetGameObject.transform.TransformPoint(nullifyAreaCenter));

            float percentageFromSize = distFromCenter / objectMaster.ApproxSize;
            if (nullifyRadius > ((objectMaster.ApproxSize * fullNullifyToughness * (0.5f + percentageFromSize))))
            {
                if (effectEnabledTimer < fadeOutStart_s)
                {
                    fadeOutStart_s = effectEnabledTimer;
                    if (magioEventInvoker) magioEventInvoker.Nullified.Invoke();
                }
                else if (nullifyRadius > (objectMaster.ApproxSize * (0.5f + percentageFromSize)))
                {
                    fadeOutLength_s -= (nullifyRadius - (objectMaster.ApproxSize * (0.5f + percentageFromSize))) * 0.1f;
                    fadeOutLength_s = Mathf.Max(0, fadeOutLength_s);
                }
                nullified = true;
            }
        }

        private void UpdateShaders()
        {
            if (enableMaterialAnimation)
            {
                bool materialAnimationTimerUpdated = false;
                bool magioMaterialAnimationTimerUpdated = false;

                if (rends.Length > 0)
                {
                    foreach (Renderer rend in rends)
                    {
                        MagioMaterialAnimationSettings overrideComp = rend.gameObject.GetComponent<MagioMaterialAnimationSettings>();
                        if (!overrideComp) overrideComp = rend.gameObject.AddComponent<MagioMaterialAnimationSettings>();
                        for (int i = 0; i < rend.materials.Length; i++)
                        {
                            Material mat = rend.materials[i];
                            if (rend.materials[i].HasProperty("_IsMagioShader"))
                            {
                                if (objectMaster.CanIUseMaterialAnimation(this, true))
                                {
                                    if (!magioMaterialAnimationTimerUpdated)
                                    {
                                        magioMaterialAnimationTimer += Time.deltaTime;
                                        magioMaterialAnimationTimerUpdated = true;
                                    }
                                    UpdateMagioShader(mat, rend);
                                }
                            }
                            else
                            {
                                if (objectMaster.CanIUseMaterialAnimation(this, false))
                                {
                                    if (!materialAnimationTimerUpdated)
                                    {
                                        materialAnimationTimer += Time.deltaTime;
                                        materialAnimationTimerUpdated = true;
                                    }

                                    List<OAVAShaderCompatibilitySO> comps = overrideComp.GetCompatibilities();

                                    if (comps.Count >= i - 1)
                                    {
                                        UpdateSupportedThirdPartyShaders(mat, rend, comps[i]);
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Something went wrong initializing material animation Compatibilities");
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        private void SetupShaders()
        {
            if (enableMaterialAnimation)
            {
                Renderer[] rends = targetGameObject.GetComponentsInChildren<Renderer>();

                if (rends.Length > 0)
                {
                    foreach (Renderer rend in rends)
                    {
                        MagioMaterialAnimationSettings overrideComp = rend.gameObject.GetComponent<MagioMaterialAnimationSettings>();
                        if (!overrideComp) overrideComp = rend.gameObject.AddComponent<MagioMaterialAnimationSettings>();
                        for (int i = 0; i < rend.materials.Length; i++)
                        {
                            Material mat = rend.materials[i];
                            if (mat.HasProperty("_IsMagioShader"))
                            {
                                if (objectMaster.CanIUseMaterialAnimation(this, true))
                                {
                                    SetupMagioShader(mat);
                                }
                            }
                            else
                            {
                                List<OAVAShaderCompatibilitySO> comps = overrideComp.GetCompatibilities();
                                if (comps.Count >= i - 1)
                                {
                                    KeywordsEnableCompatibleShaders(mat, comps[i]);
                                }
                                else
                                {
                                    Debug.LogWarning("Something went wrong initializing material animation Compatibilities");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetupMagioShader(Material mat)
        {
            if (magioShaderEffectMode == MagioShaderEffectMode.Emission_Overlay)
            {
                mat.SetFloat("_UseOverlay", 1);

                mat.SetFloat("_VoronoiMove", shaderColorNoiseSpeed * 2);
                mat.SetColor("_OverlayColor", shaderEmissionColor);
                mat.SetFloat("_OverlayMaximumBrightness", shaderEmissionMultiplier);
                mat.SetFloat("_OverlayMinimumBrightness", 0);

                mat.SetTexture("_BlendTextureAlbedo", blendAlbedoMap);
                mat.SetTexture("_BlendTextureNormal", blendNormalMap);

                mat.SetVector("_BlendTextureTiling", blendTextureTiling);
                mat.SetFloat("_BlendTextureNormalStrength", blendTextureNormalStrength);

            }
            else if (magioShaderEffectMode == MagioShaderEffectMode.Dissolve)
            {
                if (!objectMaster.IsEmissionOverlayerEffectEnabled())
                {
                    mat.SetColor("_OverlayColor", shaderEmissionColor);
                    mat.SetFloat("_OverlayMaximumBrightness", shaderEmissionMultiplier);
                    mat.SetFloat("_OverlayMinimumBrightness", 0);
                }
                mat.SetFloat("_DissolveEdgeWidth", dissolveEmissionEdgeWidth);
                mat.SetFloat("_UseDissolve", 1);
            }
            else
            {
                mat.SetFloat("_UseTextureOverride", 1);

                mat.SetTexture("_OverrideAlbedo", overrideAlbedoMap);
                mat.SetColor("_OverrideColor", overrideColor);
                mat.SetTexture("_OverrideNormalMap", overrideNormalMap);

                mat.SetVector("_OverrideTiling", overrideTextureTiling);
                mat.SetVector("_OverrideOffset", overrideTextureOffset);

                mat.SetFloat("_OverrideStrength", overrideStrength);
                mat.SetFloat("_OverrideAddWetness", overrideAddWetness);

                if (!objectMaster.IsEmissionOverlayerEffectEnabled())
                {
                    mat.SetColor("_OverlayColor", shaderEmissionColor);
                    mat.SetFloat("_OverlayMaximumBrightness", shaderEmissionMultiplier);
                    mat.SetFloat("_OverlayMinimumBrightness", 0);
                }
            }

            firstMagioShaderSetup = true;
        }

        private void KeywordsEnableCompatibleShaders(Material mat, OAVAShaderCompatibilitySO shaderComp)
        {
            if (shaderComp)
            {
                shaderComp.OnEffectStart(mat, this);
            }
        }


        private void UpdateSupportedThirdPartyShaders(Material mat, Renderer rend, OAVAShaderCompatibilitySO shaderComp)
        {
            UpdateCompatibleShaders(mat, rend, shaderComp);
        }

        private void UpdateCompatibleShaders(Material mat, Renderer rend, OAVAShaderCompatibilitySO shaderComp)
        {
            if (shaderComp)
            {
                float currentProgress;
                float noise;
                if (effectEnabledTimer < fadeOutStart_s)
                {
                    currentProgress = Mathf.Min(1, materialAnimationTimer / achieveEmissionColorTime_s);
                    noise = shaderColorNoise * currentProgress;
                    shaderComp.SetDuringProperties(mat, this, currentProgress);
                }
                else
                {
                    float timeSinceFadeOutStart = effectEnabledTimer - fadeOutStart_s;
                    currentProgress = Mathf.Max(0, 1 - (timeSinceFadeOutStart / (fadeOutLength_s)));
                    noise = shaderColorNoise * currentProgress;
                    shaderComp.SetAfterFadeOutProperties(mat, this, currentProgress);
                }

                float shaderEmissionIntensity = 1;

                if (shaderComp.useEmissionHDRColor)
                    shaderEmissionIntensity = shaderEmissionMultiplier;

                if (mat.HasProperty(shaderComp.ShaderMainColorPropertyName))
                {
                    if (!saver.OriginalMaterialValues.ContainsKey(mat)) return;
                    Color originalColor = saver.OriginalMaterialValues[mat].originalMainColor;

                    mat.SetColor(shaderComp.ShaderMainColorPropertyName, Color.Lerp(originalColor,
                    new Color(shaderEmissionColor.r * shaderEmissionIntensity,
                        shaderEmissionColor.g * shaderEmissionIntensity,
                        shaderEmissionColor.b * shaderEmissionIntensity),
                        currentProgress) * ((1 - noise) + (Mathf.PerlinNoise(effectEnabledTimer * shaderColorNoiseSpeed, 0)) * (noise * 2)));
                }
                else if (!shaderComp.ShaderMainColorPropertyName.Equals(string.Empty))
                {
                    Debug.LogWarning("Shader " + mat.shader.name + " does not contain color property: " + shaderComp.ShaderMainColorPropertyName + " which has been added to ShaderCompabilities");
                }

                if (mat.HasProperty(shaderComp.ShaderEmissionColorPropertyName))
                {
                    if (!saver.OriginalMaterialValues.ContainsKey(mat)) return;
                    Color originalColor = saver.OriginalMaterialValues[mat].originalEmissionColor;

                    mat.SetColor(shaderComp.ShaderEmissionColorPropertyName, Color.Lerp(originalColor,
                    new Color(shaderEmissionColor.r * shaderEmissionIntensity,
                        shaderEmissionColor.g * shaderEmissionIntensity,
                        shaderEmissionColor.b * shaderEmissionIntensity),
                        currentProgress) * ((1 - noise) + (Mathf.PerlinNoise(effectEnabledTimer * shaderColorNoiseSpeed, 0)) * (noise * 2)));
                }
                else if (!shaderComp.ShaderEmissionColorPropertyName.Equals(string.Empty))
                {
                    Debug.LogWarning("Shader " + mat.shader.name + " does not contain color property: " + shaderComp.ShaderEmissionColorPropertyName + " which has been added to ShaderCompabilities");
                }

            }
        }

        private void UpdateSFX()
        {
            foreach (AudioSource source in allEffectSFX)
            {
                if (source)
                {
                    if (sfxMode == SFXMode.WhenEffectIsOn)
                    {
                        if (effectEnabledTimer > fadeOutStart_s)
                        {
                            float currentBurnout = Mathf.Clamp((1 - ((effectEnabledTimer - fadeOutStart_s)) / fadeOutLength_s), 0, 1);

                            source.volume = Mathf.Max(0, currentBurnout - Mathf.PerlinNoise(effectEnabledTimer / 2, 0) * 0.1f);
                        }
                        else
                        {
                            float currentProgress = Mathf.Clamp(effectSpread / GetObjectApproxSize(), 0, 1);
                            source.volume = Mathf.Max(currentProgress - Mathf.PerlinNoise(effectEnabledTimer / 2, 0) * 0.1f);
                        }
                    }
                    else
                    {
                        float distFromCenter = Vector3.Distance(objectMaster.ApproxCenter, targetGameObject.transform.TransformPoint(_effectSpreadOriginLocal));

                        float percentageFromSize = distFromCenter / objectMaster.ApproxSize;
                        if (effectSpread < (objectMaster.ApproxSize * (0.48f + percentageFromSize)))
                        {
                            source.volume = Mathf.Clamp(effectEnabledTimer, 0, 1);
                        }
                        else
                        {
                            source.volume = Mathf.MoveTowards(source.volume, 0, Time.deltaTime);
                        }
                    }

                }
            }
        }

        private void CheckForRemove()
        {
            if (canBeReAnimated == CanBeReanimated.Always)
            {
                ResetObj();
                nullified = false;
                reAnimated = true;
            }
            else if (canBeReAnimated == CanBeReanimated.Only_After_Nullify && nullified)
            {
                ResetObj();
                reAnimated = true;
            }
            else
            {
                foreach (MagioEffectObject eff in magioEffects)
                {
                    eff.magioEffect.gameObject.AddComponent<SelfDestroy>();
                }

                foreach (SpreadPhysics trigger in spreadPhysics)
                {
                    Destroy(trigger);
                }
                magioEffects.Clear();
                spreadPhysics.Clear();
            }

            if (deleteObjectAfterFadeOut)
            {
                Destroy(gameObject);
            }
        }

        private void InvokeCallbacks()
        {
            foreach (MagioTriggerCallbacks callback in GetComponents<MagioTriggerCallbacks>())
            {
                callback.Invoke(nameof(callback.TriggerEvents), callback.delaySeconds);
            }
        }


        private void SetupSFX(Transform parent)
        {
            if (addSFXRuntime && effectSFX)
            {
                GameObject audioGO = Instantiate(effectSFX, parent);
                audioGO.transform.parent = parent;
                audioGO.transform.position = parent.transform.position;
                audioGO.transform.rotation = parent.transform.rotation;

                //Sound
                curAudio = audioGO.GetComponentInChildren<AudioSource>();
                if (curAudio)
                {
                    curAudio.time = Random.Range(0f, sfxStartTimeRandomizationMax);
                    curAudio.pitch = Random.Range(1 - sfxPitchRandomization, 1 + sfxPitchRandomization);
                    curAudio.volume = 0;

                    allEffectSFX.Add(curAudio);
                }
            }
        }

        private void SetupEffect()
        {
            if (vfxSpawnerType == VFXSpawnerType.Mesh)
            {
                SetupMeshEffect();
            }
            else
            {
                SetupSkinnedMeshEffect();
            }
        }

        private void SetupMeshEffect()
        {
            foreach (Renderer rend in magioRenderers)
            {
                MeshFilter filter = rend.GetComponent<MeshFilter>();

                if (!filter) continue;

                VisualEffect magioEff = Instantiate(_effectPrefab, Vector3.zero, Quaternion.identity, MagioEngine.instance.effectParent).GetComponent<VisualEffect>();

                if (!magioEff.HasMesh("Magio_Mesh"))
                {
                    Debug.LogError("IF you are getting this error when importing Magio for first time, Please install Visual Effects Graph and Re - import Magio Folder(Right - click->Re - import). Otherwise: Effect invalid for meshes. Please check that your effect has property called Magio_Mesh");
                    return;
                }

                if (magioEff.HasBool("Magio_UseSkinnedMesh"))
                {
                    magioEff.SetBool("Magio_UseSkinnedMesh", false);
                }

                magioEff.SetMesh("Magio_Mesh", filter.mesh);


                VFXHelpers.SetupMagioParameters(magioEff, vfxProperties);

                MagioEffectObject magObj = new MagioEffectObject() { boundObject = filter.gameObject.transform, magioEffect = magioEff, size = Vector3.Scale(filter.mesh.bounds.size, filter.transform.lossyScale).magnitude };

                if (magioEff.HasFloat("Magio_VFXMultiplier"))
                {
                    magioEff.SetFloat("Magio_VFXMultiplier", magObj.size);
                }

                SetupSFX(filter.transform);

                magioEffects.Add(magObj);
            }

            if (effectBehaviourType == EffectBehaviourMode.Spread && spreadToOtherObjects)
            {
                SpreadPhysics spreadComponent = gameObject.AddComponent<SpreadPhysics>();

                spreadComponent.myMagioObj = this;

                spreadPhysics.Add(spreadComponent);

                spreadComponent.StartTrigger();
            }
        }

        private void SetupSkinnedMeshEffect()
        {
            SkinnedMeshRenderer[] renderers = magioRenderers.OfType<SkinnedMeshRenderer>().ToArray();

            if (renderers.Length <= 0)
            {
                Debug.LogError("No Skinned mesh renderer found! Please change VFX spawner type to mesh or attach skinned mesh renderer to child");
                return;
            }

            foreach (SkinnedMeshRenderer skinnedMesh in renderers)
            {
                VisualEffect magioEff = Instantiate(_effectPrefab, Vector3.zero, Quaternion.identity, MagioEngine.instance.effectParent).GetComponent<VisualEffect>();

                if (!magioEff.HasSkinnedMeshRenderer("Magio_SkinnedMesh"))
                {
                    Debug.LogError("IF you are getting this error when importing Magio for first time, Please install Visual Effects Graph and Re-import Magio Folder (Right-click->Re-import). Otherwise: Effect invalid for Skinned Meshes.  Please check that your effect has property called Magio_SkinnedMesh");
                    return;
                }

                if (magioEff.HasBool("Magio_UseSkinnedMesh"))
                {
                    magioEff.SetBool("Magio_UseSkinnedMesh", true);
                }

                magioEff.SetSkinnedMeshRenderer("Magio_SkinnedMesh", skinnedMesh);

                VFXHelpers.SetupMagioParameters(magioEff, vfxProperties);


                MagioEffectObject magObj = new MagioEffectObject() { boundObject = transform, skinnedMeshRenderer = skinnedMesh, magioEffect = magioEff, size = Vector3.Scale(skinnedMesh.sharedMesh.bounds.size, skinnedMesh.transform.lossyScale).magnitude };

                if (magioEff.HasFloat("Magio_VFXMultiplier"))
                {
                    magioEff.SetFloat("Magio_VFXMultiplier", magObj.size);
                }

                SetupSFX(targetGameObject.transform);

                magioEffects.Add(magObj);
            }

            if (spreadToOtherObjects)
            {
                SpreadPhysics spreadComponent = targetGameObject.AddComponent<SpreadPhysics>();

                spreadComponent.myMagioObj = this;

                spreadPhysics.Add(spreadComponent);

                spreadComponent.StartTrigger();
            }
        }

        private Vector3 Clamp0360Vector(Vector3 vector)
        {
            return new Vector3(Clamp0360(vector.x), Clamp0360(vector.y), Clamp0360(vector.z));
        }

        private float Clamp0360(float eulerAngles)
        {
            float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
            if (result < 0)
            {
                result += 360f;
            }
            return result;
        }



        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Collider[] cols;
                if (useAllCollidersToSpread)
                {
                    if (targetGameObject)
                        cols = targetGameObject.GetComponentsInChildren<Collider>();
                    else
                        cols = new Collider[0];
                }
                else
                {
                    cols = spreadingColliders.ToArray();
                }

                foreach (Collider col in cols)
                {
                    if (col.GetType() == typeof(BoxCollider))
                    {
                        BoxCollider box = col as BoxCollider;
                        Gizmos.matrix = box.transform.localToWorldMatrix;
                        Gizmos.DrawWireCube(box.center, box.size + Vector3.Scale(effectSpreadAreaAddition, new Vector3(1 / box.transform.lossyScale.x, 1 / box.transform.lossyScale.y, 1 / box.transform.lossyScale.z)));
                    }
                    else if (col.GetType() == typeof(SphereCollider))
                    {
                        SphereCollider sphere = col as SphereCollider;
                        Gizmos.matrix = sphere.transform.localToWorldMatrix;
                        Gizmos.DrawWireCube(sphere.center, new Vector3(sphere.radius * 2, sphere.radius * 2, sphere.radius * 2) + Vector3.Scale(effectSpreadAreaAddition, new Vector3(1 / col.transform.lossyScale.x, 1 / col.transform.lossyScale.y, 1 / col.transform.lossyScale.z)));
                    }
                    else if (col.GetType() == typeof(CapsuleCollider))
                    {
                        CapsuleCollider capsule = col as CapsuleCollider;
                        Gizmos.matrix = capsule.transform.localToWorldMatrix;
                        float x = capsule.radius * 2;
                        float y = capsule.radius * 2;
                        float z = capsule.radius * 2;
                        if (capsule.height > capsule.radius * 2)
                        {
                            float height = capsule.height - capsule.radius * 2;
                            if (capsule.direction == 0)
                            {
                                x += height;
                            }
                            else if (capsule.direction == 1)
                            {
                                y += height;
                            }
                            else if (capsule.direction == 2)
                            {
                                z += height;
                            }
                        }

                        Gizmos.DrawWireCube(capsule.center, new Vector3(x, y, z) + Vector3.Scale(effectSpreadAreaAddition, new Vector3(1 / col.transform.lossyScale.x, 1 / col.transform.lossyScale.y, 1 / col.transform.lossyScale.z)));
                    }
                }

            }

        }

        private void OnDestroy()
        {
            if (objectMaster)
                objectMaster.RemoveMagioObject(this);
            foreach (MagioEffectObject eff in magioEffects)
            {
                if (eff.magioEffect)
                    Destroy(eff.magioEffect);
            }
            foreach (SpreadPhysics trigger in spreadPhysics)
            {
                if (trigger)
                    Destroy(trigger.gameObject);
            }
            magioEffects.Clear();
            spreadPhysics.Clear();
        }
    }

}
