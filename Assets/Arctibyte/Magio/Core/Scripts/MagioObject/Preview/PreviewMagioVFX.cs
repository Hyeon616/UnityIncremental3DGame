#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Magio
{
    [ExecuteInEditMode]
    public class PreviewMagioVFX : MonoBehaviour
    {
        [HideInInspector]
        [Range(0, 10000)]
        public float effectSpread = 100000;

        [HideInInspector]
        public MagioObjectEffect magioObj;

        [HideInInspector]
        public List<BoxCollider> flammableColliders;

        [HideInInspector]
        public List<BoxCollider> tempColliders;

        [HideInInspector]
        public List<MagioObjectEffect.MagioEffectObject> effects;

        [HideInInspector]
        public float approxSize = 1;

        [HideInInspector]
        public PreviewMagioVFXController controller;


        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                if (effects == null)
                {
                    effects = new List<MagioObjectEffect.MagioEffectObject>();
                }

                if (effects.Count > 0) return;

                if (effects.Count <= 0 && magioObj)
                {
                    StartPreview();
                }
            }
        }


        private void Awake()
        {
            if (Application.isPlaying)
            {
                this.enabled = false;
            }

        }

        public void SetupAndStart(MagioObjectEffect magioO, PreviewMagioVFXController contr)
        {
            magioObj = magioO;
            controller = contr;
            StartPreview();
        }

        private void CalculateApproxSize()
        {
            List<Renderer> rends = GetComponentsInChildren<Renderer>(true).ToList();
            if (rends.Count <= 0)
            {
                approxSize = 1;
                return;
            }

            Bounds combinedRendBounds = rends[0].bounds;

            foreach (Renderer rend in rends)
            {
                if (rend != rends[0]) combinedRendBounds.Encapsulate(rend.bounds);
            }

            approxSize = combinedRendBounds.size.magnitude;
        }

        // Update is called once per frame
        void Update()
        {
            if (!magioObj)
                DestroyImmediate(this);
            if(effects != null && !Application.isPlaying)
            {
                Vector3 localOrigin = Vector3.zero;
                if (!magioObj.customStartOrigin)
                    localOrigin = magioObj.targetGameObject.transform.InverseTransformPoint(transform.position);
                else
                    localOrigin = magioObj.targetGameObject.transform.InverseTransformPoint(magioObj.customStartOrigin.position);

                magioObj.UpdateVFX(effects, effectSpread, localOrigin, approxSize);
            }
        }

        private void StartPreview()
        {
            if (magioObj.vfxSpawnerType == VFXSpawnerType.Mesh)
            {
                StartMeshEffect();
            }
            else
            {
                StartSkinnedMeshEffect();
            }
        }

        private void StartMeshEffect()
        {
            MeshFilter[] filters;

            if (magioObj.useEffectOnAllRenderers)
            {
                filters = transform.GetComponentsInChildren<MeshFilter>();
            }
                
            else
            {
                List<MeshFilter> filterList = new List<MeshFilter>();
                foreach(Renderer rend in magioObj.magioRenderers)
                {
                    MeshFilter fil = rend.GetComponent<MeshFilter>();
                    if (fil) filterList.Add(fil);
                }

                filters = filterList.ToArray();
            }
                
            foreach (MeshFilter filter in filters)
            {
                VisualEffect magioEff = Instantiate(magioObj.EffectPrefab, Vector3.zero, Quaternion.identity, MagioEngine.instance.effectParent).GetComponent<VisualEffect>();

                if (!magioEff.HasMesh("Magio_Mesh"))
                {
                    Debug.LogError("IF you are getting this error when importing Magio for first time, Please install Visual Effects Graph and Re-import Magio Folder (Right-click->Re-import). Otherwise: Effect invalid for Meshes. Please check that your effect has property called Magio_Mesh");
                    return;
                }

                magioEff.SetMesh("Magio_Mesh", filter.sharedMesh);

                if (magioEff.HasBool("Magio_UseSkinnedMesh"))
                {
                    magioEff.SetBool("Magio_UseSkinnedMesh", false);
                }

                MagioObjectEffect.MagioEffectObject magObj = new MagioObjectEffect.MagioEffectObject() { boundObject = filter.gameObject.transform, magioEffect = magioEff, size = Vector3.Scale(filter.sharedMesh.bounds.size, filter.transform.lossyScale).magnitude };

                effects.Add(magObj);
            }

            CalculateApproxSize();
        }

        private void StartSkinnedMeshEffect()
        {
            SkinnedMeshRenderer[] renderers;

            if (magioObj.useEffectOnAllRenderers)
            {
                renderers = magioObj.targetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            else
            {
                renderers = magioObj.magioRenderers.OfType<SkinnedMeshRenderer>().ToArray();
            }

            if (renderers.Length <= 0)
            {
                Debug.LogError("No Skinned mesh renderer found! Please change VFX spawner type to mesh or attach skinned mesh renderer to child");
                return;
            }

            foreach(SkinnedMeshRenderer skinnedMesh in renderers)
            {
                VisualEffect magioEff = Instantiate(magioObj.EffectPrefab, Vector3.zero, Quaternion.identity, MagioEngine.instance.effectParent).GetComponent<VisualEffect>();

                if (!magioEff.HasSkinnedMeshRenderer("Magio_SkinnedMesh"))
                {
                    Debug.LogError("IF you are getting this error when importing Magio for first time, Please install Visual Effects Graph and Re-import Magio Folder (Right-click->Re-import). Otherwise: Effect invalid for Skinned Meshes.  Please check that your effect has property called Magio_SkinnedMesh");
                    return;
                }

                magioEff.SetSkinnedMeshRenderer("Magio_SkinnedMesh", skinnedMesh);

                if (magioEff.HasBool("Magio_UseSkinnedMesh"))
                {
                    magioEff.SetBool("Magio_UseSkinnedMesh", true);
                }

                MagioObjectEffect.MagioEffectObject magObj = new MagioObjectEffect.MagioEffectObject() { boundObject = skinnedMesh.transform, skinnedMeshRenderer = skinnedMesh, magioEffect = magioEff, size = Vector3.Scale(skinnedMesh.sharedMesh.bounds.size, skinnedMesh.transform.lossyScale).magnitude };

                effects.Add(magObj);

                CalculateApproxSize();
            }
        }
      

        private void OnDisable()
        {
            Clean();
            
        }

        private void Clean()
        {
            if (effects != null)
            {
                foreach (MagioObjectEffect.MagioEffectObject effectObj in effects)
                {
                    if (effectObj.magioEffect)
                        Undo.DestroyObjectImmediate(effectObj.magioEffect.gameObject);
                }
                effects.Clear();
                effects = null;
            }

            if (tempColliders != null)
            {
                foreach (BoxCollider tempCol in tempColliders)
                {
                    if (tempCol)
                        DestroyImmediate(tempCol.gameObject);
                }
                tempColliders.Clear();
                tempColliders = null;
            }

            if (flammableColliders != null)
            {
                flammableColliders.Clear();
                flammableColliders = null;
            }
        }

        private void OnDestroy()
        {
            Clean();
            EditorApplication.delayCall += () =>
            {
                if (controller) GameObject.DestroyImmediate(controller);
            };
        }
                
    }
}
#endif