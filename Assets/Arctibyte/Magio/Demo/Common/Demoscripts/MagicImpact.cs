using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Magio
{
    public class MagicImpact : MonoBehaviour
    {
        public float size = 1;

        [ColorUsageAttribute(true, true)]
        public Color color;

        [ColorUsageAttribute(true, true)]
        public Color innerColor;

        public LayerMask mask;

        public GameObject impact;

        public GameObject splashEffect;

        public EffectClass effClass = EffectClass.Default;



        // Start is called before the first frame update
        void Start()
        {
            VisualEffect eff = GetComponent<VisualEffect>();
            if (eff)
            {
                if(!eff.HasFloat("Size"))
                {
                    Debug.LogError("If you see this error: Please install Visual Effects Graph from the package manager and Re-import Magio Folder (Right-click->Re-Import)");
                    return;
                }
                eff.SetFloat("Size", size);
                eff.SetVector4("Color", color);
                eff.SetVector4("InnerColor", innerColor);
            }
        }

        public void Setup()
        {
            VisualEffect eff = GetComponent<VisualEffect>();
            if (eff)
            {
                if (!eff.HasFloat("Size"))
                {
                    Debug.LogError("If you see this error: Please install Visual Effects Graph from the package manager and Re-import Magio Folder (Right-click->Re-Import)");
                    return;
                }
                eff.SetFloat("Size", size);
                eff.SetVector4("Color", color);
                eff.SetVector4("InnerColor", innerColor);
            }
        }

        // Update is called once per frame
        void Update()
        {
            Collider[] col = Physics.OverlapSphere(transform.position, size, mask);
            if (col.Length > 0)
            {
                GameObject impactGO = Instantiate(impact, transform.position, Quaternion.identity);
                SphereIgnite ignite = impactGO.GetComponent<SphereIgnite>();

                VisualEffect impactEff = impactGO.GetComponent<VisualEffect>();
                if (impactEff)
                {
                    if (!impactEff.HasVector4("Color1"))
                    {
                        Debug.LogError("If you see this error: Please install Visual Effects Graph from the package manager and Re-import Magio Folder (Right-click->Re-Import)");
                        return;
                    }
                    impactEff.SetVector4("Color1", innerColor);
                    impactEff.SetVector4("Color2", color);
                }

                ignite.raycastRadius = size;
                ignite.affectedClass = effClass;
                ignite.splashEffectPrefab = splashEffect;
                impactGO.gameObject.AddComponent<SelfDestroy>().destroyDelay = 3;

                VisualEffect eff = GetComponent<VisualEffect>();
                if (eff)
                {
                    eff.SetFloat("ParticleMultiplier", 0);
                    eff.gameObject.AddComponent<SelfDestroy>().destroyDelay = 5;
                }

                enabled = false;
            }
        }
    }
}

