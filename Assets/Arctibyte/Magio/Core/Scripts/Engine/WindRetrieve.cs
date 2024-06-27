using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Used to retrieve global wind.
    /// </summary>
    public class WindRetrieve : MonoBehaviour
    {
        Vector3 currentWindVelocity = Vector3.zero;
        bool useTVEWind = false;
        bool onUse = true;
        WindZone zone;

        float enabledTimer = 0;
        bool setup = false;

        void Start()
        {
            Setup();
        }

        void Setup()
        {
            enabledTimer = 0;
            Vector2 tve = Shader.GetGlobalVector("TVE_NoiseSpeed_Vegetation");
            zone = FindObjectOfType<WindZone>(true);

            if (zone)
            {
                useTVEWind = false;
            }
            else if (tve != null)
            {
                useTVEWind = true;
            }
            else
            {
                onUse = false;
            }

            setup = true;
        }

        void Update()
        {
            if(onUse)
            {
                enabledTimer += Time.deltaTime;
            }
        }

        public bool OnUse()
        {
            return onUse;
        }

        /// <summary>
        /// Gets current global wind velocity (TVE or WINDZONE)
        /// </summary>
        /// <returns>Wind velocity</returns>
        public Vector3 GetCurrentWindVelocity()
        {
            if (!setup) Setup();
            if (onUse)
            {
                if (useTVEWind)
                {
                    Vector2 wind = Shader.GetGlobalVector("TVE_NoiseSpeed_Vegetation");
                    Vector4 multiplier = Shader.GetGlobalVector("TVE_MotionParams");
                    currentWindVelocity = new Vector3(-wind.x * multiplier.z * 10, 0, -wind.y * multiplier.z * 10);
                }
                else
                {
                    currentWindVelocity = zone.transform.forward * (zone.windMain * 0.1f + zone.windTurbulence * Mathf.PerlinNoise(enabledTimer, 0));
                }

            }

            return currentWindVelocity;
        }
    }
}
