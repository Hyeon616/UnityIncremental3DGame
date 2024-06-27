using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Attach this to VFX prefab if you want to override shader defaults with VFX specific values
    /// </summary>
    public class MagioPrefabDefaults : MonoBehaviour
    {
        [Tooltip("Only objects within same effect class can spread the effect to each other.")]
        public EffectClass effectClass = EffectClass.Default;


        [Tooltip("Enables Material animation if your shader has support created for it.")]
        public bool enableMaterialAnimation = true;

        [Tooltip("Shader emission color")]
        public Color shaderEmissionColor = new Color(241f / 255f, 121f / 255f, 11f / 255, 1f);

        [Tooltip("How fast shader changes to the burnt color? Lerp percentage multiplier.")]
        [Range(0, 10)]
        public float shaderToEffectEndInterpolateSpeed = 0.03f;

        [Min(0)]
        [Tooltip("How strong is emission in the shader")]
        public float shaderEmissionMultiplier = 1;

        [Min(0)]
        [Tooltip("How long does it to reach max brightness in material shader. (Seconds)")]
        public float achieveEmissionColorTime_s = 20;

        [Range(0, 1f)]
        [Tooltip("Shader Color noise multiplier min max")]
        public float shaderColorNoise = 0.05f;

        [Range(0, 10)]
        [Tooltip("Shader Color noise change speed.")]
        public float shaderColorNoiseSpeed = 1;

        [Tooltip("How Magio shaders behave. Emission overlay = Overlay emission with color with some noise. Dissolve = Dissolves the mesh alongside with spread.")]
        public MagioObjectEffect.MagioShaderEffectMode magioShaderEffectMode = MagioObjectEffect.MagioShaderEffectMode.Emission_Overlay;

        [Tooltip("Override object texture with this when effect has spread")]
        public Texture2D overrideAlbedoMap;

        [Tooltip("Override object texture normal with this when effect has spread")]
        public Texture2D overrideNormalMap;

        [Tooltip("Override object texture tiling")]
        public Vector2 overrideTextureTiling;

        [Tooltip("Override object texture offset")]
        public Vector2 overrideTextureOffset;

        [Range(0f, 1f)]
        [Tooltip("How much strength the override texture has? 1 = Full override, 0 = no override")]
        public float overrideStrength = 1.0f;

        [Range(0f, 1f)]
        [Tooltip("How much wetness you want to add with override? 1 = Add full wetness, 0 = no added wetness")]
        public float overrideAddWetness = 0.0f;

        [Tooltip("Which color to multiply the texture with")]
        public Color overrideColor = Color.white;

        [Tooltip("Do you want to lerp the texture to something else. (e.g. burnt with fire).")]
        public bool blendToOtherTexture;

        [Tooltip("Blend object texture with this when effect has spread")]
        public Texture2D blendAlbedoMap;

        [Tooltip("Blend object texture normal with this when effect has spread")]
        public Texture2D blendNormalMap;

        [Tooltip("Blend object texture normal map strength")]
        public float blendTextureNormalStrength;

        [Tooltip("Blend object texture tiling")]
        public Vector2 blendTextureTiling;

        [Tooltip("Prefab for the effect sound to be added when the object is ignited.")]
        public GameObject effectSFX;

        [Tooltip("Do you want to slow animation down when effect is enabled?")]
        public bool slowAnimationOnEffectStart = false;

        [Tooltip("If you want to slow the animation down after the effect is enabled.")]
        public float animationSlowDownSpeed = 0.2f;

        [Tooltip("To what value do you want to slow the animation down to?")]
        public float animationSlowDownTargetValue = 0;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(this);
        }
    }

}
