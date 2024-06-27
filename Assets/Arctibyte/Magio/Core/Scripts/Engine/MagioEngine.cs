using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Magio
{
    /// <summary>
    /// Singleton to handle all the general Magio properties and settings
    /// </summary>
    public class MagioEngine : MonoBehaviour
    {
        public static MagioEngine _instance;

        public static MagioEngine instance
        {
            get
            {
                if (_instance == null)
                    _instance = (MagioEngine)FindObjectOfType(typeof(MagioEngine));
                return _instance;
            }
        }

        
        [Tooltip("Where to parent the flames")]
        public Transform effectParent;

        [Tooltip("Where to parent the runtime trees")]
        public Transform runTimeTreeParent;

        [Tooltip("Where to parent the VSPro masks")]
        public Transform vsProMaskParent;

        [Tooltip("Splash effects are added here.")]
        public Transform splashEffectParent;

        [HideInInspector]
        public WindRetrieve flameWindRetriever;

        public NullifyRuleSet nullifyRuleSet;

        public List<MagioEffectPack> effectPacks = new List<MagioEffectPack>();

        [Header("Compatibility")]
        [Tooltip("List of all compatible shaders. Shaders not in project will be removed at Runtime Start()")]
        public List<OAVAShaderCompatibilitySO> compatibleShaders = new List<OAVAShaderCompatibilitySO>();

        [Tooltip("---Remember to backup your terrain data before using! Magio modifies terrain data and data can be lost in case of blue screen!--- Is Ignis compatible with Unity Terrain? If you do not want terrain trees to burn enabling this will affect negatively on performance")]
        public bool unityTerrainCompatible = false;

        [Tooltip("Is Ignis VSPro Compatible? If you don't want to burn Vegetation Studio Vegetation leave this off, otherwise it will negatively impact your performance")]
        public bool VegetationStudioProCompatible = false;

        [Header("Performance")]

        [Range(0.5f, 60)]
        [Tooltip("How often do we check for collisions with flame in second? Smaller = more efficient, bigger = more realistic. Value 5 = 5 times per second.")]
        public float SpreadPhysicsCheckFrequency = 5;

        [Tooltip("Option to enable the on touch collision check (If you have e.g. snow on the TVE rock which you want to melt)")]
        public bool enableOnTouchChecks = false;

        [Tooltip("If you are using Magio Mesh Prefab replacers enable this. Provides small performance boost if not using the replacers and is disabled.")]
        public bool enableMeshPrefabReplacers = false;

        [Tooltip("Allows modifying constant flame attributes on runtime. This will impact performance. Disable if you do not need to scale the object/modify flames on runtime to gain small performance boost.")]
        public bool modifyEffectParametersOnRuntime = true;

        [HideInInspector]
        public bool creatingDebug = false;

        [HideInInspector]
        public bool pause = false;

        private Dictionary<(Terrain, int), GameObject> transformedTerrainTrees = new Dictionary<(Terrain, int), GameObject>();

#if VEGETATION_STUDIO_PRO
        [HideInInspector]
        public AwesomeTechnologies.VegetationSystem.VegetationSystemPro vsPro;
#endif

        private void Awake()
        {
            if (_instance != null) Destroy(instance);

            _instance = this;
        }

        public void Start()
        {
#if VEGETATION_STUDIO_PRO
            vsPro = FindObjectOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>();
#endif

            for (int i = 0; i < compatibleShaders.Count; i++)
            {
                OAVAShaderCompatibilitySO item = compatibleShaders[i];

                if(item == null)
                {
                    compatibleShaders.RemoveAt(i);
                    i--;
                }
                else if (!item.ShaderName.Equals(string.Empty) && !Shader.Find(item.ShaderName))
                {
                    compatibleShaders.RemoveAt(i);
                    i--;
                }
            }

        }

        /// <summary>
        /// Pauses the effects.
        /// </summary>
        public void PauseEffects()
        {
            pause = true;

            foreach(VisualEffect eff in effectParent.GetComponentsInChildren<VisualEffect>())
            {
                eff.pause = true;
            }
        }

        /// <summary>
        /// Resumes the effects (if paused)
        /// </summary>
        public void ResumeEffects()
        {
            pause = false;

            foreach (VisualEffect eff in effectParent.GetComponentsInChildren<VisualEffect>())
            {
                eff.pause = false;
            }
        }

        /// <summary>
        /// Gets nullify rule parameters if it exists. Returns -1 if rule does not exist.
        /// </summary>
        /// <param name="origin">origin class</param>
        /// <param name="target">Target class</param>
        /// <returns>-1 if rule does not exist. Else NullifyLagBehind_m</returns>
        public float GetNullifyRuleLagBehind(EffectClass origin, EffectClass target)
        {
            return nullifyRuleSet.GetNullifyRuleLagBehind(origin, target);
        }
        
        /// <summary>
        /// Gets all compatible shaders.
        /// </summary>
        /// <returns>List of compatible shaders.</returns>
        public List<OAVAShaderCompatibilitySO> GetCompatibleShaders()
        {
            return compatibleShaders;
        }

        /// <summary>
        /// Masks a terrain tree and spawns a prefab on it's place if all the conditions are fulfilled. BEWARE: This is slow operation if there are lots of trees.
        /// </summary>
        /// <param name="other">Spreading object</param>
        /// <param name="hitPoint">Where it is hit</param>
        /// <param name="affectedClass">Which class is affected</param>
        /// <param name="dist">Distance from the hitpoint</param>
        /// <returns>Spawned gameobject</returns>
        public GameObject MaskUnityTerrainTreeAndInstanceAPrefabIfNecessary(GameObject other, Vector3 hitPoint, EffectClass affectedClass, float dist)
        {

            Terrain terrain = other.GetComponent<Terrain>();
            if (!terrain)
                return null;

            hitPoint.y = terrain.SampleHeight(hitPoint);
            TerrainData data = terrain.terrainData;
            float width = data.size.x;
            float height = data.size.z;
            float y = data.size.y;

            int treeID = -1;
            int treeCount = terrain.terrainData.treeInstances.Length;
            float treeDist = float.MaxValue;
            Vector3 treePos = new Vector3(0, 0, 0);

            for (int i = 0; i < treeCount; i++)
            {
                Vector3 thisTreePos = Vector3.Scale(terrain.terrainData.treeInstances[i].position, terrain.terrainData.size) + terrain.transform.position;
                float thisTreeDist = Vector3.Distance(thisTreePos, hitPoint);

                if (thisTreeDist < treeDist)
                {
                    treeID = i;
                    treeDist = thisTreeDist;
                    treePos = thisTreePos;
                }
            }

            if(treeDist > dist)
            {
                return null;
            }

            if (treeID == -1)
            {
                return null;
            }

            if(transformedTerrainTrees.ContainsKey((terrain, treeID)))
            {
                return null;
            }

            TreeInstance tree = terrain.terrainData.treeInstances[treeID];

            if (tree.prototypeIndex >= data.treePrototypes.Length)
                return null;

            var _tree = data.treePrototypes[tree.prototypeIndex].prefab;
            MagioObjectEffect[] treeMagios = _tree.GetComponentsInChildren<MagioObjectEffect>();
            MagioObjectEffect treeMagio = null;
            foreach (MagioObjectEffect magioEff in treeMagios)
            {
                if (affectedClass == magioEff.effectClass)
                {
                    treeMagio = magioEff; break;
                }
            }
            if (!treeMagio) return null;

            Vector3 position = new Vector3(
                tree.position.x * width,
                tree.position.y * y,
                tree.position.z * height) + terrain.transform.position;

            Vector3 scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
            GameObject go = Instantiate(_tree, position, Quaternion.Euler(0f, Mathf.Rad2Deg * tree.rotation, 0f), MagioEngine.instance.runTimeTreeParent) as GameObject;
            go.transform.localScale = scale;

            TreeInstance newInst = tree;
            newInst.widthScale = 0;
            newInst.heightScale = 0;

            data.SetTreeInstance(treeID, newInst);

            transformedTerrainTrees.Add((terrain, treeID), go);

            return go;
        }

        private void OnApplicationQuit()
        {
            foreach ((Terrain, int) treeInTerrain in transformedTerrainTrees.Keys)
            {
                Terrain terrain = treeInTerrain.Item1;

                TreeInstance newInst = terrain.terrainData.treeInstances[treeInTerrain.Item2];
                newInst.widthScale = transformedTerrainTrees[treeInTerrain].transform.localScale.x;
                newInst.heightScale = transformedTerrainTrees[treeInTerrain].transform.localScale.y;
                terrain.terrainData.SetTreeInstance(treeInTerrain.Item2, newInst);
            }

        }

        /// <summary>
        /// Masks Vegetation Studio instance and spawns a prefab in it's place if conditions are fulfilled.
        /// </summary>
        /// <param name="other">Spreading Gameobject</param>
        /// <param name="affectedClass">Class affected by this spreading</param>
        /// <returns>Spawned Prefab</returns>
        public GameObject MaskInstanceAndSpawnAPrefabIfNecessary(GameObject other, EffectClass affectedClass)
        {
#if VEGETATION_STUDIO_PRO
            AwesomeTechnologies.Vegetation.VegetationItemInstanceInfo vegetationItemInstanceInfo = other.gameObject.GetComponent<AwesomeTechnologies.Vegetation.VegetationItemInstanceInfo>();

            // In case of "From prefab" collider
            if (!vegetationItemInstanceInfo && other.transform.parent)
                vegetationItemInstanceInfo = other.transform.parent.GetComponent<AwesomeTechnologies.Vegetation.VegetationItemInstanceInfo>();

            if (vegetationItemInstanceInfo)
            {
                MagioObjectEffect magioObj = vegetationItemInstanceInfo.gameObject.GetComponentInChildren<MagioObjectEffect>();

               
                if (!magioObj)
                {
                    if (!runTimeTreeParent.GetComponentsInChildren<VegetationStudioProTreeUnMasker>().Any(o => o.VegetationInstanceItemId == vegetationItemInstanceInfo.VegetationItemInstanceID))
                    {
                        AwesomeTechnologies.Vegetation.RuntimeObjectInfo runInfo = vegetationItemInstanceInfo.gameObject.GetComponent<AwesomeTechnologies.Vegetation.RuntimeObjectInfo>();

                        AwesomeTechnologies.VegetationSystem.VegetationItemInfoPro info = runInfo.VegetationItemInfo;
                        
                        if (info != null)
                        {
                            GameObject tree = Instantiate(info.VegetationPrefab, vegetationItemInstanceInfo.Position, vegetationItemInstanceInfo.Rotation, MagioEngine.instance.runTimeTreeParent);
                            tree.transform.localScale = vegetationItemInstanceInfo.Scale;
                            MagioObjectEffect[] magioObjs = tree.GetComponentsInChildren<MagioObjectEffect>();

                            MagioObjectEffect magioTreeObj = null;
                            

                            foreach (MagioObjectEffect eff in magioObjs)
                            {
                                if (eff.effectClass == affectedClass) magioTreeObj = eff;
                            }


                            if (magioTreeObj)
                            {
                                GameObject vegetationItemMaskObject = new GameObject { name = "VegetationItemMask - " + other.gameObject.name };
                                vegetationItemMaskObject.transform.parent = vsProMaskParent;
                                vegetationItemMaskObject.transform.position = vegetationItemInstanceInfo.Position;

                                vegetationItemMaskObject.AddComponent<AwesomeTechnologies.Vegetation.Masks.VegetationItemMask>().SetVegetationItemInstanceInfo(vegetationItemInstanceInfo);

                                VegetationStudioProTreeUnMasker unMasker = tree.AddComponent<VegetationStudioProTreeUnMasker>();
                                unMasker.mask = vegetationItemMaskObject;
                                unMasker.VegetationInstanceItemId = vegetationItemInstanceInfo.VegetationItemInstanceID;
                                unMasker.magioObj = magioTreeObj;
                            }
                            else
                            {
                                Destroy(tree);
                            }

                            return vegetationItemInstanceInfo.gameObject;
                        }
                    }
                }
            }
#endif
            return null;
        }

    }
}
