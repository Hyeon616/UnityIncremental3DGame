using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Attach this to your light, link MagioObjectEffect and set intensities to animate the light.
    /// </summary>
    public class MagioLight : MonoBehaviour
    {
        [Tooltip("This magio object progress will command this script to enable and disable")]
        public MagioObjectEffect attachedMagioObject;

        [Tooltip("How much should the light flicker? This is a multiplier")]
        [Range(0, 1)]
        public float flickerMultiplier = 0.3f;

        [Tooltip("How fast should the light flicker?")]
        public float flickerSpeed = 5;

        [Tooltip("Smoothing when the light is enabled")]
        public float achieveMaxIntensityTime = 2;

        [Tooltip("Start intensity for the light")]
        public float startIntensity = 0;

        [Tooltip("Target intensity for the light while effect is on")]
        public float effectOnIntensity = 2;

        [Tooltip("Target intensity for the light after effect has been fade out")]
        public float afterEffectIntensity = 0;

        Light targetLight;
        float currentIntensityWithoutNoise = 0;
        bool disableThis = false;
        bool enableThis = false;
        float enabledTime = 0;

        void Awake()
        {
            targetLight = GetComponent<Light>();
        }

        private void Start()
        {
            if (targetLight.intensity < 0.001f)
            {
                targetLight.enabled = false;
            }
            enabledTime = 0;
            targetLight.intensity = startIntensity;
        }

        /// <summary>
        /// Smoothly linearly enables the light and starts the interpolation
        /// </summary>
        public void SmoothEnable()
        {
            if (!enableThis)
            {
                disableThis = false;
                enableThis = true;
                targetLight.enabled = true;
                currentIntensityWithoutNoise = startIntensity;
            }
            
        }

        /// <summary>
        /// Disables the light. Interpolate to afterEffectIntensity.
        /// </summary>
        public void SmoothDisable()
        {
            enableThis = false;
            disableThis = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (attachedMagioObject.effectEnabled)
            {
                if (attachedMagioObject.effectEnabledTimer > attachedMagioObject.fadeOutStart_s)
                {
                    SmoothDisable();
                }
                else
                {
                    if ((Vector3.Distance(transform.position, attachedMagioObject.GetEffectOrigin()) <= attachedMagioObject.effectSpread) && (attachedMagioObject.GetNullifyRadius() < 0.05 || Vector3.Distance(transform.position, attachedMagioObject.GetNullifyCenter()) > attachedMagioObject.GetNullifyRadius()))
                    {
                        SmoothEnable();
                    }
                    else
                    {
                        SmoothDisable();
                    }
                }
            }

            if (enableThis)
            {
                enabledTime += Time.deltaTime;
                currentIntensityWithoutNoise = Mathf.MoveTowards(currentIntensityWithoutNoise, effectOnIntensity, (Mathf.Abs(startIntensity - effectOnIntensity) * Time.deltaTime) / achieveMaxIntensityTime);
                targetLight.intensity = currentIntensityWithoutNoise * ((1 - flickerMultiplier) + Mathf.PerlinNoise(enabledTime * flickerSpeed, 0) * (flickerMultiplier * 2));
            }
            else if (disableThis)
            {
                enabledTime += Time.deltaTime;
                currentIntensityWithoutNoise = Mathf.MoveTowards(currentIntensityWithoutNoise, afterEffectIntensity, (Mathf.Abs(effectOnIntensity - afterEffectIntensity) * Time.deltaTime) / attachedMagioObject.fadeOutLength_s);
                targetLight.intensity = currentIntensityWithoutNoise * ((1 - flickerMultiplier) + Mathf.PerlinNoise(enabledTime * flickerSpeed, 0) * (flickerMultiplier * 2));

                if (targetLight.intensity < 0.01f)
                {
                    targetLight.enabled = false;
                }
            }
        }
    }

}
